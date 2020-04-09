using Framework.Infra.Extentions;
using Infra.Authentication.Jwt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Authentication.Jwt.DbContexts
{
    public class TokenStoreDbContext: DbContext
    {
        public TokenStoreDbContext(DbContextOptions options)
            : base(options)
        {
           
        }


        public DbSet<UserToken> UserToken { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //var etitiesType = typeof(UserToken);
            //var entitiesAssembly = etitiesType.Assembly;
            //modelBuilder.RegisterAllEntities(etitiesType, entitiesAssembly);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
    
}
