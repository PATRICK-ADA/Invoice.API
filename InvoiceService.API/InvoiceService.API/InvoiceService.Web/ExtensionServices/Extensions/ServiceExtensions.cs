using InvoiceService.API.InvoiceService.Core.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoomService.Infrastructure.Data;
using Invoice.Infrastructure.Repositories;
using Invoice.Core.Abstraction;
using Invoice.API.KafkaConsumerService;
using Serilog;

namespace Invoice.API.Notification.Web.Extensions
{
   
    public static class ServiceRegistrations
    {
        public static IServiceCollection ConfigureKafka(this IServiceCollection services)
        {


           services.AddHostedService<KafkaConsumerService.KafkaConsumerService>();


            return services;

        }



        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
            {
                config.Enrich.FromLogContext()
                    .WriteTo.Console()
                    .ReadFrom.Configuration(context.Configuration);

            });

            return builder; 

        }

        
        
        public static IServiceCollection AppServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddSingleton<IKafKaPublisherService, KafkaPublisherService>();
           

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddControllers();

            services.AddScoped<IInvoiceService, InvoiceService.Core.Services.InvoiceService>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(
                 configuration.GetConnectionString("DefaultConnection"),

                  b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)), ServiceLifetime.Transient);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithMethods("GET", "PUT", "DELETE", "POST")
                    );
            });

            return services;
        }


        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddSwaggerGen
                (g =>
                {
                    g.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Auction Invoice Management API",
                        Description = "Documentation for Auction Invoice Management API"

                    });

                });

            return services;
        }
    }
}