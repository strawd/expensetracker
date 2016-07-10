// Copyright 2016 David Straw

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using ExpenseTracker.DataObjects;
using ExpenseTracker.Models;
using Microsoft.Azure.Mobile.Server;

namespace ExpenseTracker.Controllers
{
    public class ExpensePeriodController : TableController<ExpensePeriod>
    {
        private MobileServiceContext _context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<ExpensePeriod>(_context, Request);
        }

        // GET tables/ExpensePeriod
        public IQueryable<ExpensePeriod> GetAllExpensePeriod()
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            return Query().Where(expensePeriod => userAccounts.Contains(expensePeriod.AccountId));
        }

        // GET tables/ExpensePeriod/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ExpensePeriod> GetExpensePeriod(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpensePeriods.Where(expensePeriod => userAccounts.Contains(expensePeriod.AccountId) && expensePeriod.Id == id);
            return SingleResult.Create(query);
        }

        // PATCH tables/ExpensePeriod/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ExpensePeriod> PatchExpensePeriod(string id, Delta<ExpensePeriod> patch)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpensePeriods.Where(expensePeriod => userAccounts.Contains(expensePeriod.AccountId) && expensePeriod.Id == id);
            if (!query.Any())
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var changedProperties = patch.GetChangedPropertyNames().ToList();
            var filteredDelta = new Delta<ExpensePeriod>();

            if (changedProperties.Contains(nameof(ExpensePeriod.AmountAvailable), StringComparer.OrdinalIgnoreCase))
            {
                filteredDelta.TrySetPropertyValue(nameof(ExpensePeriod.AmountAvailable), patch.GetEntity().AmountAvailable);
            }
            if (changedProperties.Contains(nameof(ExpensePeriod.StartDate), StringComparer.OrdinalIgnoreCase))
            {
                filteredDelta.TrySetPropertyValue(nameof(ExpensePeriod.StartDate), patch.GetEntity().StartDate);
            }

            return UpdateAsync(id, filteredDelta);
        }

        // POST tables/ExpensePeriod
        public async Task<IHttpActionResult> PostExpensePeriod(ExpensePeriod item)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            item.AccountId = userAccounts.FirstOrDefault();

            if (item.AccountId == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Account not found"));

            ExpensePeriod current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ExpensePeriod/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteExpensePeriod(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpensePeriods.Where(expensePeriod => userAccounts.Contains(expensePeriod.AccountId) && expensePeriod.Id == id);
            if (!query.Any())
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return DeleteAsync(id);
        }
    }
}