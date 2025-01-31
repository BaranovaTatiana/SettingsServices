using Microsoft.EntityFrameworkCore;
using SettingsService.Db;
using SettingsService.Db.Entity;

namespace SettingsServiceTests;

public static class SeedEntities
{
    public static SettingsDbContext InitData()
    {
        var options = new DbContextOptionsBuilder<SettingsDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb1")
            .Options;
        
        var context = new SettingsDbContext(options);
        context.Database.EnsureCreated();
        InsertData(context);

        context.SaveChanges();
        return context;
    }

    private static void InsertData(SettingsDbContext context)
    {
        var guid1 = Guid.NewGuid();
        var personTable = new List<Person>
            {
                new() { FirstName = "Варфоломей", MiddleName = "Иванович", LastName = "Иванов"},
            }
            .AsQueryable();

        var settingsPresetVersionTable = new List<SettingsPresetVersion>
        {
            new()
            { 
                VersionNumber = 1,
                Settings = "set",
                Id = 1,
                CreationDate = DateTime.Now,
                SettingsPresetGuid = guid1,
            }
        }.AsQueryable();
        
        var settingsPresetTable = new List<SettingsPreset>
        {
            new()
            {
                Person = personTable.First(),
                Guid = guid1,
                PersonId = personTable.First().Id,
                Name = "TestSettings",
                SettingsPresetVersions = settingsPresetVersionTable.ToList()
            }
        }.AsQueryable();
        
        context.Person.AddRange(personTable);
        context.SettingsPresetVersion.AddRange(settingsPresetVersionTable);
        context.SettingsPreset.AddRange(settingsPresetTable);
    }
}