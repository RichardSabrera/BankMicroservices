using CuentaMovimientoApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CuentaMovimientoApi.Filters;

public class ManejoExcepcionesFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if (exception is SaldoNoDisponibleException || exception.Message.Contains("Saldo no disponible"))
        {
            context.Result = new BadRequestObjectResult(new
            {
                mensaje = "Saldo no disponible"
            });
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new BadRequestObjectResult(new
            {
                mensaje = exception.Message
            });
            context.ExceptionHandled = true;
        }
    }
}
