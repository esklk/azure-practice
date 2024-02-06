namespace AZP.DataModels
{
    public record Employee(
        string FirstName,
        string LastName,
        DateTime Birthday,
        Address Address);
}
