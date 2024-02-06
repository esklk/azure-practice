using AZP.DataModels;
using Azure;
using Azure.Data.Tables;

namespace AZP.DataDecomposition.Function.Models
{
    public record CompanyTableEntity(
        string Cin,
        string Name,
        string CatchPhrase,
        string Bs, 
        string Country, 
        string City,
        string Street, 
        string Suite, 
        string ZipCode) : ITableEntity
    {
        public string PartitionKey { get; set; } = Cin;
        public string RowKey { get; set; } = Cin;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
