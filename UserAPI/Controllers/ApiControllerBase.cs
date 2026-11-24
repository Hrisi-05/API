using Microsoft.AspNetCore.Mvc;
using UserAPI.DTOs;

namespace UserAPI.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected IActionResult Fail<T>(ServiceResult<T> result) => result.Error switch
        {
            ServiceError.NotFound => NotFound(new { error = result.Message }),
            ServiceError.Conflict => Conflict(new { error = result.Message }),
            ServiceError.Unauthorized => Unauthorized(new { error = result.Message }),
            _ => BadRequest(new { error = result.Message })
        };
    }
}
