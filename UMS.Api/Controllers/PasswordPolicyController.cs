using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class PasswordPolicyController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IPasswordPolicyService m_passwordPolicyService;

        public PasswordPolicyController(ILogService logger, IPasswordPolicyService passwordPolicyService)
        {
            m_logService = logger;
            m_passwordPolicyService = passwordPolicyService;
        }

        //UPDATE PASSWORD POLICY

        [HttpPut("password-policy")]
        public IActionResult UpdatePasswordPolicy(string passwordPolicy, long updatedBy)
        {
            m_logService.Log("Password Policy -> UpdatePasswordPolicy() was called");

            try
            {
                m_passwordPolicyService.updatePasswordPolicy(passwordPolicy, updatedBy);

                m_logService.Log("Edit Password Policy: Updated successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            finally
            {

            }
        }
    }
}
