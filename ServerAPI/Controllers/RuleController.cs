using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Rules;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RuleController : ControllerBase
{
    private readonly IRuleRepository _repository;

    public RuleController(IRuleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rule>>> GetAll()
    {
        var rules = await _repository.GetAll();
        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult<Rule>> Add([FromBody] Rule rule)
    {
        if (string.IsNullOrWhiteSpace(rule.Text))
            return BadRequest("Regeltekst mangler.");

        var added = await _repository.Add(rule);
        return CreatedAtAction(nameof(GetAll), new { id = added.Id }, added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Rule rule)
    {
        if (id != rule.Id) return BadRequest();
        await _repository.Update(rule);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        return NoContent();
    }
}