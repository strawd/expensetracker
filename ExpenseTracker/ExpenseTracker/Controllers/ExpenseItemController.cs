﻿// Copyright 2016 David Straw

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
    [Authorize]
    public class ExpenseItemController : TableController<ExpenseItem>
    {
        private MobileServiceContext _context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<ExpenseItem>(_context, Request);
        }

        // GET tables/ExpenseItem
        public IQueryable<ExpenseItem> GetAllExpenseItem()
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            return Query().Where(expenseItem => userAccounts.Contains(expenseItem.AccountId));
        }

        // GET tables/ExpenseItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ExpenseItem> GetExpenseItem(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpenseItems.Where(expenseItem => userAccounts.Contains(expenseItem.AccountId) && expenseItem.Id == id);
            return SingleResult.Create(query);
        }

        // PATCH tables/ExpenseItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ExpenseItem> PatchExpenseItem(string id, Delta<ExpenseItem> patch)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpenseItems.Where(expenseItem => userAccounts.Contains(expenseItem.AccountId) && expenseItem.Id == id);
            if (!query.Any())
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var changedProperties = patch.GetChangedPropertyNames().ToList();
            var filteredDelta = new Delta<ExpenseItem>();

            if (changedProperties.Contains(nameof(ExpenseItem.Amount), StringComparer.OrdinalIgnoreCase))
            {
                filteredDelta.TrySetPropertyValue(nameof(ExpenseItem.Amount), patch.GetEntity().Amount);
            }
            if (changedProperties.Contains(nameof(ExpenseItem.Date), StringComparer.OrdinalIgnoreCase))
            {
                filteredDelta.TrySetPropertyValue(nameof(ExpenseItem.Date), patch.GetEntity().Date);
            }
            if (changedProperties.Contains(nameof(ExpenseItem.Description), StringComparer.OrdinalIgnoreCase))
            {
                filteredDelta.TrySetPropertyValue(nameof(ExpenseItem.Description), patch.GetEntity().Description);
            }

            return UpdateAsync(id, filteredDelta);
        }

        // POST tables/ExpenseItem
        public async Task<IHttpActionResult> PostExpenseItem(ExpenseItem item)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            item.AccountId = userAccounts.FirstOrDefault();

            if (item.AccountId == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Account not found"));

            item.CreatedBy = userSid;

            ExpenseItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ExpenseItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteExpenseItem(string id)
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var query = _context.ExpenseItems.Where(expenseItem => userAccounts.Contains(expenseItem.AccountId) && expenseItem.Id == id);
            if (!query.Any())
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return DeleteAsync(id);
        }
    }
}
