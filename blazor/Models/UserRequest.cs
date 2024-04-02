namespace BlazorChat.Models
{
    public class UserRequest
    {
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string? TenantId { get; set; }     
        public string? Prompt { get; set; }
    }
}
