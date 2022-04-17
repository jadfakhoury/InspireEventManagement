using EventManagementLibrary.Models;
using EventManagementLibrary.DBContext;
using EventManagementLibrary.Interfaces;

namespace EventManagementAPI.Repositories;

public class EventsGenericCRUD
{
    private readonly EventDBContext dbContext;
    private readonly ILogger<EventsGenericCRUD> _logger;

    public EventsGenericCRUD(ILogger<EventsGenericCRUD> logger, EventDBContext _dbContext)
    {
        _logger = logger;
        dbContext = _dbContext;
    }

    internal async Task<int> Create<T>(T item) where T : class, IDBObject
    {
        try
        {
            dbContext.Set<T>().Add(item);
            await dbContext.SaveChangesAsync();
            return item.Id;
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }

    internal async Task Delete<T>(int id) where T : class, IDBObject
    {
        try
        {
            T toDelete = dbContext.Set<T>().Where(e => e.Id.Equals(id)).SingleOrDefault();
            dbContext.Remove(toDelete);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }

    internal async Task Edit<T>(T entity) where T : class, IDBObject
    {
        try
        {
            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }

    internal T GetbyId<T>(int id) where T : class, IDBObject
    {
        try
        {
            return dbContext.Set<T>().Where(e => e.Id.Equals(id)).SingleOrDefault();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }

    internal List<T> GetList<T>() where T : class
    {
        try
        {
            return dbContext.Set<T>().ToList();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }
}
