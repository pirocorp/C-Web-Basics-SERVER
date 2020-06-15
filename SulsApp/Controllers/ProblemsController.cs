namespace SulsApp.Controllers
{
    using Services;
    using SIS.HTTP;
    using SIS.MvcFramework;

    public class ProblemsController : Controller
    {
        private readonly IProblemsService _problemsService;

        public ProblemsController(IProblemsService problemsService)
        {
            this._problemsService = problemsService;
        }

        public HttpResponse Create()
        {
            return this.View();
        }

        [HttpPost("/Problems/Create")]
        public HttpResponse DoCreate(string name, int points)
        {
            this._problemsService.Create(name, points);
            return this.Redirect("/");
        }
    }
}
