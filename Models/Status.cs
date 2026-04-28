using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        public Guid StatusId { get; set; }
        public string? StatusName { get; set; }
        public bool? IsActive { get; set; }
    }
}
