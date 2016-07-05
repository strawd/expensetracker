// Copyright 2016 David Straw

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseTracker
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"LoggingHttpHandler: Request for {request.RequestUri}");

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                Trace.TraceInformation($"LoggingHttpHandler: Responding with {(int)response.StatusCode} ({response.ReasonPhrase}) to {request.RequestUri}");

                return response;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"LoggingHttpHandler: Exception thrown during request for {request.RequestUri}:\n{ex.ToString()}");

                throw;
            }
        }
    }
}