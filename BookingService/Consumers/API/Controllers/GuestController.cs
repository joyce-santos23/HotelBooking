using Application.Dtos;
using Application.Guest.Requests;
using Application.Ports;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("guests")]
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
            var request = new CreateGuestRequest
            {
                Data = guest,
            };

            var res = await _guestManager.CreateGuest(request);

            if (res.Success) return Created("", res.Data);

            if (res.ErrorCode == Application.ErrorCode.NOT_FOUND)
            {
                return BadRequest(res);
            }
            else if (res.ErrorCode == Application.ErrorCode.INVALID_PERSON_ID)
            {
                return BadRequest(res);
            }
            else if (res.ErrorCode == Application.ErrorCode.MISSING_REQUIRED_INFORMATION)
            {
                return BadRequest(res);
            }
            else if (res.ErrorCode == Application.ErrorCode.INVALID_EMAIL)
            {
                return BadRequest(res);
            }
            else if (res.ErrorCode == Application.ErrorCode.COULD_NOT_STORE_DATA)
            {
                return BadRequest(res);
            }

            _logger.LogError("Response with unknown ErrorCode returned", res);
            return BadRequest();
        }

        [HttpGet("{guestId}")]
        public async Task<ActionResult<GuestDto>> Get(int guestId)
        {
            var res = await _guestManager.GetGuest(guestId);

            if (res.Success) return Ok(res.Data);

            return NotFound(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestDto>>> GetAll()
        {
            var guests = await _guestManager.GetAllGuests();

            if (!guests.Any())
            {
                return NotFound(new GuestResponse
                {
                    Success = false,
                    ErrorCode = Application.ErrorCode.NOT_FOUND,
                    Message = "No guest records were found"
                });
            }

            var guestDtos = guests.Select(g => g.Data).ToList();
            return Ok(guestDtos);
        }

        [HttpDelete("{guestId}")]
        public async Task<IActionResult> Delete(int guestId)
        {
            var res = await _guestManager.DeleteGuest(guestId);

            if (res.Success) return NoContent();

            return res.ErrorCode switch
            {
                Application.ErrorCode.NOT_FOUND => NotFound(new { Message = res.Message, ErrorCode = res.ErrorCode }),
                Application.ErrorCode.CANNOT_DELETE_GUEST_WITH_BOOKINGS => BadRequest(new { Message = res.Message, ErrorCode = res.ErrorCode }),
                _ => BadRequest(new { Message = "An error occurred while deleting the guest.", ErrorCode = res.ErrorCode })
            };
        }



    }
}
