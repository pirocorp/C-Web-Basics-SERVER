namespace SulsApp.Services
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Models;
    using SIS.MvcFramework;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _db;

        public UsersService(ApplicationDbContext db)
        {
            this._db = db;
        }

        public void CreateUser(string username, string email, string password)
        {
            var user = new User()
            {
                Username = username,
                Email = email,
                Password = this.Hash(password),
                Role = IdentityRole.User
            };

            this._db.Users.Add(user);
            this._db.SaveChanges();
        }

        public string GetUserId(string username, string password)
        {
            var hashedPassword = this.Hash(password);

            return this._db.Users
                .Where(u => u.Username == username && u.Password == hashedPassword)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        public void ChangePassword(string username, string newPassword)
        {
            var user = this._db.Users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                return;
            }

            user.Password = this.Hash(newPassword);
            this._db.SaveChanges();
        }

        public int CountUsers()
            => this._db.Users.Count();

        public bool IsUsernameUsed(string username)
            => this._db.Users.Any(u => u.Username == username);

        public bool IsEmailUsed(string email)
            => this._db.Users.Any(u => u.Email == email);

        private string Hash(string input)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2")); //255 => FF
            }

            return hash.ToString();
        }
    }
}
