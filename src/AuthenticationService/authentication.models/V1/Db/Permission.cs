namespace authentication.models.V1.Db;

public class Permission
{
    public int PermissionID { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public int RoleID { get; set; }
    public Role Role { get; set; } = new();
}
