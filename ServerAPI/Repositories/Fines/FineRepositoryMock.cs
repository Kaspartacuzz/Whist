using Core;

namespace ServerAPI.Repositories.Fines;

public class FineRepositoryMock : IFineRepository
{
    private readonly List<Fine> _fines = new()
    {
        new Fine
        {
            Id = 1,
            UserId = 1,
            Amount = 100,
            Comment = "Tabte kortene på gulvet",
            Date = DateTime.Now.AddDays(-3),
            IsPaid = false,
        },
        new Fine
        {
            Id = 2,
            UserId = 2,
            Amount = 50,
            Comment = "Mødte for sent op",
            Date = DateTime.Now.AddDays(-2),
            IsPaid = true,
        }
    };

    public Fine[] GetAll()
    {
        return _fines.OrderByDescending(f => f.Date).ToArray();
    }

    public Fine[] GetByUserId(int userId)
    {
        return _fines.Where(f => f.UserId == userId).OrderByDescending(f => f.Date).ToArray();
    }

    public void AddFine(Fine fine)
    {
        fine.Id = _fines.Any() ? _fines.Max(f => f.Id) + 1 : 1;
        fine.Date = DateTime.Now;
        _fines.Add(fine);
    }

    public void Update(Fine fine)
    {
        var existing = _fines.FirstOrDefault(f => f.Id == fine.Id);
        if (existing != null)
        {
            existing.Amount = fine.Amount;
            existing.Comment = fine.Comment;
            existing.IsPaid = fine.IsPaid;
        }
    }

    public void Delete(int id)
    {
        var fine = _fines.FirstOrDefault(f => f.Id == id);
        if (fine != null)
        {
            _fines.Remove(fine);
        }
    }

    public PagedResult<Fine> GetPaged(int page, int pageSize, int? userId = null)
    {
        throw new NotImplementedException();
    }
}