using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Identity
{
    public interface IUserService
    {
        Models.User Authenticate(string username, string passwordKey);
        IEnumerable<Models.User> GetAll();
        Models.User GetById(Guid id);
    }
}
