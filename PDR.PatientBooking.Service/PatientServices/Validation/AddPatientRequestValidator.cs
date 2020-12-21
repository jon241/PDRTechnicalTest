using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.PatientServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PDR.PatientBooking.Service.PatientServices.Validation
{
    public class AddPatientRequestValidator : IAddPatientRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddPatientRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddPatientRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            // inserted here because the data should be valid before attempting any potential
            // database communication
            if (IsEmailInvalid(request, ref result))
                return result;

            if (PatientAlreadyInDb(request, ref result))
                return result;

            if (ClinicNotFound(request, ref result))
                return result;

            return result;
        }

        private bool MissingRequiredFields(AddPatientRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.FirstName))
                errors.Add("FirstName must be populated");

            if (string.IsNullOrEmpty(request.LastName))
                errors.Add("LastName must be populated");

            if (string.IsNullOrEmpty(request.Email))
                errors.Add("Email must be populated");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        // My personal best practice is for any boolean statement/method/variable to be positive
        // e.g. IsEmailValid rather than IsEmailInvalid
        // I have kept the current practce here to be consistent, to do otherwise makes the code
        // less understandable.
        private bool IsEmailInvalid(AddPatientRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            // Email regex source from this website and
            // Email would be further validated when sent.
            // http://www.regular-expressions.info/email.html
            const string emailRegex = @"\A(?=[a-z0-9@.!#$%&'*+/=?^_‘{|}~-]{6,254}\z)(?=[a-z0-9.!#$%&'*+/=?^_‘{|}~-]{1,64}@)[a-z0-9!#$%&'*+/=?^_‘{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_‘{|}~-]+)*@(?:(?=[a-z0-9-]{1,63}\.)[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?=[a-z0-9-]{1,63}\z)[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\z";

            if (!Regex.IsMatch(request.Email, emailRegex))
                errors.Add("Email must be a valid email address");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool PatientAlreadyInDb(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (_context.Patient.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A patient with that email address already exists");
                return true;
            }

            return false;
        }

        private bool ClinicNotFound(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (!_context.Clinic.Any(x => x.Id == request.ClinicId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A clinic with that ID could not be found");
                return true;
            }

            return false;
        }
    }
}
