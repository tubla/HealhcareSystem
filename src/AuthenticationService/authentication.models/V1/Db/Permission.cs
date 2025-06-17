using System.ComponentModel.DataAnnotations.Schema;

namespace authentication.models.V1.Db;

[Table("permission", Schema = "healthcare")]
public class Permission
{
    [Column("permission_id")]
    public int PermissionId { get; set; }

    [Column("permission_name")]
    public string PermissionName { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
