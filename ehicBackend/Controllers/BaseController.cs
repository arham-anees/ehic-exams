using Microsoft.AspNetCore.Mvc;
using EhicBackend.Services;
namespace EhicBackend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IAuthService _authService;

        public BaseController(IAuthService authService)
        {
            _authService = authService;
        }
    }
}