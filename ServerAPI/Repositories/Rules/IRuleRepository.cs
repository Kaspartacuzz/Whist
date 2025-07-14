using Core;

namespace ServerAPI.Repositories.Rules;

public interface IRuleRepository
{
    Task<List<Rule>> GetAll();
    Task<Rule?> GetById(int id);
    Task<Rule> Add(Rule rule);
    Task Update(Rule rule);
    Task Delete(int id);
}