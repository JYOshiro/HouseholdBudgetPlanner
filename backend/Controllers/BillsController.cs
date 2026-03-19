using HouseholdBudgetApi.DTOs.Bill;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Bills controller for managing recurring and one-time bills.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly ILogger<BillsController> _logger;

    public BillsController(IBillService billService, ILogger<BillsController> logger)
    {
        _billService = billService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all bills for the user's household.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetBills()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetBills called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bills = await _billService.GetBillsAsync(householdId);
        return Ok(bills);
    }

    /// <summary>
    /// Gets upcoming bills (due within 30 days).
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetUpcomingBills()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetUpcomingBills called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bills = await _billService.GetUpcomingBillsAsync(householdId);
        return Ok(bills);
    }

    /// <summary>
    /// Gets a single bill by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BillDto>> GetBill(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetBill called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bill = await _billService.GetBillAsync(id, householdId);
        if (bill == null)
        {
            return NotFound(new { message = "Bill not found" });
        }

        return Ok(bill);
    }

    /// <summary>
    /// Creates a new bill.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BillDto>> CreateBill([FromBody] CreateBillDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("CreateBill called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bill = await _billService.CreateBillAsync(householdId, request);
        return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
    }

    /// <summary>
    /// Updates a bill.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BillDto>> UpdateBill(int id, [FromBody] UpdateBillDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateBill called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bill = await _billService.UpdateBillAsync(id, householdId, request);
        return Ok(bill);
    }

    /// <summary>
    /// Deletes a bill.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBill(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteBill called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _billService.DeleteBillAsync(id, householdId);
        return NoContent();
    }

    /// <summary>
    /// Marks a bill as paid.
    /// </summary>
    [HttpPost("{id}/pay")]
    public async Task<ActionResult<BillDto>> MarkBillAsPaid(int id, [FromBody] MarkBillPaidDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("MarkBillAsPaid called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var bill = await _billService.MarkBillAsPaidAsync(id, householdId, request);
        return Ok(bill);
    }
}
