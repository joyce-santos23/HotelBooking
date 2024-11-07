﻿using Application.Dtos;
using Application.Booking.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;
using Application.Responses;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingManager _bookingManager;

        public BookingController(
            ILogger<BookingController> logger,
            IBookingManager bookingManager)
        {
            _logger = logger;
            _bookingManager = bookingManager;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> Post(BookingDto booking)
        {
            var request = new CreateBookingRequest
            {
                BookingData = booking,
            };

            var res = await _bookingManager.CreateBooking(request);

            if (res.Success)
                return CreatedAtAction(nameof(Get), new { id = res.BookingData.Id }, res.BookingData);

            _logger.LogError("Response with unknown error code", res);
            return BadRequest("An error occurred while creating the booking.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> Get(int id)
        {
            var res = await _bookingManager.GetBooking(id);

            if (res.Success)
                return Ok(res.BookingData);

            return NotFound(new { message = "Booking not found" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var bookings = await _bookingManager.GetAllBookings();

            if (!bookings.Any())
            {
                return NotFound(new BookingResponse
                {
                    Success = false,
                    ErrorCode = Application.ErrorCode.NOT_FOUND,
                    Message = "No guest records were found"
                });
            }

            var bookingDtos = bookings.Select(g => g.BookingData).ToList();
            return Ok(bookingDtos);
        }


        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> Delete(int bookingId)
        {
            var success = await _bookingManager.DeleteBooking(bookingId);

            if (success) return NoContent();

            return NotFound(new BookingResponse
            {
                Success = false,
                ErrorCode = Application.ErrorCode.NOT_FOUND,
                Message = "No guest record was found with the given id"
            });
        }


    }
}
