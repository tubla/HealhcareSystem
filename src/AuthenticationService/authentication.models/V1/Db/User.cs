namespace authentication.models.V1.Db;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleID { get; set; }
    public Role Role { get; set; } = new();
}
