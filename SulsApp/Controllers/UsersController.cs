namespace SulsApp.Controllers
{
    using System;
    using System.Net.Mail;
    using Services;
    using SIS.HTTP;
    using SIS.HTTP.Logging;
    using SIS.MvcFramework;
    using ViewModels.Users;

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
        public HttpResponse DoLogin(string username, string password)
        {
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
        public HttpResponse DoRegister(RegisterInputModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return this.Error("Confirm password does not match password");
            }

            if (string.IsNullOrWhiteSpace(model.Username)
                || model.Username.Length < 5
                || model.Username.Length > 20)
            {
                return this.Error("Username should be between 5 and 20 long.");
            }

            if (!IsValid(model.Email))
            {
                return this.Error("Invalid email.");
            }

            if (string.IsNullOrWhiteSpace(model.Password)
                || model.Password.Length < 6
                || model.Password.Length > 20)
            {
                return this.Error("Password should be between 6 and 20 long.");
            }

            this._usersService.CreateUser(model.Username, model.Email, model.Password);

            var userId = this._usersService.GetUserId(model.Username, model.Password);
            this.SignIn(userId);
            this._loggerService.Log("New user: " + model.Username);

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
