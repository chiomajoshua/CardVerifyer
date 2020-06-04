namespace CardVerifyer.Data.DataTransferObjects
{
    public class ResponseDataDto
    {
        public int ResponseCode { get; set; }
        public RootDataDto ResponseObject { get; set; }
        public string ResponseMessage { get; set; }
    }
}