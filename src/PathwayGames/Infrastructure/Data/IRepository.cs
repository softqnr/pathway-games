using PathwayGames.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PathwayGames.Infrastructure.Data
{
    public interface IRepository<T> where T : ModelBase, new()
    {
        Task<List<object>> QueryAsync(string query, params object[] args);
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Int64 id);
        Task<List<T>> GetAllWithChildrenAsync(Expression<Func<T, bool>> predicate = null);
        Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        AsyncTableQuery<T> AsQueryable();
        Task<int> InsertAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task DeleteAsync(T entity, bool recursive = false); 
        Task InsertAllAsync(IEnumerable<T> objects);
        Task UpdateAllAsync(IEnumerable<T> objects);
        Task DeleteAllAsync(IEnumerable<T> objects, bool recursive = false);

        Task<List<T>> GetAllWithChildrenAsync();
        Task<T> GetWithChildrenAsync(Int64 id, bool recursive = false);
        Task InsertWithChildrenAsync(T entity, bool recursive = false);
        Task UpdateWithChildrenAsync(T entity);
        Task InsertOrReplaceWithChildrenAsync(T entity, bool recursive = false);
        Task InsertAllWithChildrenAsync(IEnumerable<T> objects, bool recursive = false);
        Task UpdateWithChildrenAsync(IEnumerable<T> objects);
    }
}
