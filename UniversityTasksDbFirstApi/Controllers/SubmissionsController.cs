using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _service;

    public SubmissionsController(SubmissionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubmission([FromBody] CreateSubmissionDto dto)
    {
        try
        {
            var id = await _service.CreateSubmissionAsync(dto);
            return CreatedAtAction(
                nameof(GradeSubmission),
                new { idSubmission = id },
                new { submissionId = id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already submitted"))
                return Conflict(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{idSubmission:int}/grade")]
    public async Task<IActionResult> GradeSubmission(int idSubmission, [FromBody] GradeSubmissionDto dto)
    {
        try
        {
            await _service.GradeSubmissionAsync(idSubmission, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{idSubmission:int}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        try
        {
            await _service.DeleteSubmissionAsync(idSubmission);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
