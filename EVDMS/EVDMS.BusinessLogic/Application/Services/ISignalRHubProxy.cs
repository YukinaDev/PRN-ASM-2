namespace EVDMS.BusinessLogic.Application.Services;

public interface ISignalRHubProxy
{
    Task SendToUserAsync(string userEmail, string method, object data);
}
