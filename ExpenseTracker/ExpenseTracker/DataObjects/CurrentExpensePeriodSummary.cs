﻿// Copyright 2016 David Straw

using System;

namespace ExpenseTracker.DataObjects
{
    public class CurrentExpensePeriodSummary
    {
        public decimal AmountAvailable { get; set; }

        public decimal AmountRemaining { get; set; }

        public int ExpensesCount { get; set; }

        public DateTimeOffset StartDate { get; set; }
    }
}