using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    [TestFixture]
    public class BookingServiceTests
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;

        private PatientBookingContext _context;
        private Mock<IAddBookingRequestValidator> _validator;

        private BookingService _bookingService;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _validator = _mockRepository.Create<IAddBookingRequestValidator>();
            
            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _bookingService = new BookingService(
                _context,
                _validator.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        private void SetupMockDefaults()
        {
            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddBookingRequest>()))
                .Returns(new PdrValidationResult(true));
            _context.Patient.Add(new Patient() { Id = 123 });
            _context.Patient.Add(new Patient() { Id = 789 });
            _context.Doctor.Add(new Doctor() { Id = 456 });
            _context.Doctor.Add(new Doctor() { Id = 901 });
        }

        [Test]
        public void AddBooking_ValidatesRequest()
        {
            //arrange
            var request = _fixture.Create<AddBookingRequest>();

            //act
            _bookingService.AddBooking(request);

            //assert
            _validator.Verify(x => x.ValidateRequest(request), Times.Once);
        }

        [Test]
        public void AddBooking_ValidatorFails_ThrowsArgumentException()
        {
            //arrange
            var failedValidationResult = new PdrValidationResult(false, _fixture.Create<string>());

            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddBookingRequest>())).Returns(failedValidationResult);

            //act
            var exception = Assert.Throws<ArgumentException>(() => _bookingService.AddBooking(_fixture.Create<AddBookingRequest>()));

            //assert
            exception.Message.Should().Be(failedValidationResult.Errors.First());
        }

        [Test]
        public void AddBooking_AddsBookingToContextWithGeneratedId()
        {
            //arrange
            var request = _fixture.Create<AddBookingRequest>();
            var patient = _context.Patient.First();

            var expected = new Order
            {
                Id = new Guid(),
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientId = 123,
                DoctorId = 901,
                Patient = _context.Patient.First(p => p.Id == 123),
                Doctor = null,//_context.Doctor.First(),
                SurgeryType = 159
            };

            //act
            _bookingService.AddBooking(request);

            //assert
            _context.Order.Should().ContainEquivalentOf(expected, options => options
                .Excluding(order => order.Id));

        }
    }
}
