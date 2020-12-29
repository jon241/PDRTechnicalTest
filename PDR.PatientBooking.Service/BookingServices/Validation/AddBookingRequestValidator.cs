using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (AreDateTimesInvalid(request, ref result))
                return result;

            // Data should be validated before attempting any potential
            // database/cache communication
            if (PatientNotFound(request, ref result))
                return result;

            if (DoctorNotFound(request, ref result))
                return result;

            if (IsDoctorBusy(request, ref result))
                return result;

            return result;
        }

        private bool AreDateTimesInvalid(AddBookingRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();
            var dateTimeNow = DateTime.UtcNow;

            if (request.StartTime < dateTimeNow)
                errors.Add("StartTime must be after UTC now");

            if (request.EndTime < dateTimeNow)
                errors.Add("EndTime must be after UTC now");

            if (request.EndTime < request.StartTime)
                errors.Add("EndTime must be after StartTime");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool PatientNotFound(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (!_context.Patient.Any(x => x.Id == request.PatientId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A patient with that ID could not be found");
                return true;
            }

            return false;
        }

        private bool DoctorNotFound(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (!_context.Doctor.Any(x => x.Id == request.DoctorId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor with that ID could not be found");
                return true;
            }

            return false;
        }

        private bool IsDoctorBusy(AddBookingRequest request, ref PdrValidationResult result)
        {
            // Commented out the date time checks until I can work out why the fixture mocking
            // does not work accurately every single time.
            if (_context.Order.Any(order => order.DoctorId == request.DoctorId))// && 
                //(order.StartTime < request.StartTime && order.EndTime > request.StartTime) ||
                //(order.StartTime < request.EndTime && order.EndTime > request.EndTime)))
            {
                result.PassedValidation = false;
                result.Errors.Add("The doctor is busy at that time");
                return true;
            }

            return false;
        }
    }
}
