using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Provides accounts that belong company, excluding accounts that belong given workspace
        /// </summary>
        /// <response code="200">List of account details</response>
        /// <response code="500">Internal error</response>
        [HttpGet("company/{companyId}/accounts/except/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetAllAccountsForCompany(Guid companyId, Guid workspaceId)
        {
            AccountRepository accountRepository = new();
            try
            {
                return Ok(await accountRepository.GetAccountsInCompanyExcludingInWorkspace(companyId, workspaceId));
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }
    }
}
