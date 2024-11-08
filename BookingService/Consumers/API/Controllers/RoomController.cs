using Application.Dtos;
using Application.Room.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;
using Application.Room.Responses;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomManager _roomManager;

        public RoomController(
            ILogger<RoomController> logger,
            IRoomManager roomManager)
        {
            _logger = logger;
            _roomManager = roomManager;
        }

        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<RoomDto>> Post(RoomDto room)
        {
            var request = new CreateRoomRequest
            {
                RoomData = room,
            };

            var res = await _roomManager.CreateRoom(request);

            if (res.Success)
                return CreatedAtAction(nameof(Get), new { id = res.RoomData.Id }, res.RoomData);

            _logger.LogError("Failed to create room: {ErrorCode} - {Message}", res.ErrorCode, res.Message);

            return res.ErrorCode switch
            {
                // Mapear os códigos de erro específicos com suas respectivas mensagens
                Application.ErrorCode.MISSING_REQUIRED_INFORMATION => BadRequest(new { Message = res.Message, ErrorCode = res.ErrorCode }),
                Application.ErrorCode.INVALID_PRICE => BadRequest(new { Message = res.Message, ErrorCode = res.ErrorCode }),
                Application.ErrorCode.ROOM_NOT_AVAILABLE => BadRequest(new { Message = res.Message, ErrorCode = res.ErrorCode }),

                // Caso o código de erro seja desconhecido
                _ => BadRequest(new { Message = "An error occurred while creating the room." })
            };
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> Get(int id)
        {
            var res = await _roomManager.GetRoom(id);

            if (res.Success)
                return Ok(res.RoomData);

            return NotFound(new { message = "Room not found" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomManager.GetAllRooms();

            if (!rooms.Any())
            {
                return NotFound(new RoomResponse
                {
                    Success = false,
                    Message = "No rooms records were found"
                });
                
            }
            var roomDtos = rooms.Select(g => g.RoomData).ToList();
            return Ok(roomDtos);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _roomManager.DeleteRoom(id);

            return NotFound(new RoomResponse
            {
                Success = false,
                ErrorCode = Application.ErrorCode.NOT_FOUND,
                Message = "No guest record was found with the given id"
            });

            
        }



    }
}
