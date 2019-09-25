using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Identity.Models
{
    public class User
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PasswordKey { get; set; }
        public DateTime PasswordKeyValidTill { get; set; }
        public string Token { get; set; }
        public bool AllowAccess { get; set; }
    }
}
