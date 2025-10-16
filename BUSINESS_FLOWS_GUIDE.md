# 📋 EVDMS - Hướng Dẫn 2 Luồng Nghiệp Vụ Chính

## 🎯 Tổng Quan Hệ Thống

**EVDMS (Electric Vehicle Distribution Management System)** - Hệ thống quản lý phân phối xe điện

### Vai trò người dùng:
1. **Admin** - Người phê duyệt cuối cùng
2. **EVMStaff** - Nhân viên nhà sản xuất xe điện (EVM)
3. **DealerManager** - Quản lý đại lý
4. **DealerStaff** - Nhân viên đại lý

---

## 🚗 LUỒNG 1: QUẢN LÝ KẾ HOẠCH PHÂN PHỐI XE

### 📊 Workflow Overview
```
EVMStaff              Admin                DealerManager
   |                    |                       |
   |--[1. Tạo Draft]--->|                       |
   |                    |                       |
   |--[2. Submit]------>|                       |
   |                    |                       |
   |                    |--[3. Review]          |
   |                    |                       |
   |                    |--[4. Approve]-------->|
   |                    |                       |
   |                    |                       |--[5. View & Execute]
```

### 📝 Chi Tiết Từng Bước

#### **BƯỚC 1: EVMStaff Tạo Kế Hoạch Phân Phối** 
**Vai trò:** EVMStaff  
**Màn hình:** `/DistributionPlans/Create`  
**Mục đích:** Lập kế hoạch phân phối xe cho các đại lý trong tháng

**Dữ liệu cần nhập:**
- **Tên kế hoạch** (VD: "Phân phối T10/2025")
- **Tháng mục tiêu** (Target Month)
- **Mô tả** (Description - optional)
- **Chi tiết phân phối** (Lines):
  - Đại lý (Dealer)
  - Dòng xe (Vehicle Model)
  - Số lượng (Planned Quantity)
  - Tỷ lệ chiết khấu (Discount Rate %)
  - Ghi chú (Notes - optional)

**Ví dụ dữ liệu:**
```
Tên: "Kế hoạch phân phối Q4/2025"
Tháng: 10/2025
Chi tiết:
  - Đại lý: ABC Motors | Xe: VinFast VF8 | Số lượng: 50 | Chiết khấu: 5%
  - Đại lý: XYZ Auto   | Xe: VinFast VF9 | Số lượng: 30 | Chiết khấu: 3%
```

**Trạng thái:** `Draft` (Nháp)  
**Action:** Click "Tạo kế hoạch" → Chuyển sang Details

---

#### **BƯỚC 2: EVMStaff Gửi Phê Duyệt**
**Vai trò:** EVMStaff  
**Màn hình:** `/DistributionPlans/Details/{id}`  
**Mục đích:** Gửi kế hoạch đến Admin để xét duyệt

**Action:**
1. Review lại thông tin kế hoạch
2. Click nút **"Submit for Approval"** (Gửi phê duyệt)
3. Hệ thống gọi: `DistributionPlanService.SubmitAsync(planId)`

**Validation:**
- Chỉ kế hoạch ở trạng thái `Draft` hoặc `Rejected` mới được submit

**Trạng thái:** `Draft` → `Submitted`

---

#### **BƯỚC 3: Admin Xem Danh Sách Chờ Duyệt**
**Vai trò:** Admin  
**Màn hình:** `/Admin/Approvals/Index`  
**Mục đích:** Xem tất cả kế hoạch chờ phê duyệt

**Hiển thị:**
- **Pending Distribution Plans** (Kế hoạch phân phối chờ duyệt)
- **Pending KPI Plans** (Kế hoạch KPI chờ duyệt)

**Action:** Click vào kế hoạch → Xem chi tiết

---

#### **BƯỚC 4: Admin Phê Duyệt/Từ Chối**
**Vai trò:** Admin  
**Màn hình:** `/DistributionPlans/Details/{id}`  
**Mục đích:** Quyết định phê duyệt hoặc từ chối kế hoạch

