namespace UpShopApi.Domain.Models
{
    public class ApiError
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
