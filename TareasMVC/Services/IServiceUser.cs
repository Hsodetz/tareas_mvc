using System;
using System.Security.Claims;

namespace TareasMVC.Services
{
	public interface IServiceUser
	{
		string getUserId();
	}

	public class ServiceUsers : IServiceUser
	{
        private readonly HttpContext httpContext;

        public ServiceUsers(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public string getUserId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                return idClaim.Value;
            }
            else
            {
                throw new Exception("El usuario no esta autenticado!");
            }
        }
    }

	
}

