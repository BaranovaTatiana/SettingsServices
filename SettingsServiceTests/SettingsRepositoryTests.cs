using System.Collections;
using FluentAssertions;
using SettingsService.API;
using SettingsService.API.Models;
using SettingsService.API.Models.Settings;
using SettingsService.API.Models.SettingsPresets;
using SettingsService.API.Repositories;
using SettingsService.Db;

namespace SettingsServiceTests;

public class SettingsRepositoryTests
{
    private SettingsRepository _settingsRepository;
    private SettingsDbContext _dbContext;
    
    [SetUp]
    public void Setup()
    {
        _dbContext = SeedEntities.InitData();
        _settingsRepository = new SettingsRepository(_dbContext);
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test, TestCaseSource(nameof(PersonModelCases))]
    public void Test_Create(CreatedSettingsModel newPerson, Result expected)
    { 
        var result = _settingsRepository.CreateSettings(newPerson).Result;
        result.Should().BeEquivalentTo(expected);
    }
    
    private static IEnumerable PersonModelCases
    {
        get
        {
            yield return new TestCaseData(new CreatedSettingsModel
                {
                    Person = new PersonModel { FirstName = "Варфоломей", MiddleName = "Иванович", LastName = "Иванов" },
                    Settings = new Settings(),
                    Name = "TestSettings"
                },
                new Result(Status.Error, "Такая конфигурация уже существует для пользователя Варфоломей Иванович Иванов"));
            yield return new TestCaseData(new CreatedSettingsModel
                {
                    Person = new PersonModel { FirstName = "Варфоломей", MiddleName = "Иванович", LastName = "Иванов" },
                    Settings = new Settings()
                    {
                        Font = new FontSettings()
                        {
                            FontFamily = "Consolas",
                            FontSize = 11,
                            FontStyle = "Style"
                        },
                        ColorScheme = new ColorSchemeSettings()
                        {
                            BackgroundColor = "Black",
                            HighlightColor = "White",
                            TextColor = "Blue"
                        },
                        HotKeys = new HotKeysSettings()
                        {
                            KeyBindings = new Dictionary<string, string>()
                            {
                                {"Копировать", "Ctrl+C"},
                            }
                        },
                        WindowPositions = new Dictionary<string, WindowPosition>
                        {
                            {
                                "Позиция справа",
                                new WindowPosition()
                                {
                                    Height = 100,
                                    Width = 200,
                                    X = 240,
                                    Y = 230,
                                    WindowName = "TestWindowN"
                                }
                            }
                        }
                    },
                    Name = "TestSettings1"
                },
                new Result(Status.Success, "Конфигурация успешно добавлена"));
        }
    }
    //туду дописать тесты
}