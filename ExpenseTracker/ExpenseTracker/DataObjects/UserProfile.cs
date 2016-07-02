// Copyright 2016 David Straw

using Microsoft.Azure.Mobile.Server;

namespace ExpenseTracker.DataObjects
{
    public class UserProfile : EntityData
    {
        public string UserId { get; set; }

        public string GivenName { get; set; }
    }
}