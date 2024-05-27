using SITCAMSClientIntegration.Options;
using Microsoft.AspNetCore.Mvc;
using System;
using SITCAMSClientIntegration.Responses;
using Microsoft.Extensions.Options;

namespace SITCAMSClientIntegration.Controllers
{
    /// <summary>
    /// Get platform Info
    /// </summary>
    [Route("api/platform-info")]
    [ApiController]
    public class PlatformInfoController : ControllerBase
    {
        private readonly CentralAuthOptions _centralAuthOptions;

        public PlatformInfoController(IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _centralAuthOptions = centralAuthOptions.Value;
        }

        /// <summary>
        /// Get the current platform's ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPlatformId()
        {
            try
            {
                var dateTimeWithTimeZone = DateTimeOffset.Now;
                string toLocal = dateTimeWithTimeZone.ToLocalTime().ToString("zzz");

                return Ok(new { _centralAuthOptions.PlatformId, TimeZone = toLocal });
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Get(false, e.Message));
            }
        }
    }
}
