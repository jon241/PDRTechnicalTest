using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using System;

namespace PDR.PatientBookingApi.Controllers
{
    // Note I have created a new controller separate from the original 
    // BookingController to make change easier.
    // This controller design is more in keeping with my own best practice
    // and what is found in the other controllers.
    [Route("api/[controller]")]
    [ApiController]
    public class BookingV2Controller : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingV2Controller(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost()]
        public IActionResult AddBooking(AddBookingRequest request)
        {
            try
            {
                _bookingService.AddBooking(request);

                return Ok();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

    }
}