using Microsoft.AspNetCore.Mvc;

namespace ClientePersonaApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ManejoExcepcionesAttribute : TypeFilterAttribute
{
    public ManejoExcepcionesAttribute() : base(typeof(ManejoExcepcionesFilter))
    {
    }
}
