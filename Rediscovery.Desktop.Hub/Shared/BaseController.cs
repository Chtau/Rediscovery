using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Shared
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase
    {
    }
}
