using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    [TestFixture]
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _addBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _addBookingRequestValidator = new AddBookingRequestValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {

        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        [Test]
        public void ValidateRequest_StartTimeBeforeUtcNow_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddSeconds(-1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("StartTime must be after UTC now");
        }

        [Test]
        public void ValidateRequest_EndTimeBeforeUtcNow_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.EndTime = DateTime.UtcNow.AddSeconds(-1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("EndTime must be after UTC now");
        }

        [Test]
        public void ValidateRequest_EndTimeBforeStartTime_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddMinutes(15);
            request.EndTime = DateTime.UtcNow.AddMinutes(1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("EndTime must be after StartTime");
        }

        [Test]
        public void ValidateRequest_PatientDoesNotExist_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.PatientId++; //offset patientId

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A patient with that ID could not be found");
        }

        [Test]
        public void ValidateRequest_DoctorDoesNotExist_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.DoctorId++; //offset patientId

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A doctor with that ID could not be found");
        }

        private AddBookingRequest GetValidRequest()
        {
            var patient = _fixture.Create<Patient>();
            _context.Patient.Add(patient);
            var doctor = _fixture.Create<Doctor>();
            _context.Doctor.Add(doctor);
            _context.SaveChanges();

            var request = _fixture.Build<AddBookingRequest>()
                .With(x => x.StartTime, DateTime.UtcNow.AddSeconds(5))
                .With(x => x.EndTime, DateTime.UtcNow.AddMinutes(15))
                .With(x => x.PatientId, patient.Id)
                .With(x => x.DoctorId, doctor.Id)
                .Create();
            return request;
        }
    }
}
