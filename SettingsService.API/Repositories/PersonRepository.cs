using SettingsService.API.Abstractions;
using SettingsService.API.Models;
using SettingsService.Db;

namespace SettingsService.API.Repositories;

public class PersonRepository : IPersonRepository
{
    private static SettingsDbContext _dbContext;
    public PersonRepository(SettingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result> CreatePerson(Person person)
    {
        _dbContext.Person.Add(new Db.Entity.Person
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            MiddleName = person.MiddleName,
        });

        await _dbContext.SaveChangesAsync();

        return new Result(Status.Success, "Пользователь добавлен");
    }
}