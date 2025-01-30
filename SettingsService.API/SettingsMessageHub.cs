using Microsoft.AspNetCore.SignalR;
using SettingsService.API.Abstractions;

namespace SettingsService.API;

public class SettingsMessageHub(ISettingsRepository repository): Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var groupName = httpContext?.Request.Query["group"];
        
        if (!string.IsNullOrEmpty(groupName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName.Value.ToString());
        }
        
        await base.OnConnectedAsync();
        
        var settings = repository.GetAllSettings();
        await Clients.Caller.SendAsync("ReceiveAllSettings", settings);
    }
    
    public async Task SendCreateSettingsMessage(string message)
    {
        await Clients.Group("CreateSettings").SendAsync("ReceiveCreateSettingsMessage", message);
    }
    
    public async Task SendUpdateSettingsMessage(string message)
    {
        await Clients.Group("UpdateSettings").SendAsync("ReceiveUpdateSettingsMessage", message);
    }
    
    public async Task SendRemoveSettingsMessage(string message)
    {
        await Clients.Group("RemoveSettings").SendAsync("ReceiveRemoveSettingsMessage", message);
    }
}