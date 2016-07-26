// Copyright 2016 David Straw

using System;
using System.Collections.Generic;
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
        [Route("ExpensePeriods")]
        public List<ExpensePeriodSummary> GetExpensePeriodSummaries()
        {
            var userSid = this.GetCurrentUserSid();
            var userAccounts = _context.AccountUsers
                .Where(accountUser => accountUser.UserId == userSid)
                .Select(accountUser => accountUser.AccountId)
                .ToList();

            var now = DateTimeOffset.Now;

            var expensePeriods = _context.ExpensePeriods
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.StartDate <= now)
                .Take(10)
                .OrderByDescending(x => x.StartDate)
                .ToList();

            var nextExpensePeriod = _context.ExpensePeriods
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.StartDate > now)
                .OrderBy(x => x.StartDate)
                .FirstOrDefault();

            var summaries = new List<ExpensePeriodSummary>();

            for (int i = 0; i < expensePeriods.Count; i++)
            {
                var expensePeriod = expensePeriods[i];

                var expensesQuery = _context.ExpenseItems
                    .Where(x => userAccounts.Contains(x.AccountId))
                    .Where(x => x.Date >= expensePeriod.StartDate);

                var endDate = DateTimeOffset.MaxValue;

                if (i == 0)
                {
                    if (nextExpensePeriod != null)
                    {
                        expensesQuery = expensesQuery.Where(x => x.Date < nextExpensePeriod.StartDate);
                        endDate = nextExpensePeriod.StartDate - TimeSpan.FromDays(1);
                    }
                }
                else
                {
                    var followingStartDate = expensePeriods[i - 1].StartDate;
                    expensesQuery = expensesQuery.Where(x => x.Date < followingStartDate);
                    endDate = followingStartDate - TimeSpan.FromDays(1);
                }

                var expenses = expensesQuery.ToList();

                summaries.Add(new ExpensePeriodSummary
                {
                    AmountAvailable = expensePeriod.AmountAvailable,
                    AmountRemaining = expensePeriod.AmountAvailable - expenses.Aggregate(0m, (sum, item) => sum + item.Amount),
                    ExpensesCount = expenses.Count,
                    StartDate = expensePeriod.StartDate,
                    EndDate = endDate
                });
            }

            return summaries;
        }
    }
}