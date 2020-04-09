using Core.Domain.Abstractions.Entities;
using Framework.Infra.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infra.Data.SqlServer.DbContexts
{
    public class OnionArchitectDbContext:DbContext
    {
        public OnionArchitectDbContext(DbContextOptions options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var etitiesType = typeof(IEntity);
            var entitiesAssembly = etitiesType.Assembly;
            modelBuilder.RegisterAllEntities(etitiesType,entitiesAssembly);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
        
    }
}
