using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Identity
{
    public interface IUserService
    {
        event EventHandler<Models.User> NewUserAdded;
        Models.User Authenticate(string username, string passwordKey);
        IEnumerable<Models.User> GetAll();
        Models.User GetById(Guid id);
        Models.User GetByName(string userName);
        void AddUser(string userName);
    }
}
