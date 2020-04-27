using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CoreCodeCamp.Data
{
    public class CampContextFactory : IDesignTimeDbContextFactory<CampContext>
    {
        public CampContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets("242f3d33-7096-46a9-8205-f3c9dd8394b4")
                .Build();
            
            var builder = new NpgsqlConnectionStringBuilder();
            builder.Port = Int32.Parse(config.GetConnectionString("pgPort"));
            builder.Database = config.GetConnectionString("pgDb");
            builder.Host = config.GetConnectionString("pgHost");
            builder.Username = config.GetConnectionString("PgUser");
            // from user-secret
            builder.Password = config["CodeCamp"];

            var optionsBuilder = new DbContextOptionsBuilder<CampContext>();
            optionsBuilder.UseNpgsql(builder.ConnectionString);

            return new CampContext(optionsBuilder.Options, config);
        }
    }
}