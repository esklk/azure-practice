namespace AZP.DataModels
{
    public record EmploymentStoredRecord(
        string Id,
        DateTime CreatedAt,
        Employee Employee,
        Company Company,
        string Email,
        string Phone,
        string Title)
    {
    }
}
