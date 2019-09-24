using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Identity.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        // TODO: password key will be generated only on registry request in IAuth
        public string PasswordKey { get; set; }
        public string Token { get; set; }
        public bool AllowAccess { get; set; }
    }
}
