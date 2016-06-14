// Copyright 2016 David Straw

using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ExpenseTracker.Startup))]

namespace ExpenseTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}