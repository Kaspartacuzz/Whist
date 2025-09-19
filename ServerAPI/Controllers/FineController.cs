using Core;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories.Fines;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FineController : ControllerBase
{
    private readonly IFineRepository _fineRepository;

    public FineController(IFineRepository fineRepository)
    {
        _fineRepository = fineRepository;
    }

    [HttpGet]
    public ActionResult<Fine[]> GetAll()
    {
        return _fineRepository.GetAll();
    }

    [HttpGet("user/{userId}")]
    public ActionResult<Fine[]> GetByUserId(int userId)
    {
        return _fineRepository.GetByUserId(userId);
    }

    [HttpPost]
    public IActionResult Add(Fine fine)
    {
        _fineRepository.AddFine(fine);
        return Ok();
    }

    [HttpPut]
    public IActionResult Update(Fine fine)
    {
        _fineRepository.Update(fine);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _fineRepository.Delete(id);
        return Ok();
    }
    
    [HttpGet("paged")]
    public ActionResult<PagedResult<Fine>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? userId = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = _fineRepository.GetPaged(page, pageSize, userId);
        return Ok(result);
    }
}