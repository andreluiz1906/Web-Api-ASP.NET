
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProdutoCatalogo.Shared.Messages;

namespace ProdutoCatalogo.Infra.Configurations.Headers;

public class HeaderValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.ContainsKey("User-Agent"))
        {
            context.Result = new BadRequestObjectResult(ValidationMessages.Header.UserAgent.Missing);
            return;
        }

        // Validação do x-request-timestamp
        if (context.ActionArguments.ContainsKey("headerTimestamp"))
        {
            if (headers.ContainsKey("x-request-timestamp"))
            {
                string valueHeader = headers["x-request-timestamp"].FirstOrDefault() ?? string.Empty;

                if (!DateTime.TryParse(valueHeader, out DateTime timestampHeader))
                {
                    context.Result = new BadRequestObjectResult(ValidationMessages.Header.RequestTimestamp.Invalid);
                    return;
                }

                var now = DateTime.UtcNow;
                var difference = Math.Abs((now - timestampHeader).TotalMinutes);
                if (difference > 1)
                {
                    context.Result = new BadRequestObjectResult(ValidationMessages.Header.RequestTimestamp.Expired);
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(ValidationMessages.Header.RequestTimestamp.Missing);
                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Este método pode ser deixado em branco ou usado para processamento adicional após a execução da ação
    }
}

