// Copyright 2016 David Straw

using System;
using Microsoft.Azure.Mobile.Server;

namespace ExpenseTracker.DataObjects
{
    public class ExpensePeriod : EntityData
    {
        public decimal AmountAvailable { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public string AccountId { get; set; }
    }
}