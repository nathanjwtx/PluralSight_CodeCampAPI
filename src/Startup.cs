using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreCodeCamp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddAutoMapper(Assembly.GetEntryAssembly());

            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 1);
                opt.ReportApiVersions = true;
                // this lets you change the default text in the api controller route from api-version to something else e.g. ver
                // opt.ApiVersionReader = new QueryStringApiVersionReader("ver");
                
                // sets a header name to use to request a version
                // opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                
                // can also combine different methods to allow either method to be used
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-Version"),
                    new QueryStringApiVersionReader("ver", "version")
                );
                
                // also change controller attribute. see CampsController
                // opt.ApiVersionReader = new UrlSegmentApiVersionReader();

                // doesn't work in this version of asp net but allows you to centralise versioning info rather than
                // put it on the controller. Either or.
                // opt.Conventions.Controller<TalksController>()
                //     .HasApiVersion(new ApiVersion(1, 0))
                //     .HasApiVersion(new ApiVersion(1, 1))
                //     .Action(c => c.Delete(default(string), default(int)))
                //         .MapToApiVersion(1, 1);
            });
            
            // EnableEndpointRouting is not compatible with versioning in aspnet core 2.2
            services.AddMvc(opt => opt.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
      
            app.UseMvc();
        }
    }
}