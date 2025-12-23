using Aro.UI.Application.DTOs.CountryMetadata;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Aro.UI.Infrastructure.Services;

public class CountryMetadataService(HttpClient http) : ICountryMetadataService
{
    private readonly HttpClient _http = http;
    private ICollection<CountryMetadata> _countries = [];

    public async Task InitializeAsync()
    {
        if (_countries != null && _countries.Count != 0)
            return;

        try
        {
            var data = await _http.GetFromJsonAsync<ICollection<CountryMetadata>>("api/country-metadata");

            if (data != null)
            {
                _countries = data;
            }
        }
        catch (Exception ex)
        {

        }
    }

    public CountryMetadata? GetByName(string name)
    {
        return _countries.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            c.OfficialName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public CountryMetadata? GetByISO2(string iso2)
    {
        return _countries.FirstOrDefault(c =>
        c.ISO2.Equals(iso2, StringComparison.OrdinalIgnoreCase));
    }

    public CountryMetadata? GetByCountryCode(string countryCode)
    {
        return _countries.FirstOrDefault(c =>
        c.PhoneCountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<CountryMetadata> GetAll() => _countries;

    public IEnumerable<string> GetAllCountryNames()
    {
        return _countries.Select(c => c.Name);
    }

    public IEnumerable<string> GetAllCountryCodes()
    {
        return _countries.Select(c => c.PhoneCountryCode);
    }


    public bool ValidatePostalCode(string countryCode, string postalCode)
    {
        var country = GetByISO2(countryCode);
        if (country == null || string.IsNullOrWhiteSpace(country.PostalCodeRegex)) return false;

        return Regex.IsMatch(postalCode, country.PostalCodeRegex);
    }

    public bool ValidateTelephone(string countryCode, string telephoneNumber)
    {
        var country = GetByCountryCode(countryCode);
        if (country == null || string.IsNullOrWhiteSpace(country.PhoneNumberRegex)) return false;

        return Regex.IsMatch(telephoneNumber, country.PhoneNumberRegex);
    }
}
