namespace authentication.models.V1.Dtos;

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = [];
}
