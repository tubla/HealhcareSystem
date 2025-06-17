using System.ComponentModel.DataAnnotations.Schema;

namespace authentication.models.V1.Db;

[Table("user", Schema = "healthcare")]
public class User
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("user_name")]
    public string UserName { get; set; } = string.Empty;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
