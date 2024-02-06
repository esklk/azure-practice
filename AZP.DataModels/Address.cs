namespace AZP.DataModels;

public record Address(
    string Country,
    string City,
    string Street,
    string Suite,
    string Zipcode,
    Geo Geo);