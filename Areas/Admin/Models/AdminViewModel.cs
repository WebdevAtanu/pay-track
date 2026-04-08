namespace payroll_mvc.Areas.Admin.Models
{
    public class AdminViewModel
    {
        public Guid AdminId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
