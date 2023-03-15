using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.ProvidusImplementation.Persistence
{
    public class ContextRepository<TEntity>
     where TEntity : class
    {
        ApplicationDbContext DataContext = new ApplicationDbContext();

        public List<TEntity> Get()
        {
            var context = DataContext;
            return context.Set<TEntity>().ToList();
        }
        public TEntity Get(string value) 
        {
            var context = DataContext;
            return context.Set<TEntity>().Find(value);
        }

        public void Save(TEntity entity)
        {

            var context = DataContext;
            try
            {
                context.Entry<TEntity>(entity).State = EntityState.Added;
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                throw dbex;
            }
        }

        public void Update(TEntity entity)
        {
            var context = DataContext;
            try
            {
                context.Entry<TEntity>(entity).State = EntityState.Modified;
                context.SaveChanges();
            }

            catch (DbEntityValidationException dbex)
            {
                throw dbex;
            }

        }
    }
}
