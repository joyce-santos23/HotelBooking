using Application.Dtos;
using Application.Ports;
using Application.Room.Requests;
using Application.Room.Responses;
using Domain.Entities;
using Domain.Ports;
using Domain.Room.Enums;
using Domain.Room.Exceptions;
using Domain.Room.ValueObjects;

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
                    Message = "The room has been created successfully!"
                };
            }
            catch (InvalidRoomNameException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.MISSING_REQUIRED_INFORMATION,
                    Message = "Missing name information"
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
                    Message = "Missing level information"
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
                        ErrorCode = ErrorCode.NOT_FOUND,
                        Message = "Room not found."
                    };
                }

                if (room.Bookings != null && room.Bookings.Any())
                {
                    return new RoomResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.CANNOT_DELETE_ROOM_WITH_BOOKINGS,
                        Message = "Cannot delete room because it has existing bookings."
                    };
                }

                var deleteSuccess = await _roomRepository.Delete(roomId);

                if (deleteSuccess)
                {
                    return new RoomResponse
                    {
                        Success = true,
                        Message = "The room was successfully deleted."
                    };
                }

                return new RoomResponse
                {
                    Success = false,
                    Message = "Error occurred while trying to delete the room."
                };
            }
            catch (Exception ex)
            {
                return new RoomResponse
                {
                    Success = false,
                    Message = "Error deleting a room: " + ex.Message
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

        public async Task<RoomResponse> UpdateRoom(int roomId, UpdateRoomRequest request)
        {
            try
            {
                var room = await _roomRepository.Get(roomId);
                if (room == null)
                {
                    return new RoomResponse
                    {
                        Success = false,
                        Message = "Room not found."
                    };
                }

                room.Name = request.RoomData.Name ?? room.Name;
                room.Price = request.RoomData.PriceValue != 0
                    ? new Price { Value = request.RoomData.PriceValue, Currency = (AcceptedCurrencies)request.RoomData.CurrencyCode }
                    : room.Price;
                room.InMaintenance = request.RoomData.InMaintenance;

                room.Validate();

                await _roomRepository.Update(room);

                return new RoomResponse
                {
                    RoomData = RoomDto.MapToDto(room),
                    Success = true,
                    Message = "Room updated successfully!"
                };
            }
            catch (InvalidRoomNameException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.MISSING_REQUIRED_INFORMATION,
                    Message = "Missing name information"
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
                    Message = "Missing level information"
                };
            }
            catch (Exception ex)
            {
                return new RoomResponse
                {
                    Success = false,
                    Message = "Error updating room: " + ex.Message
                };
            }
        }






    }
}
