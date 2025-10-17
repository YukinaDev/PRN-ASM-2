# 🔔 SignalR Real-Time Notification System

## 📋 Tổng Quan

Hệ thống thông báo real-time sử dụng SignalR để gửi thông báo tức thời khi:
- **Staff submit ticket** → Admin nhận thông báo có ticket cần duyệt
- **Admin approve/reject** → Staff nhận thông báo kết quả phê duyệt

---

## 🏗️ Kiến Trúc

### Backend Components

#### 1. **NotificationHub** (`EVDMS.WebApp/Hubs/NotificationHub.cs`)
- SignalR Hub chính xử lý kết nối
- Tự động add/remove user vào group theo email
- Endpoint: `/notificationHub`

#### 2. **INotificationService & NotificationService** (`EVDMS.BusinessLogic/Application/Services/`)
```csharp
public interface INotificationService
{
    Task NotifyPlanSubmittedAsync(string planType, int planId, string planName, string submitterId);
    Task NotifyPlanApprovedAsync(string planType, int planId, string planName, string submitterId, bool isApproved, string? reason = null);
}
```

#### 3. **ISignalRHubProxy & SignalRHubProxy** (`EVDMS.WebApp/Hubs/SignalRHubProxy.cs`)
- Wrapper cho IHubContext để tách biệt Business Logic khỏi infrastructure
- Implement trong WebApp layer để inject vào BusinessLogic

#### 4. **IUserRepository & UserRepository** (`EVDMS.DataAccess/Repositories/`)
- Query users theo role (Admin, EVMStaff, etc.)
- Hỗ trợ gửi notification đến nhiều admin

---

### Frontend Components

#### 1. **SignalR Client** (`wwwroot/js/signalr-notification.js`)
- Kết nối SignalR Hub
- Lắng nghe sự kiện `ReceiveNotification`
- Xử lý hiển thị notification (badge, toast, dropdown)
- Auto-reconnect khi mất kết nối

#### 2. **Notification UI** (`Pages/Shared/_Layout.cshtml`)
- **Bell Icon**: Hiển thị badge đếm số notification
- **Dropdown Menu**: Danh sách notifications
- **Toast**: Popup notification tạm thời
- CSS animations và styling

---

## 🔄 Luồng Hoạt Động

### 1. Submit Ticket Flow
```
Staff Submit
    ↓
DistributionPlanService.SubmitAsync()
    ↓
NotificationService.NotifyPlanSubmittedAsync()
    ↓
Query Admin users from UserRepository
    ↓
SignalRHubProxy.SendToUserAsync() → Hub
    ↓
Admin Browser receives via SignalR
    ↓
JavaScript shows: Badge + Toast + Dropdown item
```

### 2. Approve/Reject Flow
```
Admin Approve/Reject
    ↓
DistributionPlanService.ApproveAsync()
    ↓
NotificationService.NotifyPlanApprovedAsync()
    ↓
Get Submitter user from UserRepository
    ↓
SignalRHubProxy.SendToUserAsync() → Hub
    ↓
Staff Browser receives via SignalR
    ↓
JavaScript shows: Badge + Toast + Dropdown item
```

---

## 📦 Notification Data Structure

```json
{
  "type": "PlanSubmitted" | "PlanApproved" | "PlanRejected",
  "planType": "DistributionPlan" | "DealerKpi",
  "planId": 123,
  "planName": "Kế hoạch Q4/2025",
  "message": "Kế hoạch mới cần phê duyệt: Kế hoạch Q4/2025",
  "reason": "Lý do từ chối (nếu rejected)",
  "isApproved": true | false,
  "timestamp": "2025-01-18T10:30:00Z"
}
```

---

## 🎨 UI Features

### Notification Bell
- **Badge**: Hiển thị số lượng notification chưa đọc
- **Animation**: Pulse effect để thu hút chú ý
- **Color**: Red badge trên icon bell trắng

### Dropdown Menu
- **Header**: "Thông báo" với button "Xóa tất cả"
- **List**: Tối đa 50 notifications, scroll được
- **Item**: Icon + Message + Timestamp
- **Click**: Navigate đến chi tiết plan

### Toast Notification
- **Auto-dismiss**: Tự động đóng sau 5 giây
- **Icons**: 
  - ✅ Green check: Approved
  - ❌ Red X: Rejected
  - 🔔 Blue bell: Submitted
- **Sound**: Subtle notification sound

---

## 🧪 Testing Guide

### Test Case 1: Submit Notification
1. Login as EVMStaff (`evmstaff@evdms.com`)
2. Create new Distribution Plan
3. Submit for approval
4. **Expected**: 
   - Staff không thấy notification (vì mình submit)
   - Login as Admin → Thấy badge +1, toast popup, dropdown có item mới

### Test Case 2: Approve Notification
1. Login as Admin
2. Go to Approvals page
3. Approve một plan đang chờ
4. **Expected**:
   - Admin thấy success message
   - Login as submitter Staff → Thấy notification "Kế hoạch đã được phê duyệt"

### Test Case 3: Reject Notification
1. Login as Admin
2. Reject một plan với lý do "Cần điều chỉnh số lượng"
3. **Expected**:
   - Staff nhận notification với reason hiển thị rõ ràng

