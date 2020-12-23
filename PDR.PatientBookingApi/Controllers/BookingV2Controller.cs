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
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    public class BookingV2Controller : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingV2Controller(IBookingService bookingService)
        {
            if (bookingService == null)
                throw new ArgumentNullException(nameof(bookingService));

            _bookingService = bookingService;
        }

        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult AddBooking([FromBody]AddBookingRequest request)
        {
            try
            {
                _bookingService.AddBooking(request);

                return StatusCode(201);
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