**Options:**
1. **Approve** (Phê duyệt)
   - Click "Approve"
   - Hệ thống gọi: `DistributionPlanService.ApproveAsync(planId, adminId, true, null)`
   - Trạng thái: `Submitted` → `Approved`

2. **Reject** (Từ chối)
   - Click "Reject"
   - Nhập lý do từ chối (Rejection Reason)
   - Hệ thống gọi: `DistributionPlanService.ApproveAsync(planId, adminId, false, reason)`
   - Trạng thái: `Submitted` → `Rejected`

**Validation:**
- Chỉ kế hoạch ở trạng thái `Submitted` mới được approve/reject

---

#### **BƯỚC 5: DealerManager Xem Kế Hoạch Được Duyệt**
**Vai trò:** DealerManager  
**Màn hình:** `/DistributionPlans/DealerBoard`  
**Mục đích:** Xem các kế hoạch phân phối đã được phê duyệt cho đại lý mình

**Hiển thị:**
- Danh sách kế hoạch có status = `Approved`
- Chỉ hiển thị các dòng (lines) liên quan đến dealer của mình
- Thông tin: Xe, số lượng, chiết khấu, notes

**Action:**
- Click vào kế hoạch → Xem chi tiết
- Chuẩn bị nhập xe theo kế hoạch

---

### 🔄 Quy Trình Xử Lý Rejection

**Khi bị từ chối:**
1. EVMStaff nhận thông báo kế hoạch bị reject
2. Xem lý do từ chối (Rejection Reason)
3. Chỉnh sửa lại kế hoạch (không cần tạo mới)
4. Submit lại → Quay về BƯỚC 2

---

### 🗂️ Database & Status Flow

**Table:** `DistributionPlans`

**Status Enum:**
```csharp
Draft = 0      // Nháp
Submitted = 1  // Chờ duyệt
Approved = 2   // Đã duyệt
Rejected = 3   // Bị từ chối
```

**Status Transitions:**
```
Draft ----Submit----> Submitted
  ↑                      |
  |                      |
  |--Reject-----------Approve/Reject
                         |
                         ↓
                    Approved/Rejected
```

---

### 💡 Điểm Demo Quan Trọng

1. **Multi-line Distribution**: Một kế hoạch có thể phân phối nhiều dòng xe cho nhiều đại lý
2. **Discount Management**: Mỗi dòng có thể có chiết khấu khác nhau
3. **Approval Workflow**: Workflow phê duyệt 2 cấp (Staff → Admin)
4. **Dealer Isolation**: Dealer chỉ xem được kế hoạch liên quan đến mình
5. **Status Tracking**: Theo dõi trạng thái kế hoạch qua từng bước

---

## 📊 LUỒNG 2: QUẢN LÝ KPI ĐẠI LÝ

### 📊 Workflow Overview
```
EVMStaff              Admin                DealerManager/Staff
   |                    |                       |
   |--[1. Tạo KPI]---->|                       |
   |                    |                       |
   |--[2. Submit]------>|                       |
   |                    |                       |
   |                    |--[3. Review]          |
   |                    |                       |
   |                    |--[4. Approve]-------->|
   |                    |                       |
   |                    |                       |--[5. Track Progress]
   |                    |                       |
   |                    |                       |--[6. Record Performance]
```

### 📝 Chi Tiết Từng Bước

#### **BƯỚC 1: EVMStaff Tạo KPI Cho Đại Lý**
**Vai trò:** EVMStaff  
**Màn hình:** `/DealerKpi/Create`  
**Mục đích:** Đặt mục tiêu KPI cho đại lý trong khoảng thời gian

**Dữ liệu cần nhập:**
- **Đại lý** (Dealer)
- **Ngày bắt đầu** (Target Start Date)
- **Ngày kết thúc** (Target End Date)
- **Mục tiêu doanh thu** (Revenue Target) - VD: 50,000,000,000 VNĐ
- **Mục tiêu số xe bán** (Unit Target) - VD: 100 xe
- **Mục tiêu vòng quay tồn kho** (Inventory Turnover Target) - VD: 2.5
- **Ghi chú** (Notes - optional)

