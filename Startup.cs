using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.Tables;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Westwind.AspNetCore.Markdown;
using Headstrong.Models;

using Microsoft.AspNetCore.HttpOverrides;

namespace Headstrong
{
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
            services.Configure<MarkdownConfigurationModel>(Configuration.GetSection("Markdown"));

            var markdownConfiguration = new MarkdownConfigurationModel();
            Configuration.GetSection("Markdown").Bind(markdownConfiguration);

            services.AddMarkdown(config =>
            {
                config.HtmlTagBlackList = "iframe|object|embed|form";
                if(markdownConfiguration.MarkdownDirectories != null && markdownConfiguration.MarkdownDirectories.Count > 0)
                {
                    var folderConfig = config.AddMarkdownProcessingFolder(markdownConfiguration.MarkdownDirectories.FirstOrDefault(), "~/Pages/__MarkdownPageTemplate.cshtml");
                    foreach (string folderName in markdownConfiguration.MarkdownDirectories.Where(x => x != markdownConfiguration.MarkdownDirectories.FirstOrDefault()))
                    {
                        config.AddMarkdownProcessingFolder(
                            string.Format("{0}/", folderName),
                            "~/Views/Shared/MarkdownPageTemplate.cshtml");
                    }
                    folderConfig.SanitizeHtml = true;
                    folderConfig.ProcessExtensionlessUrls = true;
                }
                
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UsePipeTables()
                        .UseGridTables()
                        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                        .UseAutoLinks() // URLs are parsed into anchors
                        .UseAbbreviations()
                        .UseYamlFrontMatter()
                        .UseEmojiAndSmiley(true)
                        .UseListExtras()
                        .UseFigures()
                        .UseTaskLists()
                        .UseCustomContainers()
                        .UseGenericAttributes();
                };
            });

            services.AddMvc()
                    .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");            
            }

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new List<string> { "index.md", "index.html" }
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // Patch path base with forwarded path
            app.Use(async (context, next) =>
            {
                var forwardedPath = context.Request.Headers["X-Forwarded-Path"].ToString();
                if (!string.IsNullOrEmpty(forwardedPath))
                {
                    context.Request.PathBase = forwardedPath;
                }

                await next();
            });

            //app.UseHttpsRedirection();
            

            //app.UseRouting(routes =>
            //{
            //    routes.MapApplication();
            //    routes.MapControllerRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            //app.UseCookiePolicy();
            //app.UseAuthorization();

            app.UseMarkdown();

            app.UseRouting();

            app.UseStaticFiles();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();

            });
        }
    }
}
