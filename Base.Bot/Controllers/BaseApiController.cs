using AutoMapper;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Base.Bot.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMapper Mapper;
        protected BaseApiController()
        {
        }

        protected BaseApiController(IMapper mapper)
        {
            Mapper = mapper;
        }

        protected IActionResult Created(object value)
        {
            return StatusCode(201, value);
        }
    }
}
