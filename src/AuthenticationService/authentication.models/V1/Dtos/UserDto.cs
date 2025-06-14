namespace authentication.models.V1.Dtos;

public class UserDto
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleID { get; set; }
}
