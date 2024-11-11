using Application.Responses;
using Application.Room.Requests;
using Application.Room.Responses;

namespace Application.Ports
{
    public interface IRoomManager
    {
        Task<RoomResponse> CreateRoom(CreateRoomRequest request);
        Task<RoomResponse> GetRoom(int roomId);
        Task<RoomResponse> DeleteRoom(int roomId);
        Task<IEnumerable<RoomResponse>> GetAllRooms();
        Task<RoomResponse> UpdateRoom(int roomId, UpdateRoomRequest request);
    }
}
