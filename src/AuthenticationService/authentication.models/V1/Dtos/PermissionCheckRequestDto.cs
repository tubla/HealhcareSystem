namespace authentication.models.V1.Dtos;

public class PermissionCheckRequestDto
{
    public int UserId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}
