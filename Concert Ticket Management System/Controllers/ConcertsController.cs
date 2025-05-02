using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Services.ConcertServices;
using Concert_Ticket_Management_System.Shared;
using Concert_Ticket_Management_System.Shared.Validators;
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
        if (concertDTO == null)
        {
            return BadRequest(new ApiResponse<string>(false, new List<string> { "Concert object cannot be null." }));
        }

        var validator = new ConcertDTOValidator();
        var validationResult = validator.Validate(concertDTO);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ApiResponse<string>(false, errors));
        }

        var result = await _concertService.AddConcertAsync(concertDTO, cancellationToken).ConfigureAwait(false);
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
        var concerts = await _concertService.GetAllConcertsAsync(cancellationToken).ConfigureAwait(false);
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
        var concert = await _concertService.GetConcertByIdAsync(concertId, cancellationToken).ConfigureAwait(false);
        if (concert != null)
        {
            return Ok(new ApiResponse<Concert>(true, null, concert));
        }
        return NotFound(new ApiResponse<string>(false, new List<string> { "Concert not found." }));
    }

    [HttpDelete("{concertId:int}")]
    public async Task<IActionResult> DeleteConcertAsync(int concertId, CancellationToken cancellationToken)
    {
        await _concertService.DeleteConcertAsync(concertId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPut("{concertId:int}")]
    public async Task<IActionResult> UpdateConcertAsync(int concertId, [FromBody] ConcertDTO concertDTO, CancellationToken cancellationToken)
    {
        var result = await _concertService.UpdateConcertAsync(concertId, concertDTO, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<Concert>(true, new List<string> { "Concert updated successfully." }, result.Data));
        }

        return BadRequest(new ApiResponse<string>(false, result.Errors));
    }

    [HttpPut("{concertId}/manage-capacity")]
    public async Task<IActionResult> ManageCapacity(int concertId, [FromBody] ManageCapacityRequest request, CancellationToken cancellationToken)
    {
        var result = await _concertService.UpdateAvailableCapacityAsync(concertId, request, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<string>(false, result.Errors));
        }

        return Ok(new ApiResponse<int>(true, new List<string> { "Capacity updated successfully." }, result.Data));

    }

    [HttpPost("{concertId}/reserve-tickets")]
    public async Task<IActionResult> ReserveTicketsAsync(int concertId, [FromBody] ReservationRequest request, CancellationToken cancellationToken)
    {
        // There's a lot of validation here and it's cluttering the controller. TODO move these validations to a middleware
        var concertValidator = new ConcertIdValidator();
        var concertIdValidationResult = concertValidator.Validate(concertId);

        if (!concertIdValidationResult.IsValid)
        {
            return BadRequest(new ApiResponse<string>(false, concertIdValidationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var reservationValidator = new ReservationRequestValidator();
        var reservationValidationResult = reservationValidator.Validate(request);

        if (!reservationValidationResult.IsValid)
        {
            return BadRequest(new ApiResponse<string>(false, reservationValidationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        if (request == null)
        {
            return BadRequest(new ApiResponse<string>(false, new List<string> { "Request cannot be null." }));
        }

        // Ensure the concertId in the path matches the one in the request body
        if (concertId != request.ConcertId)
        {
            return BadRequest(new ApiResponse<string>(false, new List<string> { "Concert ID in the path does not match the request body." }));
        }

        var result = await _concertService.ReserveTicketsAsync(request, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<string>(false, result.Errors));
        }

        return Ok(new ApiResponse<Reservation>(true, new List<string> { "Ticket(s) reserved successfully." }, result.Data));
    }

    [HttpGet("{concertId:int}/reservations/{reservationId:int}")]
    public async Task<IActionResult> GetReservationAsync(int reservationId,int concertId, CancellationToken cancellationToken)
    {
        var result = await _concertService.GetReservationByIdAsync(reservationId, concertId, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound(new ApiResponse<string>(false, new List<string> { "Reservation not found." }));
        }

        return Ok(new ApiResponse<Reservation>(true, new List<string> { "Reservation retrieved successfully." }, result));
    }
}