using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Entities
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public Guid AdminId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
