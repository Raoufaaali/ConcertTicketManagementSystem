using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Services.ConcertServices;
using Concert_Ticket_Management_System.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Concert_Ticket_Management_System.Controllers;

[Route("api/[controller]")]
public class ConcertsController : Controller
{
    private readonly IConcertService _concertService;
    private readonly ILogger<ConcertsController> _logger;

    public ConcertsController(
        IConcertService concertService,
        ILogger<ConcertsController> logger)
    {
        _concertService = concertService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddConcertAsync([FromBody] ConcertDTO concertDTO, CancellationToken cancellationToken)
    {
        var result = await _concertService.AddConcertAsync(concertDTO, cancellationToken);
        if (result == null)
        {
            return BadRequest(new ApiResponse<string>(false, new List<string> { "Failed to add the concert." }));
        }

        _logger.LogInformation("Concert added successfully. Name: {Name}, ConcertDate: {ConcertDate}, Description: {Description}",
            concertDTO.Name, concertDTO.ConcertDate, concertDTO.Description);

        return Ok(new ApiResponse<Concert>(true, new List<string> { "Concert added successfully." }, result));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var concerts = await _concertService.GetAllConcertsAsync(cancellationToken);
        if (concerts.Any())
        {
            _logger.LogInformation("Retrieved {Count} concerts.", concerts.Count());
            return Ok(new ApiResponse<IEnumerable<Concert>>(true, null, concerts));
        }

        return NotFound(new ApiResponse<string>(false, new List<string> { "No concerts found." }));
    }

    [HttpGet("{concertId:int}")]
    public async Task<IActionResult> GetConcertByIdAsync(int concertId, CancellationToken cancellationToken)
    {
        var concert = await _concertService.GetConcertByIdAsync(concertId, cancellationToken);
        if (concert != null)
        {
            return Ok(new ApiResponse<Concert>(true, null, concert));
        }
        return NotFound(new ApiResponse<string>(false, new List<string> { "Concert not found." }));
    }

    [HttpDelete("{concertId:int}")]
    public async Task<IActionResult> DeleteConcertAsync(int concertId, CancellationToken cancellationToken)
    {
        await _concertService.DeleteConcertAsync(concertId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{concertId:int}")]
    public async Task<IActionResult> UpdateConcertAsync(int concertId, [FromBody] ConcertDTO concertDTO, CancellationToken cancellationToken)
    {
        var result = await _concertService.UpdateConcertAsync(concertId, concertDTO, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<Concert>(true, new List<string> { "Concert updated successfully." }, result.Data));
        }

        return BadRequest(new ApiResponse<string>(false, result.Errors));
    }

    [HttpPut("{concertId}/manage-capacity")]
    public async Task<IActionResult> ManageCapacity(int concertId, [FromBody] ManageCapacityRequest request, CancellationToken cancellationToken)
    {
        var result = await _concertService.UpdateAvailableCapacityAsync(concertId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<string>(false, result.Errors));
        }

        return Ok(new ApiResponse<int>(true, new List<string> { "Capacity updated successfully." }, result.Data));

    }

}