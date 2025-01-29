namespace Configuration.API;

public interface IDbManager
{
    Task<Result> AddUser(User user);

    Task<Result> CreateConfiguration(CreatedConfigurationModel config);
    
    List<FullConfigurationModel> GetConfigurationsByDateAsync(DateTime date);
    
    List<FullConfigurationModel> GetConfigurationsByNameAsync(string name);
    List<FullConfigurationModel> GetAllConfigurations();
    
    Task<Result> UpdateConfiguration(Guid guid, Settings settings);
}