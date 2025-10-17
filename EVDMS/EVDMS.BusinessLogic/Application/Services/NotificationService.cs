using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Repositories;

namespace EVDMS.BusinessLogic.Application.Services;

public class NotificationService : INotificationService
{
    private readonly ISignalRHubProxy _hubProxy;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(ISignalRHubProxy hubProxy, IUnitOfWork unitOfWork)
    {
        _hubProxy = hubProxy;
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyPlanSubmittedAsync(string planType, int planId, string planName, string submitterId)
    {
        var adminUsers = await _unitOfWork.Users.GetUsersByRoleAsync(RoleNames.Admin);
        
        foreach (var admin in adminUsers)
        {
            await _hubProxy.SendToUserAsync(admin.Email!, "ReceiveNotification", new
            {
                type = "PlanSubmitted",
                planType,
                planId,
                planName,
                message = $"Kế hoạch mới cần phê duyệt: {planName}",
                timestamp = DateTime.UtcNow
            });
        }
    }

    public async Task NotifyPlanApprovedAsync(string planType, int planId, string planName, string submitterId, bool isApproved, string? reason = null)
    {
        var submitter = await _unitOfWork.Users.GetUserByIdAsync(submitterId);
        if (submitter == null) return;

        var statusText = isApproved ? "đã được phê duyệt" : "đã bị từ chối";
        var message = isApproved 
            ? $"Kế hoạch {planName} đã được phê duyệt" 
            : $"Kế hoạch {planName} đã bị từ chối. Lý do: {reason}";

        await _hubProxy.SendToUserAsync(submitter.Email!, "ReceiveNotification", new
        {
            type = isApproved ? "PlanApproved" : "PlanRejected",
            planType,
            planId,
            planName,
            message,
            reason,
            isApproved,
            timestamp = DateTime.UtcNow
        });
    }
}
