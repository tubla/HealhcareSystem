using System.ComponentModel.DataAnnotations.Schema;

namespace authentication.models.V1.Db;

[Table("user_role", Schema = "healthcare")]
public class UserRole
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
