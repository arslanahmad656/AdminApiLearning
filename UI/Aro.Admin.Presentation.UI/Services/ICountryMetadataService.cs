using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Services;
public interface ICountryMetadataService
{
    Task InitializeAsync();
    CountryMetadata? GetByName(string name);
    CountryMetadata? GetByISO2(string iso2);
    IEnumerable<CountryMetadata> GetAll();
    IEnumerable<string> GetAllCountryNames();
    IEnumerable<string> GetAllCountryCodes();
    bool ValidatePostalCode(string countryName, string postalCode);
    bool ValidateTelephone(string countryCode, string telephoneNumber);
}
