using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Identity
{
    public interface IUserService
    {
        event EventHandler<Models.User> NewUserAdded;
        Task<Models.User> Authenticate(string username, string passwordKey);
        Task<IEnumerable<Models.User>> GetAll();
        Task<Models.User> GetById(Guid id);
        Task<Models.User> GetByName(string userName);
        Task<Models.User> AddUser(string userName);
    }
}
