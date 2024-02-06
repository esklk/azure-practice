namespace AZP.DataModels
{
    public record EmploymentRecordUpdate(
        Employee Employee,
        Company Company,
        string Email,
        string Phone,
        string Title);
}