**Ví dụ dữ liệu:**
```
Đại lý: ABC Motors
Thời gian: 01/10/2025 - 31/12/2025 (Q4)
Mục tiêu:
  - Doanh thu: 50 tỷ VNĐ
  - Số xe bán: 100 xe
  - Vòng quay tồn kho: 2.5 lần
Ghi chú: "KPI Q4 2025 - Focus on VF8"
```

**Trạng thái:** `Draft`  
**Action:** Click "Tạo KPI" → Chuyển sang Details

---

#### **BƯỚC 2: EVMStaff Gửi Phê Duyệt**
**Vai trò:** EVMStaff  
**Màn hình:** `/DealerKpi/Details/{id}`  
**Mục đích:** Gửi KPI đến Admin để xét duyệt

**Action:**
1. Review lại thông tin KPI
2. Click **"Submit for Approval"**
3. Hệ thống gọi: `DealerKpiService.SubmitAsync(planId)`

**Validation:**
- Ngày kết thúc phải > Ngày bắt đầu
- Chỉ KPI ở trạng thái `Draft` hoặc `Rejected` mới được submit

**Trạng thái:** `Draft` → `Submitted`

---

#### **BƯỚC 3: Admin Xem & Phê Duyệt KPI**
**Vai trò:** Admin  
**Màn hình:** `/Admin/Approvals/Index` → `/DealerKpi/Details/{id}`  
**Mục đích:** Phê duyệt hoặc từ chối KPI

**Options:**
1. **Approve**: KPI hợp lý → Approve
2. **Reject**: KPI không khả thi → Reject với lý do

**Hệ thống gọi:**
```csharp
DealerKpiService.ApproveAsync(planId, adminId, approve, reason)
```

**Trạng thái:** `Submitted` → `Approved` / `Rejected`

---

#### **BƯỚC 4: DealerManager Xem KPI Được Duyệt**
**Vai trò:** DealerManager  
**Màn hình:** `/DealerKpi/Index` (sau khi login tự redirect)  
**Mục đích:** Xem các KPI đã được phê duyệt cho đại lý mình

**Hiển thị:**
- Danh sách KPI có status = `Approved`
- Chỉ hiển thị KPI của dealer mình
- Thông tin: Thời gian, mục tiêu, tiến độ hiện tại

**Action:** Click vào KPI → Xem chi tiết và tiến độ

---

#### **BƯỚC 5: Theo Dõi Tiến Độ**
**Vai trò:** DealerManager / DealerStaff  
**Màn hình:** `/DealerKpi/DealerProgress`  
**Mục đích:** Xem tiến độ đạt KPI theo thời gian thực

**Hiển thị:**
- **Mục tiêu (Target):**
  - Revenue Target: 50 tỷ VNĐ
  - Unit Target: 100 xe
  - Inventory Turnover: 2.5

- **Đã đạt được (Achieved):**
  - Revenue Achieved: 35 tỷ VNĐ (70%)
  - Units Achieved: 68 xe (68%)
  - Inventory Turnover: 2.1 (84%)

- **Performance Logs** (Nhật ký hiệu suất):
  - Danh sách các lần ghi nhận performance

**Tính toán:**
```csharp
RevenueAchieved = Sum(PerformanceLogs.Revenue)
UnitsAchieved = Sum(PerformanceLogs.UnitsSold)
InventoryTurnoverAchieved = Average(PerformanceLogs.InventoryTurnover)
```

---

#### **BƯỚC 6: Ghi Nhận Hiệu Suất (Performance Log)**
**Vai trò:** DealerManager / DealerStaff  
**Màn hình:** `/DealerKpi/Details/{id}` → "Record Performance"  
**Mục đích:** Ghi nhận hiệu suất bán hàng theo ngày/tuần/tháng

**Dữ liệu cần nhập:**
- **Ngày hoạt động** (Activity Date)
- **Số xe bán được** (Units Sold) - VD: 5 xe
- **Doanh thu** (Revenue) - VD: 2,500,000,000 VNĐ
- **Vòng quay tồn kho** (Inventory Turnover) - VD: 2.3
- **Ghi chú** (Notes - optional)

