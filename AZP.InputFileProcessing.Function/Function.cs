using AZP.DataModels;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AZP.InputFileProcessing.Function
{
    public class Function
    {
        private const string OutputDatabaseName = "azp-data";
        private const string OutputContainerName = "employments";

        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<Function> _logger;

        public Function(CosmosClient cosmosClient, ILogger<Function> logger)
        {
            _cosmosClient = cosmosClient;
            _logger = logger;
        }

        [Function(nameof(Function))]
        public async Task Run([BlobTrigger(
                blobPath: "input-files/{inputBlobName}",
                Connection = "StorageConnectionString")]
            Stream inputStream,
            string inputBlobName)
        {
            using (_logger.BeginScope("Processing file: {InputBlobName}", inputBlobName))
            {
                var source = await ExtractSourceData(inputStream);
                _logger.LogInformation("Employment entries extracted: {ExtractedEntriesCount}.", source.Length);
                if (!source.Any())
                {
                    return;
                }

                var container = _cosmosClient.GetContainer(OutputDatabaseName, OutputContainerName);

                await Task.WhenAll(source.Select(update => container.UpsertItemAsync(
                    new EmploymentStoredRecord(
                        Guid.NewGuid().ToString(),
                        DateTime.UtcNow,
                        update.Employee,
                        update.Company,
                        update.Email,
                        update.Phone,
                        update.Title))));

                _logger.LogInformation("Employees successfully saved.");
            }
        }

        private async Task<EmploymentRecordUpdate[]> ExtractSourceData(Stream inputStream)
        {
            try
            {
                string inputText;
                using (var streamReader = new StreamReader(inputStream))
                {
                    inputText = await streamReader.ReadToEndAsync();
                }

                return JsonConvert.DeserializeObject<EmploymentRecordUpdate[]>(inputText) ?? Array.Empty<EmploymentRecordUpdate>();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Deserialization failed: {Reason}", ex.Message);
                throw;
            }
        }
    }
}
