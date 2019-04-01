// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggerMiddleware>();
        }
    }
}
