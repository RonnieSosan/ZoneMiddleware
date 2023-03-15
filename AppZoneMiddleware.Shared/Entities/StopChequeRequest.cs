namespace AppZoneMiddleware.Shared.Entities
{
    public class StopChequeRequest : BaseRequest
    {
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string CustomerID { get; set; }

    }

    public class StopChequeResponse : BaseResponse
    {

    }
}