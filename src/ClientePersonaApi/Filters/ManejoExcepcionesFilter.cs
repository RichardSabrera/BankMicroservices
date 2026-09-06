using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ClientePersonaApi.Filters;

public class ManejoExcepcionesFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        string mensajeError = exception.Message;

        if (exception is DbUpdateException dbEx && dbEx.InnerException != null)
        {
            if (dbEx.InnerException.Message.Contains("UNIQUE KEY") || dbEx.InnerException.Message.Contains("duplicate key") || dbEx.InnerException.Message.Contains("uq_"))
            {
                mensajeError = "Ya existe un registro con la misma identificación o código de cliente.";
            }
            else
            {
                mensajeError = dbEx.InnerException.Message;
            }
        }

        context.Result = new BadRequestObjectResult(new
        {
            mensaje = mensajeError
        });
        context.ExceptionHandled = true;
    }
}
