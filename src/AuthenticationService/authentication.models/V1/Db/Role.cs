namespace authentication.models.V1.Db;

public class Role
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public ICollection<Permission> Permissions { get; set; } = [];
}
