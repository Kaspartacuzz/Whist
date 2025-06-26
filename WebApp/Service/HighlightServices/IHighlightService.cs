using Core;

namespace WebApp.Service.HighlightServices;

public interface IHighlightService
{
    Task<IEnumerable<Highlight>> GetAll();
    Task<Highlight?> GetById(int id);
    Task<Highlight> Add(Highlight highlight);
    Task Delete(int id);
    Task Update(Highlight highlight);
}
