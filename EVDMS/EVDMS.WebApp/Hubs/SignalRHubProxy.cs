using EVDMS.BusinessLogic.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace EVDMS.WebApp.Hubs;

public class SignalRHubProxy : ISignalRHubProxy
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRHubProxy(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(string userEmail, string method, object data)
    {
        await _hubContext.Clients.Group(userEmail).SendAsync(method, data);
    }
}
