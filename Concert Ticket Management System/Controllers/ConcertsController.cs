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
            return BadRequest(new ApiResponse<string>(false, "Failed to add the concert."));
        }

        _logger.LogInformation("Concert added successfully. Name: {Name}, ConcertDate: {ConcertDate}, Description: {Description}",
            concertDTO.Name, concertDTO.ConcertDate, concertDTO.Description);

        return Ok(new ApiResponse<Concert>(true, "Concert added successfully.", result));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var concerts = await _concertService.GetAllConcertsAsync(cancellationToken);
        if (concerts.Any())
        {
            _logger.LogInformation("Retrieved {Count} concerts.", concerts.Count());
            return Ok(new ApiResponse<IEnumerable<Concert>>(true,string.Empty, concerts ));
        }

        return NotFound(new ApiResponse<string>(false, "No concerts found."));
    }
}
