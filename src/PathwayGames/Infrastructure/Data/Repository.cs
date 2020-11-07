using SQLiteNetExtensionsAsync.Extensions;
using PathwayGames.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PathwayGames.Infrastructure.Data
{
    public class Repository<T> : IRepository<T> where T : ModelBase, new()
    {
        private SQLiteAsyncConnection db;

        public Repository(string databaseFile)
        {
            this.db = new SQLiteAsyncConnection(databaseFile);
        }

        public async Task<List<object>> QueryAsync(string query, params object[] args) =>
            await db.QueryAsync<object>(query, args);

        public AsyncTableQuery<T> AsQueryable() => 
            db.Table<T>();

        public async Task<List<T>> GetAllWithChildrenAsync() =>
            await db.GetAllWithChildrenAsync<T>();

        public async Task<List<T>> GetAllWithChildrenAsync(Expression<Func<T, bool>> predicate = null) =>
           await db.GetAllWithChildrenAsync<T>(predicate, true);

        public async Task<T> GetWithChildrenAsync(Int64 id, bool recursive = false) =>
            await db.FindWithChildrenAsync<T>(id, recursive);

        public async Task InsertWithChildrenAsync(T entity, bool recursive = false) =>
            await db.InsertWithChildrenAsync(entity, recursive);

        public async Task UpdateWithChildrenAsync(T entity) =>
            await db.UpdateWithChildrenAsync(entity);

        public async Task InsertAllWithChildrenAsync(IEnumerable<T> objects, bool recursive = false) =>
            await db.InsertAllWithChildrenAsync(objects, recursive);

        public async Task UpdateWithChildrenAsync(IEnumerable<T> objects) =>
            await db.UpdateWithChildrenAsync(objects);

        public async Task InsertOrReplaceWithChildrenAsync(T entity, bool recursive = false) =>
            await db.InsertOrReplaceWithChildrenAsync(entity, recursive);

        public async Task DeleteAllAsync(IEnumerable<T> objects, bool recursive = false) =>
            await db.DeleteAllAsync(objects, recursive);

        public async Task InsertAllAsync(IEnumerable<T> objects) => 
            await db.InsertAllAsync(objects);

        public async Task UpdateAllAsync(IEnumerable<T> objects) =>
            await db.UpdateAllAsync(objects);

        public async Task<List<T>> GetAllAsync() =>
            await db.Table<T>().ToListAsync();

        public async Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            var query = db.Table<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy<TValue>(orderBy);

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Int64 id) =>
             await db.FindAsync<T>(id);

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate) =>
            await db.FindAsync<T>(predicate);

        public async Task<int> InsertAsync(T entity) =>
             await db.InsertAsync(entity);

        public async Task<int> UpdateAsync(T entity) =>
             await db.UpdateAsync(entity);

        public async Task DeleteAsync(T entity, bool recursive = false) =>
             await db.DeleteAsync(entity, recursive);
    }
}