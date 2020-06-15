namespace SulsApp.Controllers
{
    using System;
    using System.Net.Mail;
    using Models;
    using Services;
    using SIS.HTTP;
    using SIS.MvcFramework;

    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController()
        {
            this._usersService = new UsersService(new ApplicationDbContext());
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
