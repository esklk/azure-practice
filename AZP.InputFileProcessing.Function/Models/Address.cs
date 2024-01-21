namespace AZP.InputFileProcessing.Function.Models
{
    public record Address(
        string ZipCode,
        string City,
        string Street,
        string Suit,
        Geo Geo);
}
