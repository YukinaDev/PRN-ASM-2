namespace EVDMS.BusinessLogic.Application.Services;

public interface INotificationService
{
    Task NotifyPlanSubmittedAsync(string planType, int planId, string planName, string submitterId);
    Task NotifyPlanApprovedAsync(string planType, int planId, string planName, string submitterId, bool isApproved, string? reason = null);
}
