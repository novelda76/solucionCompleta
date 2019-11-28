using Academy.Lib.Infrastructure;
using Academy.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Academy.Lib.Context
{
    public class Repository<T> where T : Entity
    {
        private static Dictionary<Guid, T> DbSet { get; set; } = new Dictionary<Guid, T>();

        public IQueryable<T> QueryAll()
        {
            return DbSet.Values.AsQueryable<T>();
        }

        public T Find(Guid id)
        {
            return DbSet[id];
        }

        public virtual SaveResult<T> Add(T entity)
        {
            var output = new SaveResult<T>();

            if (entity.Id == default(Guid))
                entity.Id = Guid.NewGuid();

            if (DbSet.ContainsKey(entity.Id))
            {
                output.IsSuccess = false;
                output.Validation.Errors.Add("Ya existe una entity con ese id");
            }

            if (output.IsSuccess)
            {
                DbSet.Add(entity.Id, entity);
            }

            return output;
        }

        public virtual SaveResult<T> Update(T entity)
        {
            var output = new SaveResult<T>();

            if (entity.Id == default(Guid))
            {
                output.IsSuccess = false;
                output.Validation.Errors.Add("No se puede actualizar una entidad sin Id");
            }

            if (entity.Id != default(Guid) && !DbSet.ContainsKey(entity.Id))
            {
                output.IsSuccess = false;
                output.Validation.Errors.Add("No existe una entity con ese id");
            }

            if (output.IsSuccess)
            {
                DbSet[entity.Id] = entity;
            }

            return output;
        }
    }
}
