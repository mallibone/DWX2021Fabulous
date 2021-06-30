namespace Forecast.Models
{
    public class Locality
    {
        public int Postalcode { get; set; }
        public string City { get; set; }
        public string Canton { get; set; }
        public string Abbreviation { get; set; }
        public string Country { get; set; }

        public override string ToString() => $"{Postalcode} {City}";
    }
}