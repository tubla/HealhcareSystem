using System.ComponentModel.DataAnnotations.Schema;

namespace authentication.models.V1.Db;

[Table("role_permission", Schema = "healthcare")]
public class RolePermission
{
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("permission_id")]
    public int PermissionId { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