**Ví dụ:**
```
Ngày: 15/10/2025
Số xe: 5 xe (VF8)
Doanh thu: 2.5 tỷ VNĐ
Vòng quay: 2.3
Ghi chú: "Chương trình khuyến mãi"
```

**Action:**
1. Nhập dữ liệu performance
2. Click "Record"
3. Hệ thống gọi: `DealerKpiService.RecordPerformanceAsync(log)`
4. Tự động cập nhật tiến độ KPI

**Data Processing:**
```csharp
log.ActivityDate = log.ActivityDate.Date;
log.Revenue = Math.Round(log.Revenue, 2);
log.InventoryTurnover = Math.Round(log.InventoryTurnover, 2);
```

---

### 🔄 Quy Trình Xử Lý Rejection

**Khi KPI bị từ chối:**
1. EVMStaff xem lý do (Rejection Reason)
2. Điều chỉnh mục tiêu KPI cho hợp lý
3. Submit lại → Quay về BƯỚC 2

---

### 🗂️ Database & Relationships

**Tables:**
1. `DealerKpiPlans` - Kế hoạch KPI
2. `DealerPerformanceLogs` - Nhật ký hiệu suất

**Relationship:**
```
DealerKpiPlan (1) ---has-many---> (N) DealerPerformanceLog
      ↓
    Dealer
      ↓
  ApplicationUser
```

**Status Flow:** (Giống Distribution Plan)
```
Draft → Submitted → Approved/Rejected
```

---

### 💡 Điểm Demo Quan Trọng

1. **Goal Setting**: Đặt mục tiêu cụ thể cho đại lý (Revenue, Units, Turnover)
2. **Real-time Tracking**: Theo dõi tiến độ real-time qua Performance Logs
3. **Performance Recording**: Dealer tự ghi nhận hiệu suất định kỳ
4. **Automatic Calculation**: Hệ thống tự động tính % hoàn thành KPI
5. **Historical Data**: Lưu trữ lịch sử performance theo thời gian
6. **Dealer Isolation**: Dealer chỉ xem KPI của mình

---

## 🎬 SCRIPT DEMO

### Demo 1: Distribution Plan (5-7 phút)

**Scene 1: EVMStaff tạo kế hoạch**
```
1. Login: evmstaff@evdms.com / password
2. Navigate: DistributionPlans → Create
3. Nhập: "Kế hoạch Q4/2025", Target: Oct 2025
4. Add lines:
   - ABC Motors | VF8 | 50 xe | 5% discount
   - XYZ Auto | VF9 | 30 xe | 3% discount
5. Create → Submit for Approval
```

**Scene 2: Admin phê duyệt**
```
1. Logout → Login: admin@evdms.com / password
2. Auto redirect → Admin/Approvals
3. Click vào "Kế hoạch Q4/2025"
4. Review chi tiết
5. Click "Approve" → Success
```

**Scene 3: Dealer xem kế hoạch**
```
1. Logout → Login: dealer1@evdms.com / password
2. Auto redirect → DealerBoard
3. Xem kế hoạch được phê duyệt
4. Review: Xe VF8, 50 chiếc, chiết khấu 5%
```

---

### Demo 2: Dealer KPI (5-7 phút)

**Scene 1: EVMStaff tạo KPI**
```
1. Login: evmstaff@evdms.com
2. Navigate: DealerKpi → Create
3. Chọn Dealer: ABC Motors
4. Period: Q4 2025 (Oct 1 - Dec 31)
5. Targets:
   - Revenue: 50 tỷ VNĐ
   - Units: 100 xe
   - Turnover: 2.5
6. Create → Submit
```

**Scene 2: Admin approve**
```
1. Login: admin@evdms.com
2. Admin/Approvals → Click KPI
3. Review → Approve
```

