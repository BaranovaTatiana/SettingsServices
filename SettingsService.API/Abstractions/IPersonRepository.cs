using SettingsService.API.Models;

namespace SettingsService.API.Abstractions;

public interface IPersonRepository
{
    Task<Result> CreatePerson(PersonModel person);
}