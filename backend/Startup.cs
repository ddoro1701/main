using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace WebApplication1.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 🔹 API-Controller aktivieren
            services.AddControllers();

            // 🔹 OpenAPI (Swagger) aktivieren
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // 🔹 HTTPS erzwingen
            app.UseHttpsRedirection();

            // 🔹 Statische Dateien bereitstellen (Frontend in wwwroot)
            app.UseStaticFiles();

            // 🔹 Routing aktivieren
            app.UseRouting();

            // 🔹 API-Authentifizierung & Autorisierung aktivieren
            app.UseAuthorization();

            // 🔹 Endpoints registrieren
            app.UseEndpoints(endpoints =>
            {
                // 🔹 API-Routen registrieren
                endpoints.MapControllers();

                // 🔹 SPA-Support für React/Angular/Vue
                endpoints.MapFallbackToFile("/index.html");
            });

            // 🔹 OpenAPI (Swagger UI) aktivieren
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1");
                c.RoutePrefix = "swagger"; // URL unter /swagger erreichbar
            });
        }
    }
}
