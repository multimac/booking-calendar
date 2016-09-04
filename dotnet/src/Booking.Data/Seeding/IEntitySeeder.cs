using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Seeding
{
    public interface IEntitySeeder
    {
        Task Seed(DbContext context, IServiceProvider services);
        Task SeedDev(DbContext context, IServiceProvider services);
    }
    
    public interface IEntitySeeder<T> : IEntitySeeder where T : DbContext
    {
        Task Seed(T context, IServiceProvider services);
        Task SeedDev(T context, IServiceProvider services);
    }
    public abstract class EntitySeeder<T> : IEntitySeeder<T> where T : DbContext
    {
        public abstract Task Seed(T context, IServiceProvider services);
        public abstract Task SeedDev(T context, IServiceProvider services);

        public async Task Seed(DbContext context, IServiceProvider services)
            => await Seed(context, (c) => Seed(c, services));
        public async Task SeedDev(DbContext context, IServiceProvider services)
            => await Seed(context, (c) => SeedDev(c, services));

        public async Task Seed(DbContext context, Func<T, Task> seed)
        {
            var typedContext = context as T;
            if(typedContext == null)
                throw new ArgumentException($"The given {nameof(DbContext)} is not {typeof(T).Name}", nameof(context));

            await seed(typedContext);
        }
    }
    
    public interface IEntitySeeder<T, U> : IEntitySeeder
        where T : class
        where U : DbContext
    {
        Task Seed(DbSet<T> dbSet, U context, IServiceProvider services);
        Task SeedDev(DbSet<T> dbSet, U context, IServiceProvider services);
    }
    public abstract class EntitySeeder<T, U> : IEntitySeeder<T, U>
        where T : class
        where U : DbContext
    {
        public abstract Task Seed(DbSet<T> dbSet, U context, IServiceProvider services);
        public abstract Task SeedDev(DbSet<T> dbSet, U context, IServiceProvider services);

        public async Task Seed(DbContext context, IServiceProvider services)
            => await Seed(context, (dbSet, c) => Seed(dbSet, c, services));
        public async Task SeedDev(DbContext context, IServiceProvider services)
            => await Seed(context, (dbSet, c) => SeedDev(dbSet, c, services));

        public async Task Seed(DbContext context, Func<DbSet<T>, U, Task> seed)
        {
            var typedContext = context as U;
            if(typedContext == null)
                throw new ArgumentException($"The given {nameof(DbContext)} is not {typeof(U).Name}", nameof(context));

            var dbSet = typedContext.Set<T>();
            if(await dbSet.AnyAsync())
                return; 

            await seed(dbSet, typedContext);
        }
    }
}