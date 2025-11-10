using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Commands;

/// <summary>
/// Represents a command to seed an application with data from a specified JSON file.
/// </summary>
/// <param name="JsonFilePath">The file path to the JSON file containing the data to seed.  The path must be a valid, accessible file path, and the
/// file must be in JSON format.</param>
public record SeedApplicationCommand(string JsonFilePath, string templatesDirectory) : IRequest;
