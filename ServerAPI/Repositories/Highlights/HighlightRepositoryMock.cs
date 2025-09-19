using Core;

namespace ServerAPI.Repositories.Highlights;

public class HighlightRepositoryMock : IHighlightRepository
{
    private static List<Highlight> _highlights = new(); // Midlertidig in-memory-liste
    private static int _nextId = 1;

    public Task<IEnumerable<Highlight>> GetAll()
    {
        return Task.FromResult(_highlights.AsEnumerable());
    }

    public Task<Highlight?> GetById(int id)
    {
        var highlight = _highlights.FirstOrDefault(h => h.Id == id);
        return Task.FromResult(highlight);
    }

    public Task<Highlight> Add(Highlight highlight)
    {
        highlight.Id = _nextId++;
        highlight.Date = DateTime.Today; // Sætter dagens dato ved oprettelse
        _highlights.Add(highlight);
        return Task.FromResult(highlight);
    }

    public Task Delete(int id)
    {
        var highlight = _highlights.FirstOrDefault(h => h.Id == id);
        if (highlight != null)
            _highlights.Remove(highlight);
        return Task.CompletedTask;
    }
    
    public Task Update(Highlight highlight)
    {
        var index = _highlights.FindIndex(h => h.Id == highlight.Id);
        if (index >= 0)
        {
            _highlights[index] = highlight;
        }
        return Task.CompletedTask;
    }

    public Task<PagedResult<Highlight>> GetPaged(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}