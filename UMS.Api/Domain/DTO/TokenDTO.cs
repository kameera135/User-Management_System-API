namespace UMS.Api.Domain.DTO
{
    public class TokenDTO
    {
        public long TokenId { get; set; }
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpireDate { get; set; }
    }

    public class TokenPostDTO
    {
        public string Token { get; set; } = null!;

    }

    public class TokenPutDTO
    {
        public long TokenId { get; set; }
        public string Token { get; set; } = null!;
    }
}
