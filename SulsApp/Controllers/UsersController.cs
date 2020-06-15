namespace SulsApp.Controllers
{
    using System;
    using System.Net.Mail;
    using Services;
    using SIS.HTTP;
    using SIS.HTTP.Logging;
    using SIS.MvcFramework;

    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ILogger _loggerService;

        public UsersController(IUsersService usersService, ILogger loggerService)
        {
            this._usersService = usersService;
            this._loggerService = loggerService;
        }

        public HttpResponse Login()
        {
            return this.View();
        }

        [HttpPost("/Users/Login")]
        public HttpResponse DoLogin()
        {
            var username = this.Request.FormData["username"];
            var password = this.Request.FormData["password"];

            var userId = this._usersService.GetUserId(username, password);

            if (userId == null)
            {
                return this.Redirect("/Users/Login");
            }

            this.SignIn(userId);
            this._loggerService.Log("User logged in: " + username);
            return this.Redirect("/");
        }

        public HttpResponse Register()
        {
            return this.View();
        }

        [HttpPost("/Users/Register")]
        public HttpResponse DoRegister()
        {
            var username = this.Request.FormData["username"];
            var email = this.Request.FormData["email"];
            var password = this.Request.FormData["password"];
            var confirmPassword = this.Request.FormData["confirmPassword"];

            if (password != confirmPassword)
            {
                return this.Error("Confirm password does not match password");
            }

            if (string.IsNullOrWhiteSpace(username)
                || username.Length < 5
                || username.Length > 20)
            {
                return this.Error("Username should be between 5 and 20 long.");
            }

            if (!IsValid(email))
            {
                return this.Error("Invalid email.");
            }

            if (string.IsNullOrWhiteSpace(password)
                || password.Length < 6
                || password.Length > 20)
            {
                return this.Error("Password should be between 6 and 20 long.");
            }

            this._usersService.CreateUser(username, email, password);

            var userId = this._usersService.GetUserId(username, password);
            this.SignIn(userId);
            this._loggerService.Log("New user: " + username);

            //TODO: Email already exists
            return this.Redirect("/");
        }

        public HttpResponse Logout()
        {
            this.SignOut();

            return this.Redirect("/");
        }

        private bool IsValid(string emailAddress)
        {
            try
            {
                var m = new MailAddress(emailAddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
