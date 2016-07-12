// Copyright 2016 David Straw

using System;
using System.Linq;
using System.Web.Http;
using ExpenseTracker.DataObjects;
using ExpenseTracker.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    [MobileAppController]
    [RoutePrefix("api/Summary")]
    public class SummaryController : ApiController
    {
        readonly MobileServiceContext _context;

        public SummaryController()
        {
            _context = new MobileServiceContext();
        }

        [HttpGet]
        [Route("CurrentExpensePeriod")]
        public CurrentExpensePeriodSummary GetCurrentExpensePeriodSummary()
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId);

            var currentExpensePeriod = _context.ExpensePeriods
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.StartDate <= DateTimeOffset.Now)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

            if (currentExpensePeriod == null)
                return new CurrentExpensePeriodSummary
                {
                    AmountAvailable = 0,
                    AmountRemaining = 0,
                    ExpensesCount = 0,
                    StartDate = DateTimeOffset.Now
                };

            var expenses = _context.ExpenseItems
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.Date >= currentExpensePeriod.StartDate)
                .Where(x => x.Date <= DateTimeOffset.Now)
                .ToList();

            return new CurrentExpensePeriodSummary
            {
                AmountAvailable = currentExpensePeriod.AmountAvailable,
                AmountRemaining = currentExpensePeriod.AmountAvailable - expenses.Aggregate(0m, (sum, item) => sum + item.Amount),
                ExpensesCount = expenses.Count,
                StartDate = currentExpensePeriod.StartDate
            };
        }
    }
}