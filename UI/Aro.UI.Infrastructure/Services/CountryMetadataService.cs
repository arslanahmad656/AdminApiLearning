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
            var response = await _http.GetAsync("api/country-metadata");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<CountryMetadataResponse>();

            if (data != null)
            {
                _countries = data.Countries;
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
        return _countries
            .Select(c => c.Name)
            .OrderBy(name => name);
    }

    public IEnumerable<string> GetAllCountryCodes()
    {
        return _countries
            .Select(c => c.PhoneCountryCode)
            .Distinct()
            .OrderBy(code => int.TryParse(code, out var num) ? num : int.MaxValue);
    }



    public bool ValidatePostalCode(string countryNameOrCode, string postalCode)
    {
        var country = GetByName(countryNameOrCode) ?? GetByISO2(countryNameOrCode);
        if (country == null || string.IsNullOrWhiteSpace(country.PostalCodeRegex)) return false;

        return Regex.IsMatch(postalCode, country.PostalCodeRegex);
    }

    public bool ValidateTelephone(string countryCode, string telephoneNumber)
    {
        var country = GetByCountryCode(countryCode);
        if (country == null || string.IsNullOrWhiteSpace(country.PhoneNumberRegex)) return false;

        return Regex.IsMatch(telephoneNumber, country.PhoneNumberRegex);
    }

    public (int MinLength, int MaxLength) GetPhoneLengthRequirements(string countryCode)
    {
        var country = GetByCountryCode(countryCode);
        if (country == null || string.IsNullOrWhiteSpace(country.PhoneNumberRegex))
            return (7, 15);

        var match = Regex.Match(country.PhoneNumberRegex, @"\{(\d+)(?:,(\d+))?\}");
        if (match.Success)
        {
            var min = int.Parse(match.Groups[1].Value);
            var max = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : min;
            return (min, max);
        }

        return (7, 15);
    }
}
