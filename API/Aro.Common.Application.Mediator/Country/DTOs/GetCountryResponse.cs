namespace Aro.Common.Application.Mediator.Country.DTOs;

public record GetCountryResponse(Guid Id, string Name, string OfficialName, string ISO2, string PostalCodeRegex, string PhoneCountryCode, string PhoneNumberRegex);
