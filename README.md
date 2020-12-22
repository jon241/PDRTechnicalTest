# PushDoctor Technical Test

[![.NET Actions Status](https://github.com/jon241/PDRTechnicalTest/workflows/.NET/badge.svg)](https://github.com/jon241/PDRTechnicalTest/actions)
[![Coverage Status](https://coveralls.io/repos/github/jon241/PDRTechnicalTest/badge.svg?branch=ticket2)](https://coveralls.io/github/jon241/PDRTechnicalTest?branch=ticket2)

## Tasks to do

### Must do
- Add validation checks on the endpoint 
  - Endpoint to have controller unit tests as I would always do.
  - To ensure patients can’t book appointments in the past.
  - To ensure that a patient can’t book an appointment with a doctor who is already busy.
  - Check if EndTime is before StartTime, return BadRequest.
- Add a new endpoint which allows a patient to cancel appointments.

### Should do
- Update existing Nuget packages.
- Find a way to test Add*Request.Created DateTime property is within seconds of other DateTime value
- Increase missing code coverage of Controllers with unit tests.
- Create automated integration tests.

## Tasks completed
- Fixed unit tests
- Added github action to build and run unit tests
- Added build status to readme.
