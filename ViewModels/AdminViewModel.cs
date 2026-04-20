namespace payroll_mvc.ViewModels
{
    public class AdminViewModel
    {
    }

    public class LoginViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
