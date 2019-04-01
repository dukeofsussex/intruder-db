// Copyright (c) Chris Satchell. All rights reserved.

// Taken from https://stackoverflow.com/documentation/asp.net-core/9510/asp-net-core-log-both-request-and-response-using-middleware#t=20170708105619966416
namespace API.Middleware
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using NLog;

    public class LoggerMiddleware
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly RequestDelegate next;

        public LoggerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (MemoryStream requestBodyStream = new MemoryStream())
            {
                using (MemoryStream responseBodyStream = new MemoryStream())
                {
                    Stopwatch watch = new Stopwatch();
                    DateTime requestReceived = DateTime.Now;

                    try
                    {
                        watch.Start();
                        await this.next(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Caught an unhandled error: {ex.Message} : {ex.StackTrace}");
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        byte[] data = System.Text.Encoding.UTF8.GetBytes($"{{\"code\": {(int)HttpStatusCode.InternalServerError}, \"message\": \"Welp, either you or me broke something :( Please try again later.\"}}");
                        context.Response.Body.Write(data, 0, data.Length);
                    }
                    finally
                    {
                        watch.Stop();
                        logger.Info($"{requestReceived} \"{context.Request.Method} {UriHelper.GetDisplayUrl(context.Request)}\""
                            + $" {context.Response.StatusCode} - {watch.ElapsedMilliseconds}ms - {Math.Floor((decimal)GC.GetTotalMemory(false) / 1000)}KB");
                    }
                }
            }
        }
    }
}