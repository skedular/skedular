using Microsoft.AspNetCore.Mvc.Filters;

namespace Enterprise.Shared.Infrastructure.Filters;

public class GlobalHttpExceptionHandler : IGlobalHttpExceptionHandler
{
    public bool HandleException(ExceptionContext context) => false;
}
