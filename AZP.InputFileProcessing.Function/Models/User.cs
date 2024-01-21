namespace AZP.InputFileProcessing.Function.Models
{
    public record User(
        int Id,
        string FirstName,
        string LastName,
        string Username,
        string Email,
        string Phone,
        string Website,
        Company Company,
        Address Address)
    {
        // required for CosmosDb
        public string id => FirstName;
    }
}
