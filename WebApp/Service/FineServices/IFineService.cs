using Core;

namespace WebApp.Service.FineServices;

public interface IFineService
{
    Task<Fine[]> GetAll();
    Task<Fine[]> GetByUserId(int userId);
    Task Add(Fine fine);
    Task Update(Fine fine);
    Task Delete(int id);
    Task<PagedResult<Fine>> GetPaged(int page, int pageSize, int? userId = null);
}