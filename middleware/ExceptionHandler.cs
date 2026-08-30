using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace payroll_mvc.Middleware
{
    public class ExceptionHandler : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        public ExceptionHandler(ILogger<ExceptionHandler> logger, ITempDataDictionaryFactory tempDataFactory)
        {
            _logger = logger;
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);

            // Store message
            tempData["ErrorMessage"] = context.Exception.Message;

            context.Result = new RedirectToActionResult("Error", "Home", new { area = "" });

            context.ExceptionHandled = true;
        }
    }
}
