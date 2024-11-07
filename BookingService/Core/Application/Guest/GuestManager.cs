using Application.Dtos;
using Application.Guest.Requests;
using Application.Ports;
using Application.Responses;
using Domain.Guest.Exceptions;
using Domain.Ports;

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

                await guest.Save(_guestRepository);
                request.Data.Id = guest.Id;

                return new GuestResponse
                {
                    Data = request.Data,
                    Success = true,
                };
            }
            catch (InvalidPersonDocumentIdException)
            {
                return new GuestResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.INVALID_PERSON_ID,
                    Message = "The passed ID is not valid"
                };
            }
            catch (MissingRequiredInformation)
            {
                return new GuestResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.MISSING_REQUIRED_INFORMATION,
                    Message = "Missing passed required information"
                };
            }
            catch (InvalidEmailException)
            {
                return new GuestResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.INVALID_EMAIL,
                    Message = "The given email is not valid"
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

        public async Task<GuestResponse> GetGuest(int guestId)
        {
            var guest = await _guestRepository.Get(guestId);

            if (guest == null)
            {
                return new GuestResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.GUEST_NOT_FOUND,
                    Message = "No guest record was found with the given id"
                };
            }

            return new GuestResponse
            {
                Data = GuestDto.MapToDto(guest),
                Success = true,
            };
        }

        public async Task<IEnumerable<GuestResponse>> GetAllGuests()
        {
            var guests = await _guestRepository.GetAll();
            var responseList = new List<GuestResponse>();

            foreach (var guest in guests)
            {
                responseList.Add(new GuestResponse
                {
                    Data = GuestDto.MapToDto(guest),
                    Success = true
                });
            }

            return responseList;
        }

        public async Task<bool> DeleteGuest(int guestId)
        {
            try
            {
                var guest = await _guestRepository.Get(guestId);
                if (guest == null)
                {
                    return false;
                }

                await _guestRepository.Delete(guestId);
                return true;
            }
            catch (Exception)
            {
                return false; 
            }
        }

    }
}
