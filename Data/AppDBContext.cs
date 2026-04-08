using Microsoft.EntityFrameworkCore;
using payroll_mvc.Models;

namespace payroll_mvc.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
    }
}
