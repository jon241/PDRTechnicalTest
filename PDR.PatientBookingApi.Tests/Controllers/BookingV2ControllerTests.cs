using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBookingApi.Controllers;
using System;
using System.Net;

namespace PDR.PatientBookingApi.Tests.Controllers
{
    [TestFixture]
    public class BookingV2ControllerTests
    {
        private Mock<IBookingService> _serviceMock;
        private BookingV2Controller _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IBookingService>();
            _controller = new BookingV2Controller(_serviceMock.Object);
        }

        [Test]
        public void Constructor_WithNullService_ThrowsArgumentNullException()
        {
            TestDelegate test = () => new BookingV2Controller(null);

            Exception actual = Assert.Catch<ArgumentNullException>(test, "Exception");
            Assert.AreEqual("Value cannot be null. (Parameter 'bookingService')", actual.Message, "Message");
        }

        [Test]
        public void AddBooking_ValidationFails_ReturnStatusCodeBadRequest()
        {
            var request = new AddBookingRequest();
            _serviceMock.Setup(p => p.AddBooking(request)).Throws<ArgumentException>();

            IActionResult response = _controller.AddBooking(request);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as BadRequestObjectResult).StatusCode, "StatusCode");
        }

        [Test]
        public void AddBooking_BookingFails_ReturnStatusCodeInternalServerError()
        {
            var request = new AddBookingRequest();
            _serviceMock.Setup(p => p.AddBooking(request)).Throws<Exception>();
            
            IActionResult response = _controller.AddBooking(request);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, (response as ObjectResult).StatusCode, "StatusCode");
        }

        [Test]
        public void AddBooking_BookingSucceeds_ReturnStatusCodeCreated()
        {
            var request = new AddBookingRequest();
            _serviceMock.Setup(p => p.AddBooking(request));

            IActionResult response = _controller.AddBooking(request);

            Assert.AreEqual((int)HttpStatusCode.Created, (response as StatusCodeResult).StatusCode, "StatusCode");
        }
    }
}