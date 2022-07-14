using AWS.Equinox.ApiClient.Configuration;
using AWS.Equinox.ApiClient.Handler;
using AWS.Equinox.Infastracture.MiddleWare.Debug;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace AWS.Equinox.ApiClient;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        AppSettings = new AppSettings();
        Configuration.Bind(AppSettings);
    }

    public IConfiguration Configuration { get; }

    private AppSettings AppSettings { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddOcelot().AddDelegatingHandler<DebuggingHandler>();

        services.PostConfigure<FileConfiguration>(configuration =>
        {
            foreach (var route in AppSettings.Ocelot.Routes.Select(x => x.Value))
            {
                var uri = new Uri(route.Downstream);

                foreach (var pathTemplate in route.UpstreamPathTemplates)
                {
                    configuration.Routes.Add(new FileRoute
                    {
                        UpstreamPathTemplate = pathTemplate,
                        DownstreamPathTemplate = pathTemplate,
                        DownstreamScheme = uri.Scheme,
                        DownstreamHostAndPorts = new List<FileHostAndPort>
                        {
                            new FileHostAndPort {Host = uri.Host, Port = uri.Port}
                        }
                    });
                }
            }

            foreach (var route in configuration.Routes)
            {
                if (string.IsNullOrWhiteSpace(route.DownstreamScheme))
                {
                    route.DownstreamScheme = Configuration["Ocelot:DefaultDownstreamScheme"];
                }

                if (string.IsNullOrWhiteSpace(route.DownstreamPathTemplate))
                {
                    route.DownstreamPathTemplate = route.UpstreamPathTemplate;
                }
            }
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        #region Middlewares

        app.UseMiddleware<DebugMiddleware>();

        #endregion

        app.UseWebSockets();
        app.UseOcelot().Wait();
    }
}