**Scene 3: Dealer theo dõi & ghi nhận**
```
1. Login: dealer1@evdms.com
2. Auto redirect → DealerProgress
3. Xem KPI: Target vs Achieved
4. Click "Record Performance"
5. Nhập:
   - Date: Today
   - Units Sold: 5
   - Revenue: 2.5 tỷ
   - Turnover: 2.3
6. Submit → Xem tiến độ cập nhật
```

---

## 🎯 Lợi Ích Business

### Luồng Distribution Plan:
1. ✅ Quản lý tập trung kế hoạch phân phối
2. ✅ Kiểm soát phê duyệt 2 cấp (Staff → Admin)
3. ✅ Transparency cho dealer về kế hoạch nhập xe
4. ✅ Quản lý discount policy thống nhất

### Luồng Dealer KPI:
1. ✅ Đặt mục tiêu rõ ràng cho đại lý
2. ✅ Theo dõi hiệu suất real-time
3. ✅ Data-driven decision making
4. ✅ Performance accountability
5. ✅ Historical tracking for analysis

---

## 🔐 Role-Based Access Control

| Feature | Admin | EVMStaff | DealerManager | DealerStaff |
|---------|-------|----------|---------------|-------------|
| Create Distribution Plan | ✅ | ✅ | ❌ | ❌ |
| Submit for Approval | ❌ | ✅ | ❌ | ❌ |
| Approve/Reject | ✅ | ❌ | ❌ | ❌ |
| View Approved Plans | ✅ | ✅ | ✅ (own) | ✅ (own) |
| Create KPI | ✅ | ✅ | ❌ | ❌ |
| View KPI | ✅ | ✅ | ✅ (own) | ✅ (own) |
| Record Performance | ❌ | ❌ | ✅ | ✅ |

---

## 📞 Technical Integration Points

### Repository Pattern (SOLID Compliant):
```csharp
IUnitOfWork
  ├── IDistributionPlanRepository
  ├── IDealerKpiPlanRepository
  ├── IDealerRepository
  └── IVehicleModelRepository

Services (Business Logic):
  ├── IDistributionPlanService
  └── IDealerKpiService
```

### AutoMapper:
```csharp
Entity ←→ DTO
  DistributionPlan ←→ DistributionPlanSummary/Detail
  DealerKpiPlan ←→ DealerKpiPlanSummary/Detail
```

---

## ✅ Checklist Demo

**Trước khi demo:**
- [ ] Có ít nhất 3 users: admin, evmstaff, dealer
- [ ] Database có data mẫu: dealers, vehicles
- [ ] Test login cho từng role
- [ ] Chuẩn bị data mẫu để nhập nhanh

**Trong demo:**
- [ ] Giải thích workflow trước khi thực hiện
- [ ] Highlight status changes (Draft → Submitted → Approved)
- [ ] Show role-based access (Admin vs Dealer view)
- [ ] Demo rejection flow nếu có thời gian
- [ ] Show performance tracking với real numbers

**Kết thúc:**
- [ ] Tóm tắt 2 luồng chính
- [ ] Nhấn mạnh SOLID principles đã áp dụng
- [ ] Q&A

---

## 🎓 Câu Hỏi Dự Đoán & Trả Lời

**Q1: Tại sao cần 2 cấp phê duyệt?**
A: Đảm bảo kiểm soát chất lượng. EVMStaff tạo, Admin review final approval.

**Q2: Dealer có thể tự tạo KPI không?**
A: Không. KPI do nhà sản xuất (EVM) đặt ra để đảm bảo consistency.

**Q3: Performance Log có thể sửa/xóa không?**
A: Trong demo này chưa có, nhưng có thể extend thêm tính năng audit trail.

**Q4: Làm sao biết Dealer có đạt KPI không?**
A: So sánh Achieved vs Target. Có thể thêm % completion indicator.

**Q5: Hệ thống có hỗ trợ notification không?**
A: Hiện tại dùng TempData message. Có thể mở rộng với email/SMS notification.

---

**📅 Last Updated:** 2025-01-14  
**📧 Contact:** support@evdms.com  
**🔗 GitHub:** [EVDMS Repository]
