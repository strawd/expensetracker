// Copyright 2016 David Straw

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ExpensePeriodSummary GetCurrentExpensePeriodSummary()
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
                return new ExpensePeriodSummary
                {
                    AmountAvailable = 0,
                    AmountRemaining = 0,
                    ExpensesCount = 0,
                    StartDate = DateTimeOffset.Now
                };

            var nextExpensePeriod = _context.ExpensePeriods
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.StartDate > DateTimeOffset.Now)
                .OrderBy(x => x.StartDate)
                .FirstOrDefault();

            var expensesQuery = _context.ExpenseItems
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.Date >= currentExpensePeriod.StartDate);

            if (nextExpensePeriod != null)
            {
                expensesQuery = expensesQuery.Where(x => x.Date < nextExpensePeriod.StartDate);
            }

            var expenses = expensesQuery.ToList();

            return new ExpensePeriodSummary
            {
                AmountAvailable = currentExpensePeriod.AmountAvailable,
                AmountRemaining = currentExpensePeriod.AmountAvailable - expenses.Aggregate(0m, (sum, item) => sum + item.Amount),
                ExpensesCount = expenses.Count,
                StartDate = currentExpensePeriod.StartDate
            };
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

            var expensePeriods = _context.ExpensePeriods
                .Where(x => userAccounts.Contains(x.AccountId))
                .Where(x => x.StartDate <= DateTimeOffset.Now)
                .Take(10)
                .OrderByDescending(x => x.StartDate)
                .ToList();

            var summaries = new List<ExpensePeriodSummary>();

            for (int i = 0; i < expensePeriods.Count; i++)
            {
                var expensePeriod = expensePeriods[i];

                var expensesQuery = _context.ExpenseItems
                    .Where(x => userAccounts.Contains(x.AccountId))
                    .Where(x => x.Date >= expensePeriod.StartDate);

                if (i > 0)
                {
                    var followingStartDate = expensePeriods[i - 1].StartDate;
                    expensesQuery = expensesQuery.Where(x => x.Date < followingStartDate);
                }

                var expenses = expensesQuery.ToList();

                summaries.Add(new ExpensePeriodSummary
                {
                    AmountAvailable = expensePeriod.AmountAvailable,
                    AmountRemaining = expensePeriod.AmountAvailable - expenses.Aggregate(0m, (sum, item) => sum + item.Amount),
                    ExpensesCount = expenses.Count,
                    StartDate = expensePeriod.StartDate
                });
            }

            return summaries;
        }
    }
}