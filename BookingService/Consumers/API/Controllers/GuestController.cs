using Application.Dtos;
using Application.Guest.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("controller")]
    public class GuestController : ControllerBase
    {
        private readonly ILogger<GuestController> _logger;
        private readonly IGuestManager _guestManager;
        

        public GuestController(
            ILogger<GuestController> logger,
            IGuestManager guestManager)
        {
            _logger = logger;
            _guestManager = guestManager;
        }

        [HttpPost]
        public async Task<ActionResult<GuestDto>> Post(GuestDto guest)
        {
            var resquest = new CreateGuestRequest
            {
                Data = guest,
            };

            var res = await _guestManager.CreateGuest(resquest);

            if (res.Success) return Created("", res.Data);

            if (res.ErrorCode == Application.ErrorCode.NOT_FOUND)
            {
                return BadRequest(res);
            }

            _logger.LogError("Response with unkwn ErrorCode Returned", res);
            return BadRequest();



        }
    }
}
