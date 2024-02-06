using AZP.DataDecomposition.Function.Models;
using AZP.DataModels;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AZP.DataDecomposition.Function
{
    public class Function
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Function(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<Function>();
        }

        [Function("Function")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "azp-data",
            containerName: "employments",
            Connection = "CosmosDbConnectionString")] IReadOnlyList<EmploymentStoredRecord>? input)
        {
            if (input is null || !input.Any())
            {
                _logger.LogWarning("Input is empty.");
                return;
            }

            var tableClient = new TableClient(_configuration["StorageConnectionString"], "Companies");
            await Task.WhenAll(input.Select(record => tableClient.UpsertEntityAsync(new CompanyTableEntity(
                record.Company.Cin,
                record.Company.Name,
                record.Company.CatchPhrase,
                record.Company.Bs,
                record.Company.Address.Country,
                record.Company.Address.City,
                record.Company.Address.Street,
                record.Company.Address.Suite,
                record.Company.Address.Zipcode))));
        }
    }
}
