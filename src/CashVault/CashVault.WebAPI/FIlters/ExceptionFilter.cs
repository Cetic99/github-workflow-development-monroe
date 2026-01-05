using System;
using System.Text.Json;
using CashVault.Domain.Common.Exceptions;
using CashVault.WebAPI.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashVault.WebAPI.FIlters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private ExceptionContext _context;

    public override void OnException(ExceptionContext context)
    {

        base.OnException(context);
        _context = context;
        HandleException();
    }

    private void HandleException()
    {
        var exception = _context.Exception;


        if (exception is BaseException)
        {
            Problem(title: "Base exception", detail: exception.Message, statusCode: 400);
        }
        else if (exception is ValidationException)
        {
            Problem(title: "Validation error", detail: JsonSerializer.SerializeToDocument(((ValidationException)exception).Errors).ToJsonString(), statusCode: 400);
        }
        else if (exception is UnauthorizedAccessException)
        {
            Problem(title: "Unauthorized", detail: "Unauthorized access", statusCode: 401);
        }
        else if (exception is ArgumentException)
        {
            Problem(title: "Bad request", detail: $"ArgumentException: {exception.Message}", statusCode: 400);
        }
        else if (exception is KeyNotFoundException)
        {
            Problem(title: "Bad request", detail: "Key not found", statusCode: 400);
        }
        else if (exception is InvalidOperationException)
        {
            Problem(title: "Bad request", detail: $"InvalidOperationException: {exception.Message}", statusCode: 400);
        }
        else
        {
            Problem(title: "Internal Server Error", detail: "Unhandled exception occurred", statusCode: 500);
        }
    }

    private void Problem(string title, string detail, int statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
        };

        _context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
        };

        _context.ExceptionHandled = true;
    }
}
