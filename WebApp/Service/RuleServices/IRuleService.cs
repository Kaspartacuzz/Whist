using Core;

namespace WebApp.Service.RuleServices;

public interface IRuleService
{
    Task<List<Rule>> GetAll();
    Task Add(Rule rule);
    Task Update(Rule rule);
    Task Delete(int id);
}