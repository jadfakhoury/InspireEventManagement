using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EventManagementLibrary.Models;
using BookstopNetModels.Models;
using EventManagementLibrary.DBContext;
using EventManagementLibrary.Interfaces;

namespace EventManagementAPI.Repositories;

public class GenericCRUD
{
    private readonly EventDBContext dbContext;
    private readonly ILogger<GenericCRUD> _logger;

    public GenericCRUD(ILogger<GenericCRUD> logger, EventDBContext _dbContext)
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

    internal async Task CreateList<T>(List<T> items) where T : class
    {
        try
        {
            dbContext.Set<T>().AddRange(items);
            await dbContext.SaveChangesAsync();
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

    internal async Task DeleteList<T>(List<int> ids) where T : class, IDBObject
    {
        try
        {
            List<T> toDelete = dbContext.Set<T>().Where(e => ids.Contains(e.Id)).ToList();
            dbContext.RemoveRange(toDelete);
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

    internal List<T> GetList<T>(List<int> list) where T : class, IDBObject
    {
        try
        {
            return dbContext.Set<T>().Where(e => list.Contains(e.Id)).ToList();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }

    internal T GetSingle<T>() where T : class, IDBObject
    {
        try
        {
            return dbContext.Set<T>().FirstOrDefault();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
            throw;
        }
    }
}
