namespace CashVault.Application.Common.Models
{
    public class SessionUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Company { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