### Test Case 4: Multiple Users
1. Mở 2 browser/incognito window
2. Window 1: Admin
3. Window 2: Staff
4. Submit từ Staff → Admin nhận ngay lập tức
5. Approve từ Admin → Staff nhận ngay lập tức

### Test Case 5: Reconnection
1. Login và connect SignalR
2. Tắt mạng hoặc ngắt kết nối
3. Bật lại mạng
4. **Expected**: SignalR auto-reconnect, console log "reconnected"

---

## 🔧 Configuration

### Program.cs
```csharp
// Register SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<ISignalRHubProxy, SignalRHubProxy>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Map Hub endpoint
app.MapHub<NotificationHub>("/notificationHub");
```

### JavaScript Client
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveNotification", function (data) {
    // Handle notification
});
```

---

## 📊 Database Changes

### New Tables/Repositories
- **IUserRepository**: Query users by role
- **UnitOfWork**: Added `Users` property

### No Schema Changes
- Không cần migration mới
- Sử dụng existing Identity tables

---

## 🎯 Benefits

### Business Value
1. ✅ **Instant Feedback**: Admin biết ngay có ticket mới
2. ✅ **Improved UX**: Staff không cần refresh để xem kết quả
3. ✅ **Faster Workflow**: Giảm thời gian xử lý ticket
4. ✅ **Transparency**: Track được status real-time

### Technical Value
1. ✅ **Scalable**: SignalR handle nhiều concurrent connections
2. ✅ **SOLID Compliant**: Separation of concerns với ISignalRHubProxy
3. ✅ **Testable**: Mock được INotificationService
4. ✅ **Maintainable**: Clean architecture, dễ extend

---

## 🚀 Future Enhancements

### Phase 2 Ideas
1. **Persistence**: Lưu notifications vào database
2. **Mark as Read**: Đánh dấu đã đọc
3. **Filter**: Lọc theo type, date
4. **Settings**: User preferences (sound on/off, email notification)
5. **Push Notifications**: Browser push API cho tab không active
6. **Mobile**: Extend cho mobile app

### Advanced Features
1. **Group Chat**: Staff chat với Admin về ticket
2. **File Attachments**: Đính kèm file trong notification
3. **Mention**: @mention user trong comments
4. **Activity Feed**: Timeline của tất cả activities

---

## 📝 Code Files Created/Modified

### Created Files
1. `EVDMS.WebApp/Hubs/NotificationHub.cs`
2. `EVDMS.WebApp/Hubs/SignalRHubProxy.cs`
3. `EVDMS.BusinessLogic/Application/Services/INotificationService.cs`
4. `EVDMS.BusinessLogic/Application/Services/NotificationService.cs`
5. `EVDMS.BusinessLogic/Application/Services/ISignalRHubProxy.cs`
6. `EVDMS.DataAccess/Repositories/IUserRepository.cs`
7. `EVDMS.DataAccess/Repositories/UserRepository.cs`
8. `EVDMS.WebApp/wwwroot/js/signalr-notification.js`

### Modified Files
1. `EVDMS.WebApp/Program.cs` - SignalR config
2. `EVDMS.WebApp/Pages/Shared/_Layout.cshtml` - UI components
3. `EVDMS.WebApp/wwwroot/css/site.css` - Notification styles
4. `EVDMS.DataAccess/Repositories/IUnitOfWork.cs` - Added Users
5. `EVDMS.DataAccess/Repositories/UnitOfWork.cs` - Added Users
6. `EVDMS.BusinessLogic/Application/Services/DistributionPlanService.cs` - Integration
7. `EVDMS.BusinessLogic/Application/Services/DealerKpiService.cs` - Integration

---

## ⚠️ Important Notes

### Security
- SignalR Hub requires `[Authorize]` attribute
- User groups based on email (unique identifier)
- No sensitive data in notifications (only IDs and names)

### Performance
- Notifications sent only to relevant users (not broadcast)
- Auto-reconnect để handle network issues
- Client-side throttling với max 50 notifications

### Browser Support
- Requires modern browser với WebSocket support
- Fallback to Server-Sent Events hoặc Long Polling
- IE11 not supported (SignalR 8.0)

---

## 🎓 Demo Script

### 5-Minute Demo
```
1. [0:00-0:30] Giới thiệu tính năng real-time notification
2. [0:30-1:30] Demo Staff submit → Admin nhận notification
   - Show badge animation
   - Show toast popup
   - Show dropdown list
3. [1:30-2:30] Demo Admin approve → Staff nhận notification
   - Show approve notification
   - Click notification → Navigate to details
4. [2:30-3:30] Demo Admin reject với reason
   - Show rejection notification với reason
5. [3:30-4:30] Demo multiple users (2 browsers)
6. [4:30-5:00] Tóm tắt benefits và technical approach
```

---

## 📞 Support

### Issues & Questions
- Check browser console for SignalR connection errors
- Verify user is authenticated (isSignedIn = true)
- Check Network tab for WebSocket connection
- Test with simple notification first

### Common Problems
1. **No notification received**: Check SignalR connection status in console
2. **Badge not updating**: Verify JavaScript loaded correctly
3. **Toast not showing**: Check Bootstrap JS is loaded
4. **Wrong user receives**: Verify user grouping by email

---

**Last Updated**: 2025-01-18  
**Version**: 1.0  
**Author**: Factory AI Assistant
