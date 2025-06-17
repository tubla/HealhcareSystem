using System.ComponentModel.DataAnnotations.Schema;

namespace authentication.models.V1.Db;

[Table("role", Schema = "healthcare")]
public class Role
{
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
