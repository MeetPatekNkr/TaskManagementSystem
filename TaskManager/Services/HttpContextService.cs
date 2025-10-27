using Microsoft.AspNetCore.Http;

namespace TaskManager.Services
{
    public interface IHttpContextService
    {
        HttpContext? Current { get; }
    }

    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext? Current => _httpContextAccessor.HttpContext;
    }
}