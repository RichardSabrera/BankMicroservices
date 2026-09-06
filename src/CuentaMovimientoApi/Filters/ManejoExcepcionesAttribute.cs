using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ManejoExcepcionesAttribute : TypeFilterAttribute
{
    public ManejoExcepcionesAttribute() : base(typeof(ManejoExcepcionesFilter))
    {
    }
}
