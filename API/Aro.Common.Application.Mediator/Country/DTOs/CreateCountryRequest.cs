using System;
using System.Collections.Generic;
using System.Text;

namespace Aro.Common.Application.Mediator.Country.DTOs;

public record CreateCountryRequest(string Name, string OfficialName, string ISO2, string PostalCodeRegex, string PhoneCountryCode, string PhoneNumberRegex);

