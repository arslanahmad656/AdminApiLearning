namespace Aro.Common.Application.Services.Country;

public record class CountryResponse(Guid Id, string Name, string OfficialName, string ISO2, string PostalCodeRegex, string PhoneCountryCode, string PhoneNumberRegex);
