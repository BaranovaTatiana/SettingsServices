using System.Collections;
using FluentAssertions;
using SettingsService.API;
using SettingsService.API.Models;
using SettingsService.API.Repositories;
using SettingsService.Db;

namespace SettingsServiceTests;

public class PersonRepositoryTests
{
    private PersonRepository _personRepository;
    private SettingsDbContext _dbContext;
    
    [SetUp]
    public void Setup()
    {
        _dbContext = SeedEntities.InitData();
        _personRepository = new PersonRepository(_dbContext);
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test, TestCaseSource(nameof(PersonModelCases))]
    public void Test_Create(PersonModel newPerson, Result expected)
    { 
        var result = _personRepository.CreatePerson(newPerson).Result;
        result.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable PersonModelCases
    {
        get
        {
            yield return new TestCaseData(new PersonModel
                    { FirstName = "Иван", MiddleName = "Иванович", LastName = "Иванов" },
                new Result(Status.Success, "Пользователь добавлен"));
            yield return new TestCaseData(new PersonModel
                    { FirstName = "Аркадий", LastName = "Макаров"},
                new Result(Status.Success, "Пользователь добавлен"));
            yield return new TestCaseData(new PersonModel 
                    { FirstName = "Варфоломей", MiddleName = "Иванович", LastName = "Иванов" },
                new Result(Status.Error, "Такой пользователь уже существует"));
        }
    }
}