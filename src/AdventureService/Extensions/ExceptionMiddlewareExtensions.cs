using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using AdventureService.Infraestructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AdventureService.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature.Error is ValidationException ||
                        contextFeature.Error is DbUpdateException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }

                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetail()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.InnerException != null ?
                                contextFeature.Error.Message + Environment.NewLine + contextFeature.Error.InnerException.Message
                                : contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}