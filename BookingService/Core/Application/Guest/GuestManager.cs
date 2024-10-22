using Application.Dtos;
using Application.Guest.Requests;
using Application.Ports;
using Application.Responses;
using Domain.Ports;
using System;

namespace Application.Guest
{
    public class GuestManager : IGuestManager
    {
        private IGuestRepository _guestRepository;

        public GuestManager(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }
        public async Task<GuestResponse> CreateGuest(CreateGuestRequest request)
        {
            try
            {
                var guest = GuestDto.MapToEntity(request.Data);

                request.Data.Id = await _guestRepository.Create(guest);

                return new GuestResponse
                {
                    Data = request.Data,
                    Success = true,
                };
            }
            catch (Exception)
            {
                return new GuestResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.COULD_NOT_STORE_DATA,
                    Message = "There was an error when saving to DB"
                };
            }
        }

        public Task<GuestResponse> CreateGuest()
        {
            throw new NotImplementedException();
        }

        public Task<GuestResponse> GetGuest(int guestId)
        {
            throw new NotImplementedException();
        }
    }
}
