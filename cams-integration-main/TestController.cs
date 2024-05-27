using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SITCAMSClientIntegration.AuthorizationMiddleware;

namespace SITCAMSClientIntegration
{
    /// This uses the "Token" authentication scheme to authenticate calls to all methods in the controller
    [Authorize(AuthenticationSchemes = "Token")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController : ControllerBase
    {

        /// This uses the RequiredPermission Authorization Attribute to check the permissions of the user for this particular method only.
        [RequiredPermission("some-permission")]
        [HttpGet("permission")]
        public ActionResult<string> TestPermissionMethod()
        {
            return Ok("Successfully accessed method with using Token authentication and requiring 'some-permission' permission");
        }

        /// This uses the RequiredRank Authorization Attribute to check the rank of the user for this particular method only.
        [RequiredRank(3)]
        [HttpGet("requiredRank")]
        public ActionResult<string> TestRequiredRankMethod()
        {
            return Ok("Successfully accessed method with using Token authentication and requiring rank 3");
        }

        /// This uses the Rank Authorization Attribute to check the rank of the user and compare it 
        /// to the configured rank in the context for this particular method only.
        [Authorize("Rank")]
        [HttpGet("rank")]
        public ActionResult<string> TestRankMethod()
        {
            return Ok("Successfully accessed method with using Token authentication and requiring rank based on database");
        }

        /// This uses the Rank Authorization Attribute to check the rank of the user and compare it 
        /// to the configured rank in the context for this particular method only. It is the same as the above.
        [VerifyRank]
        [HttpGet("verify-rank")]
        public ActionResult<string> TestVerifyRankMethod()
        {
            return Ok("Successfully accessed method with using Token authentication and requiring rank based on database");
        }

        /// This uses the RequiredRole authorization attribute to check the role of the user and compare it 
        /// to the required role for this particular method only.
        [RequiredRole("level1")]
        [HttpGet("role")]
        public ActionResult<string> TestRoleMethod()
        {
            return Ok("Successfully accessed method with using Token authentication and requiring role level1");
        }

    }
}