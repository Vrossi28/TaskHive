using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TaskHive.Core.Entities;
using TaskHive.Core.Utils;
using TaskHive.Infrastructure.Models;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class AccountRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<Account> GetAccountById(Guid accountId)
        {
            Account account = null;
            account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((a) => a.AccountId == accountId && a.AccountState == Core.Enums.State.Active);

            return account;
        }

        public async Task<Account> GetActiveAccountByEmailAsync(string email)
        {

            Account account = null;
            account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((a) => a.Email == email && a.AccountState == Core.Enums.State.Active);

            return account;
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {

            Account account = null;
            account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((a) => a.Email == email);

            return account;
        }

        public async Task<List<Account>> GetAccountsByIds(List<AccountToAdd> accountsToAdd)
        {
            List<Account> accounts = new();

            foreach (var account in accountsToAdd)
            {
                var newAccount = await GetAccountById(account.AccountId);
                if (newAccount != null)
                    accounts.Add(newAccount);
            }

            return accounts;
        }

        public bool ResetTokenAlreadyExists(string resetToken)
        {
            return _dbContext.Account.Where((a) => a.PasswordResetToken == resetToken).Any();
        }

        public async Task<bool> AddAccount(Account account)
        {
            _dbContext.Account.Add(account);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<Account> GetAccountByVerificationToken(string token)
        {

            Account account = null;
            account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((a) => a.VerificationToken == token);

            return account;
        }

        public async Task<Account> GetAccountByResetPasswordToken(string token)
        {

            Account account = null;
            account = await _dbContext.Account.AsNoTracking().SingleOrDefaultAsync((a) => a.PasswordResetToken == token);

            return account;
        }

        public async Task<bool> UpdateAccount(Account account)
        {
            _dbContext.Account.Update(account);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<List<AccountDto>> GetAccountDtos(List<EmailAccounts> emails)
        {
            List<AccountDto> accounts = new();
            foreach (var email in emails)
            {
                var account = await ConvertToDto(email.Email);
                if (account.Email == email.Email) accounts.Add(account);
            }

            return accounts;
        }

        public async Task<AccountDto> GetAccountDto(string email)
        {
            AccountDto account;

            account = await ConvertToDto(email);

            return account;
        }

        public async Task<PagedList<AccountDto>> GetAccountsInWorkspace(Guid workspaceId,
            string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<AccountWorkspace> query = _dbContext.AccountWorkspace.AsNoTracking().Where((a) => a.WorkspaceId == workspaceId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Join(_dbContext.Account,
                accountWorkspace => accountWorkspace.AccountId,
                account => account.AccountId,
                (accountWorkspace, account) => new { AccountWorkspace = accountWorkspace, Account = account })
                .Where(joinResult => joinResult.Account.FirstName.Contains(searchTerm) 
                || joinResult.Account.LastName.Contains(searchTerm) 
                || joinResult.Account.Email.Contains(searchTerm))
                .Select(joinResult => joinResult.AccountWorkspace);

            if (sortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortColumn));
            }

            List<AccountDto> accounts = new();
            var accountWorkspaces = PagedList<AccountWorkspace>.Create(query, page, pageSize);
            
            try
            {
                foreach (var acc in accountWorkspaces.Items)
                {
                    Account account = await GetAccountById(acc.AccountId);
                    accounts.Add(await ConvertToDto(account.Email));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return new(accounts, accountWorkspaces.Page, accountWorkspaces.PageSize, accountWorkspaces.TotalCount);

        }

        public async Task<List<AccountDto>> GetAccountsInCompanyExcludingInWorkspace(Guid companyId, Guid workspaceId)
        {
            List<AccountDto> accounts = new();

            try
            {
                var accountsByCompanyExcludingInWorkspace = _dbContext.Account.AsNoTracking()
                    .Where(acc => acc.CompanyId == companyId && !_dbContext.AccountWorkspace
                    .Any(accWorkspace => accWorkspace.AccountId == acc.AccountId && accWorkspace.WorkspaceId == workspaceId))
                    .Distinct()
                    .ToList();

                if (accountsByCompanyExcludingInWorkspace == null || accountsByCompanyExcludingInWorkspace.Count == 0) return accounts;

                foreach (var acc in accountsByCompanyExcludingInWorkspace)
                {
                    Account account = await GetAccountById(acc.AccountId);
                    accounts.Add(await ConvertToDto(account.Email));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return accounts;
        }

        private async Task<AccountDto> ConvertToDto(string email)
        {
            var account = await GetActiveAccountByEmailAsync(email);
            if (account == null) return new AccountDto();

            var company = await _dbContext.Company.AsNoTracking().SingleOrDefaultAsync((a) => a.CompanyId == account.CompanyId && a.CompanyState == Core.Enums.State.Active);

            AccountDto dto = new()
            {
                AccountId = account.AccountId,
                Email = account.Email,
                CompanyId = account.CompanyId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                MobileNumber = account.MobileNumber,
                OriginCountry = account.OriginCountry,
                CompanyName = company?.Name,
                Premium = account.Premium,
                PremiumExpirationDate = account.PremiumExpirationDate,
                Culture = account.Culture,
                TimeZone = account.TimeZone
            };

            return dto;
        }

        public async Task<AccountDto> EditAccount(AccountDto accountDto)
        {
            var account = await GetAccountById(accountDto.AccountId);
            if (account == null) return new AccountDto();

            if (account != null)
            {
                account.Email = accountDto.Email;
                account.FirstName = accountDto.FirstName;
                account.LastName = accountDto.LastName;
                account.MobileNumber = accountDto.MobileNumber;
                account.Premium = accountDto.Premium;
                account.PremiumExpirationDate = accountDto.PremiumExpirationDate;
                account.Culture = accountDto.Culture;
                account.TimeZone = accountDto.TimeZone;
                account.UpdatedAt = DateTime.Now;

                await UpdateAccount(account);
            }

            return await ConvertToDto(accountDto.Email);
        }

        private static Expression<Func<AccountWorkspace, object>> GetSortProperty(string? sortColumn)
        {
            return (sortColumn?.ToLower()) switch
            {
                "role" => acc => acc.RoleId,
                "accountid" => acc => acc.AccountId,
                _ => work => work.AccountWorkspaceId
            };
        }
    }
}
