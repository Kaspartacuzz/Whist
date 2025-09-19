using Core;

namespace ServerAPI.Repositories.Fines;

public interface IFineRepository
{
    Fine[] GetAll();               // Hent alle bøder
    Fine[] GetByUserId(int userId); // Hent bøder for én bruger
    void AddFine(Fine fine);      // Tilføj ny bøde
    void Update(Fine fine);       // Opdater en bøde (fx hvis betalt)
    void Delete(int id);          // Slet en bøde (valgfrit)
    PagedResult<Fine> GetPaged(int page, int pageSize, int? userId = null);
}