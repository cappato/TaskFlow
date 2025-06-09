using Microsoft.AspNetCore.Mvc;
using TaskFlow.Server.Services;
using TaskFlow.Shared.DTOs;

namespace TaskFlow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomAttributesController : ControllerBase
{
    private readonly ICustomAttributeService _customAttributeService;

    public CustomAttributesController(ICustomAttributeService customAttributeService)
    {
        _customAttributeService = customAttributeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomAttributeDto>>> GetAttributes()
    {
        var attributes = await _customAttributeService.GetAllAttributesAsync();
        return Ok(attributes);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<CustomAttributeDto>>> GetActiveAttributes()
    {
        var attributes = await _customAttributeService.GetActiveAttributesAsync();
        return Ok(attributes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomAttributeDto>> GetAttribute(int id)
    {
        var attribute = await _customAttributeService.GetAttributeByIdAsync(id);
        if (attribute == null)
            return NotFound();

        return Ok(attribute);
    }

    [HttpPost]
    public async Task<ActionResult<CustomAttributeDto>> CreateAttribute(CreateCustomAttributeDto createAttributeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var attribute = await _customAttributeService.CreateAttributeAsync(createAttributeDto);
            return CreatedAtAction(nameof(GetAttribute), new { id = attribute.Id }, attribute);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomAttributeDto>> UpdateAttribute(int id, UpdateCustomAttributeDto updateAttributeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var attribute = await _customAttributeService.UpdateAttributeAsync(id, updateAttributeDto);
            if (attribute == null)
                return NotFound();

            return Ok(attribute);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttribute(int id)
    {
        var result = await _customAttributeService.DeleteAttributeAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
