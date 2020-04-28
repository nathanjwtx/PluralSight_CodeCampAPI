using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CoreCodeCamp.Data
{
    public class CampContext : DbContext
    {
        private readonly DbContextOptions _options;
        private readonly IConfiguration _config;

        public CampContext(DbContextOptions<CampContext> options, IConfiguration config) : base(options)
        {
            _options = options;
            _config = config;
        }

        public DbSet<Camp> Camps { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Talk> Talks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var secretConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddUserSecrets("242f3d33-7096-46a9-8205-f3c9dd8394b4")
                .Build();
            
            var builder = new NpgsqlConnectionStringBuilder();
            builder.Port = Int32.Parse(_config.GetConnectionString("pgPort"));
            builder.Database = _config.GetConnectionString("pgDb");
            builder.Host = _config.GetConnectionString("pgHost");
            builder.Username = _config.GetConnectionString("pgUser");
            // from user-secret
            builder.Password = secretConfig.GetSection("Secrets")["CodeCamp"];
            
            optionsBuilder.UseNpgsql(builder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder bldr)
        {
            bldr.Entity<Camp>()
                .HasData(new 
                {
                    CampId = 1,
                    Moniker = "ATL2018",
                    Name = "Atlanta Code Camp",
                    EventDate = new DateTime(2018, 10, 18),
                    LocationId = 1,
                    Length = 1
                });

            bldr.Entity<Location>()
                .HasData(new 
                {
                    LocationId = 1,
                    VenueName = "Atlanta Convention Center",
                    Address1 = "123 Main Street",
                    CityTown = "Atlanta",
                    StateProvince = "GA",
                    PostalCode = "12345",
                    Country = "USA"
                });

            bldr.Entity<Talk>()
                .HasData(new 
                    {
                        TalkId = 1,
                        CampId = 1,
                        SpeakerId = 1,
                        Title = "Entity Framework From Scratch",
                        Abstract = "Entity Framework from scratch in an hour. Probably cover it all",
                        Level = 100
                    },
                    new
                    {
                        TalkId = 2,
                        CampId = 1,
                        SpeakerId = 2,
                        Title = "Writing Sample Data Made Easy",
                        Abstract = "Thinking of good sample data examples is tiring.",
                        Level = 200
                    });

            bldr.Entity<Speaker>()
                .HasData(new
                {
                    SpeakerId = 1,
                    FirstName = "Shawn",
                    LastName = "Wildermuth",
                    BlogUrl = "http://wildermuth.com",
                    Company = "Wilder Minds LLC",
                    CompanyUrl = "http://wilderminds.com",
                    GitHub = "shawnwildermuth",
                    Twitter = "shawnwildermuth"
                }, new
                {
                    SpeakerId = 2,
                    FirstName = "Resa",
                    LastName = "Wildermuth",
                    BlogUrl = "http://shawnandresa.com",
                    Company = "Wilder Minds LLC",
                    CompanyUrl = "http://wilderminds.com",
                    GitHub = "resawildermuth",
                    Twitter = "resawildermuth"
                });

        }

    }
}