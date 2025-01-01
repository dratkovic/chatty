using Sensei.Core.Domain.Models;

namespace Sensei.Core.Domain;

public interface IInitialData<T> where T: EntityBase
{
    /// <summary>
    /// Get initial data for the entity that is supposed 
    /// to be seeded in the database when the application starts
    /// </summary>
    /// <returns>List of entities that must be seeded for app to work</returns>
    List<T> GetInitialData();
    
    /// <summary>
    /// Get test data for the entity that is supposed
    /// to be seeded in the database when the application starts
    /// </summary>
    /// <returns>List of entities that can be seeded for testing purpose</returns>
    List<T> GetTestData();
}