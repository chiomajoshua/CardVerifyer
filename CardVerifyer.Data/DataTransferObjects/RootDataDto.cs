namespace CardVerifyer.Data.DataTransferObjects
{
    public class RootDataDto : Resource
    {
        public string Scheme { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public bool Prepaid { get; set; }
        public CountryDto Country { get; set; }
        public BankDto Bank { get; set; }
    }
}