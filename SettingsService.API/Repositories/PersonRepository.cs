using SettingsService.API.Abstractions;
using SettingsService.API.Models;
using SettingsService.Db;
using SettingsService.Db.Entity;

namespace SettingsService.API.Repositories;

public class PersonRepository : IPersonRepository
{
    private static SettingsDbContext _dbContext;
    public PersonRepository(SettingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result> CreatePerson(PersonModel person)
    {
        if (IsPersonExists(person))
        {
            return new Result(Status.Error, "Такой пользователь уже существует");
        }
        _dbContext.Person.Add(new Person
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            MiddleName = person.MiddleName,
        });

        await _dbContext.SaveChangesAsync();

        return new Result(Status.Success, "Пользователь добавлен");
    }

    private static bool IsPersonExists(PersonModel person)
    {
        return _dbContext.Person.Any(p => person.FirstName == p.FirstName
                                          && person.LastName == p.LastName
                                          && person.MiddleName == p.MiddleName);
    }
}