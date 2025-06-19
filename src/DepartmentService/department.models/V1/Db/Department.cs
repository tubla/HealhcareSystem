using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace department.models.V1.Db;

[Table("department")]
public class Department
{
    [Key]
    [Column("dept_id")]
    public int DeptId { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}