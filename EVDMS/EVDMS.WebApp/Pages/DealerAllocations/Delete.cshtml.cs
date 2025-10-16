using AutoMapper;
using EVDMS.BusinessLogic.Models;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DealerAllocations;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class DeleteModel : PageModel
{
    private readonly IDealerAllocationService _dealerAllocationService;
    private readonly IMapper _mapper;

    public DeleteModel(IDealerAllocationService dealerAllocationService, IMapper mapper)
    {
        _dealerAllocationService = dealerAllocationService;
        _mapper = mapper;
    }

    public DealerAllocationListItem? Allocation { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _dealerAllocationService.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        Allocation = _mapper.Map<DealerAllocationListItem>(entity);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _dealerAllocationService.DeleteAsync(id);
        TempData["StatusMessage"] = "Đã xoá phân bổ khỏi hệ thống.";
        return RedirectToPage("Index");
    }
}
