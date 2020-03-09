using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DesktopService.Features.DeviceFeature
{
    [Authorize]
    [Route("features")]
    [ApiController]
    public class DeviceFeatureController : BaseController
    {
        private readonly IFeatureService _featureService;
        private readonly ILogger<DeviceFeatureController> _logger;

        public DeviceFeatureController(IFeatureService featureService, ILoggerFactory loggerFactory)
        {
            _featureService = featureService;
            _logger = loggerFactory.CreateLogger<DeviceFeatureController>();
        }

        [HttpGet("ui/{featureId}")]
        public async Task<IActionResult> UIArchives([FromRoute]Guid featureId)
        {
            var archivePath = _featureService.GetFeatureUIArchivePath(featureId);
            if (!string.IsNullOrWhiteSpace(archivePath) && System.IO.File.Exists(archivePath))
            {
                return File(await System.IO.File.ReadAllBytesAsync(archivePath), MediaTypeNames.Application.Zip, $"ui.zip");
            }
            return NotFound();
        }
    }
}
