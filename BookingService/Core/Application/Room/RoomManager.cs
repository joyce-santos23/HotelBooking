using Application.Dtos;
using Application.Ports;
using Application.Room.Requests;
using Application.Room.Responses;
using Domain.Entities;
using Domain.Ports;
using Domain.Room.Exceptions;

namespace Application.Room
{
    public class RoomManager : IRoomManager
    {
        private IRoomRepository _roomRepository; 

        public RoomManager(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
        {
            try
            {
                var room = RoomDto.MapToEntity(request.RoomData);
                await room.Save(_roomRepository);
                request.RoomData.Id = room.Id;

                return new RoomResponse
                {
                    RoomData = request.RoomData,
                    Success = true,
                    Message = "Sala criada com sucesso!"
                };
            }
            catch (InvalidRoomNameException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.MISSING_REQUIRED_INFORMATION,
                    Message = "Missing passed name information"
                };
            }
            catch (InvalidRoomPriceException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.INVALID_PRICE,
                    Message = "The passed price cannot be <= 0"
                };
            }
            catch (InvalidRoomLevelException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.MISSING_REQUIRED_INFORMATION,
                    Message = "Missing passed level information"
                };
            }
            catch (RoomNotAvailableException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.ROOM_NOT_AVAILABLE,
                    Message = "The passed Room is not available"
                };
            }
            catch (Exception)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.COULD_NOT_STORE_DATA,
                    Message = "There was an error creating a room"
                };
            }
        }

        public async Task<RoomResponse> DeleteRoom(int roomId)
        {
            try
            {
                var room = await _roomRepository.Get(roomId);
                if (room == null)
                {
                    return new RoomResponse
                    {
                        Success = false,
                        Message = "Sala não encontrada."
                    };
                }

                await _roomRepository.Delete(roomId);

                return new RoomResponse
                {
                    Success = true,
                    Message = "Sala deletada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new RoomResponse
                {
                    Success = false,
                    Message = "Erro ao deletar sala: " + ex.Message
                };
            }
        }


        public async Task<RoomResponse> GetRoom(int roomId)
        {
            var room = await _roomRepository.Get(roomId);

            if (room == null)
            {
                return new RoomResponse
                {
                    Success = false,
                    Message = "Sala não encontrada."
                };
            }

            return new RoomResponse
            {
                RoomData = RoomDto.MapToDto(room),
                Success = true,
                Message = "Sala encontrada com sucesso!"
            };
            
        }

        public async Task<IEnumerable<RoomResponse>> GetAllRooms()
        {
            var room = await _roomRepository.GetAll();
            var responseList = new List<RoomResponse>();

            foreach (var rooms in room)
            {
                responseList.Add(new RoomResponse
                {
                    RoomData = RoomDto.MapToDto(rooms),
                    Success = true
                });
            }

            return responseList;
        }


       

    }
}
