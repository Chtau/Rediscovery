using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DesktopService.Features.Identity.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DesktopService.Features.Identity
{
    public class UserService : IUserService
    {
        public event EventHandler<User> NewUserAdded;

        private List<User> _users = new List<User>
        {
            new User { Id = new Guid("45092B6B-6435-40EC-A01A-E1245C610404"), UserName = "dev", PasswordKey = "123456", AllowAccess = true },
            new User { Id = new Guid("37ED7C16-91DE-4A38-ACEA-8997CBF53D8F"), UserName = "dev1", PasswordKey = "654321", AllowAccess = true }
        };

        private readonly Models.IdentitySettings _identitySettings;
        private readonly Random _random;

        public UserService(IOptions<Models.IdentitySettings> identitySettings)
        {
            _identitySettings = identitySettings.Value;
            _random = new Random();

            OnLoadDemoUsers();
        }

        private void OnLoadDemoUsers()
        {
            var u1 = Authenticate("dev", "123456");
            _users[0].Token = u1.Token;

            var u2 = Authenticate("dev1", "654321");
            _users[1].Token = u1.Token;
        }

        public User Authenticate(string username, string passwordKey)
        {
            var user = _users.SingleOrDefault(x => x.UserName == username && x.PasswordKey == passwordKey);

            // return null if user not found
            if (user == null)
                return null;

            user.AllowAccess = true; // update user db

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                }),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.PasswordKey = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _users.Select(x => {
                x.PasswordKey = null;
                return x;
            });
        }

        public User GetById(Guid id)
        {
            var user = _users.FirstOrDefault(x => x.Id == id);
            if (user != null)
                user.PasswordKey = null;

            return user;
        }

        public User GetByName(string userName)
        {
            var user = _users.FirstOrDefault(x => string.Equals(x.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (user != null)
                user.PasswordKey = null;

            return user;
        }

        public User AddUser(string userName)
        {
            User user = GetByName(userName);
            if (user != null)
            {
                user.Token = null;
                user.PasswordKey = OnCreatePasswordKey();
            } else
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    PasswordKey = OnCreatePasswordKey(),
                    UserName = userName
                };
                _users.Add(user);
            }
            NewUserAdded?.Invoke(this, user);
            return user;
        }

        private string OnCreatePasswordKey()
        {
            string retVal = "";
            for (int i = 0; i < _identitySettings.PasswordKeyLength + 1; i++)
            {
                retVal += _random.Next(0, 9);
            }
            return retVal;
        }
    }
}
