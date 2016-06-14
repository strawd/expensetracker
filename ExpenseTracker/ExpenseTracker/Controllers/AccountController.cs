// Copyright 2016 David Straw

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using ExpenseTracker.DataObjects;
using ExpenseTracker.Models;
using Microsoft.Azure.Mobile.Server;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class AccountController : TableController<Account>
    {
        private MobileServiceContext _context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Account>(_context, Request);
        }

        // GET tables/Account
        public IQueryable<Account> GetAllAccounts()
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            return Query().Where(account => userAccounts.Contains(account.Id));
        }

        // GET tables/Account/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Account> GetAccount(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = Query().Where(account => userAccounts.Contains(account.Id) && account.Id == id);
            return SingleResult.Create(query);
        }

        // PATCH tables/Account/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Account> PatchAccount(string id, Delta<Account> patch)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = Query().Where(account => userAccounts.Contains(account.Id) && account.Id == id);
            if (!query.Any())
                NotFound();

            return UpdateAsync(id, patch);
        }

        // POST tables/Account
        public async Task<IHttpActionResult> PostAccount(Account item)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = Query().Where(account => userAccounts.Contains(account.Id));
            if (query.Any())
                Conflict();

            Account current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Account/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAccount(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = Query().Where(account => userAccounts.Contains(account.Id) && account.Id == id);
            if (!query.Any())
                NotFound();

            return DeleteAsync(id);
        }
    }
}