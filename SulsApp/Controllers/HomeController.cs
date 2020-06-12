namespace SulsApp.Controllers
{
    using System;
    using SIS.HTTP;
    using SIS.MvcFramework;
    using ViewModels;

    public class HomeController : Controller
    {
        [HttpGet("/")]
        public HttpResponse Index()
        {
            var viewModel = new IndexViewModel
            {
                Message = "Welcome to SULS Platform!",
                Year = DateTime.UtcNow.Year,
            };

            return this.View(viewModel);
        }
    }
}
