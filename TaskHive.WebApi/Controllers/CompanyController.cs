using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskHive.Infrastructure.Repositories;

namespace TaskHive.WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        /// <summary>
        /// Provides accounts that belong company, excluding accounts that belong given workspace
        /// </summary>
        /// <param name="searchTerm">Provides a search term to filter results</param>
        /// <param name="sortColumn">Determine whether is intending to sort by first name (firstname), last name (lastname) or email (email), default is according to current database id's</param>
        /// <param name="sortOrder">Determine whether is intending to order by ascending (asc) or descending (desc)</param>
        /// <param name="page">Determine desired page to retrieve items</param>
        /// <param name="pageSize">Determine desired size for each page containing items</param>
        /// <response code="200">List of account details</response>
        /// <response code="404">Authenticated account not found</response>
        /// <response code="500">Internal error</response>
        [HttpGet("company/except/{workspaceId}")]
        [Authorize]
        public async Task<IActionResult> GetAccountsForCompany(Guid workspaceId,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            AccountRepository accountRepository = new();
            try
            {
                var email = User.Claims.Where(e => e.Value.Contains('@')).First().Value;
                var user = await accountRepository.GetActiveAccountByEmailAsync(email);
                if (user == null) return NotFound(new { message = "Account not found." });

                return Ok(await accountRepository.GetAccountsInCompanyExcludingInWorkspace(user.CompanyId, workspaceId,
                    searchTerm, sortColumn, sortOrder, page, pageSize));
            }
            catch (Exception ex)
            {
                return ValidationProblem(ex.Message);
            }
        }
    }
}
