// Copyright 2016 David Straw

using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace ExpenseTracker.Controllers
{
    [MobileAppController]
    public class SimpleController : ApiController
    {
        public object Get()
        {
            return "Hello World";
        }
    }
}