using System.Collections.Generic;
using AdventureService.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdventureService.Utils
{
    public static class DatabaseUtils
    {
        public static void UsePostgresNamingConventions(IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entity in entityTypes)
            {
                entity.Relational().TableName = entity.Relational().TableName.ToSnakeCase();

                foreach (var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = property.Name.ToSnakeCase();
                }

                foreach (var key in entity.GetKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.Relational().Name = index.Relational().Name.ToSnakeCase();
                }
            }
        }
    }
}