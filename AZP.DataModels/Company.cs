namespace AZP.DataModels;

public record Company(
    string Cin,
    string Name,
    string CatchPhrase,
    string Bs,
    Address Address);