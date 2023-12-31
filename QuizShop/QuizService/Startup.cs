﻿using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizService.Model;

namespace QuizService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<QuizUpdateRequestValidator>();
        services.AddMvc();
        services.AddLogging();
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestMethod
                                    | HttpLoggingFields.RequestPath
                                    | HttpLoggingFields.RequestQuery
                                    | HttpLoggingFields.RequestBody
                                    | HttpLoggingFields.ResponseStatusCode;
        });
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();
        app.UseHttpLogging();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}