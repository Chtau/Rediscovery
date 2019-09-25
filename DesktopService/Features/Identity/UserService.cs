using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DesktopService.Features.Identity.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DesktopService.Features.Identity
{
    public class UserService : IUserService
    {
        public event EventHandler<User> NewUserAdded;

        private readonly Models.IdentitySettings _identitySettings;
        private readonly Random _random;
        private readonly DAL.IDBContext _dBContext;

        public UserService(IOptions<Models.IdentitySettings> identitySettings, DAL.IDBContext dBContext)
        {
            _dBContext = dBContext;
            _identitySettings = identitySettings.Value;
            _random = new Random();
        }


        public async Task<User> Authenticate(string username, string passwordKey)
        {
            var user = await _dBContext.Instance.Table<Models.User>().FirstOrDefaultAsync(x => x.UserName == username && x.PasswordKey == passwordKey && x.PasswordKeyValidTill > DateTime.UtcNow);

            // return null if user not found
            if (user == null)
                return null;

            user.AllowAccess = true; // update user db
            user.PasswordKeyValidTill = DateTime.MaxValue;
            await OnUpdateUser(user);

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

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await _dBContext.Instance.Table<Models.User>().ToListAsync();

            return users.Select(x => {
                x.PasswordKey = null;
                return x;
            });
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _dBContext.Instance.Table<Models.User>().FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
                user.PasswordKey = null;

            return user;
        }

        public async Task<User> GetByName(string userName)
        {
#pragma warning disable RCS1155 // Use StringComparison when comparing strings.
            var user = await _dBContext.Instance.Table<Models.User>().FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
#pragma warning restore RCS1155 // Use StringComparison when comparing strings.
            if (user != null)
                user.PasswordKey = null;

            return user;
        }

        public async Task<User> AddUser(string userName)
        {
            User user = await GetByName(userName);
            if (user != null)
            {
                user.Token = null;
                if (user.PasswordKeyValidTill >= DateTime.UtcNow || string.IsNullOrWhiteSpace(user.PasswordKey))
                {
                    user.PasswordKey = OnCreatePasswordKey();
                    user.PasswordKeyValidTill = DateTime.UtcNow.AddMinutes(5);
                }
                await OnUpdateUser(user);
            } else
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    PasswordKey = OnCreatePasswordKey(),
                    UserName = userName,
                    PasswordKeyValidTill = DateTime.UtcNow.AddMinutes(5)
                };
                await OnAddUser(user);
            }
            NewUserAdded?.Invoke(this, user);
            return user;
        }

        private async Task OnUpdateUser(User user)
        {
            await _dBContext.Instance.UpdateAsync(user);
        }

        private async Task OnAddUser(User user)
        {
            await _dBContext.Instance.InsertOrReplaceAsync(user);
        }

        private string OnCreatePasswordKey()
        {
            string retVal = "";
            for (int i = 0; i < _identitySettings.PasswordKeyLength; i++)
            {
                retVal += _random.Next(0, 9);
            }
            return retVal;
        }
    }
}
