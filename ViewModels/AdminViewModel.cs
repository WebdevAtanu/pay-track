namespace payroll_mvc.ViewModels
{
    public class AdminViewModel
    {
    }

    public class LoginViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? LoginType { get; set; } // "Admin" or "Employee"
    }

    public class RegisterViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
