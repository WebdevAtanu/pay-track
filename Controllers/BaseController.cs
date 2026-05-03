using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace payroll_mvc.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        
    }
}
