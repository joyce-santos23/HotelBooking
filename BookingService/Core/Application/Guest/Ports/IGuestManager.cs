﻿using Application.Dtos;
using Application.Guest.Requests;
using Application.Responses;

namespace Application.Ports
{
    public interface IGuestManager
    {
        Task<GuestResponse> CreateGuest(CreateGuestRequest request);
        Task<GuestResponse> GetGuest(int guestId);
        Task<IEnumerable<GuestResponse>> GetAllGuests();
        Task<GuestResponse> DeleteGuest(int guestId);
 
    }
}
