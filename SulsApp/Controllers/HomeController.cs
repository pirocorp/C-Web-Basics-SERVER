namespace SulsApp.Controllers
{
    using System;
    using Services;
    using SIS.HTTP;
    using SIS.HTTP.Logging;
    using SIS.MvcFramework;
    using ViewModels;

    public class HomeController : Controller
    {
        private readonly ILogger _loggerService;

        public HomeController(ILogger loggerService)
        {
            this._loggerService = loggerService;
        }

        [HttpGet("/")]
        public HttpResponse Index()
        {
            var viewModel = new IndexViewModel
            {
                Message = "Welcome to SULS Platform!",
                Year = DateTime.UtcNow.Year,
            };

            this._loggerService.Log("Index was accessed");
            return this.View(viewModel);
        }
    }
}
