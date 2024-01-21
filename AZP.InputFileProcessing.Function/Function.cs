using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AZP.InputFileProcessing.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AZP.InputFileProcessing.Function
{
    public static class Function
    {
        [FunctionName("Function")]
        public static async Task Run(
            [BlobTrigger("input-files/{inputBlobName}", Connection = "StorageConnectionString")]Stream data,
            string inputBlobName,
            [CosmosDB("azp-data", "users", Connection = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            using (log.BeginScope("Processing file: {InputBlobName}", inputBlobName))
            {
                var inputUsers = await GetInputUsers(data, log);
                if (!inputUsers.Any())
                {
                    log.LogWarning("No users in the file.");
                }

                foreach (var inputUser in inputUsers)
                {
                    try
                    {
                        await documentsOut.AddAsync(inputUser);
                        log.LogInformation("User {UserId} successfully saved.", inputUser.Id);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "User save failed: {Reason}.", ex.Message);
                    }
                }
            }
        }

        private static async Task<User[]> GetInputUsers(Stream inputStream, ILogger log)
        {
            try
            {
                string inputText;
                using (var streamReader = new StreamReader(inputStream))
                {
                    inputText = await streamReader.ReadToEndAsync();
                }

                return JsonConvert.DeserializeObject<User[]>(inputText);
            }
            catch (JsonException ex)
            {
                log.LogError("Deserialization failed: {Reason}", ex.Message);
                throw;
            }
        }
    }
}
