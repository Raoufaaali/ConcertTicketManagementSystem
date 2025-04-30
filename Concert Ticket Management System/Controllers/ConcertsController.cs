using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Services.ConcertServices;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

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
    public async Task<IActionResult> AddConcertAsync(ConcertDTO concertDTO, CancellationToken cancellationToken)
    {
        var result = await _concertService.AddConcertAsync(concertDTO, cancellationToken);
        if (result == null)
        {
            throw new InvalidOperationException ("Failed to add the concert.");
        }

        _logger.LogInformation("Concert added successfully. Name: {Name}, StartDate: {StartDate}, Description: {Description}",
            concertDTO.Name, concertDTO.StartDate, concertDTO.Description);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var concerts = await _concertService.GetAllConcertsAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} concerts.", concerts.Count());
        return Ok(concerts);
    }
}
