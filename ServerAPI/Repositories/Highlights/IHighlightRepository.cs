using Core;

namespace ServerAPI.Repositories.Highlights;

public interface IHighlightRepository
{
    Task<IEnumerable<Highlight>> GetAll();
    Task<Highlight?> GetById(int id);
    Task<Highlight> Add(Highlight highlight);
    Task Delete(int id);
    Task Update(Highlight highlight);
}