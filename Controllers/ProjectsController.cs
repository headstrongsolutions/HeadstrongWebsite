using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Headstrong.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;

namespace Headstrong.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IWebHostEnvironment HostingEnvironment;

        private AppSettingsModel AppSettings;

        private const string Section = "Projects";

        public ProjectsController(IWebHostEnvironment hostingEnvironment, IOptionsMonitor<AppSettingsModel> appSettings)
        {
            this.HostingEnvironment = hostingEnvironment;
            this.AppSettings = appSettings.CurrentValue;
        }

        public IActionResult Index()
        {
            int fileCounter = 0;
            var markdownHierachyViewModel =
                new MarkdownHierachyViewModel
                {
                    MarkdownPages = new List<MarkdownPageViewModel>()
                };

            markdownHierachyViewModel.MarkdownPages.Add(
                new MarkdownPageViewModel
                {
                    Directory = true,
                    Id = fileCounter,
                    Name = AppSettings.MarkdownDirectory,
                    ParentId = null
                }
            );
            fileCounter++;

            foreach (string filename in Directory.EnumerateFiles(
                string.Format("{0}/{1}/{2}", "wwwroot", AppSettings.MarkdownDirectory, Section),
                "*.md",
                SearchOption.AllDirectories
                )
            )
            {
                var finalFilename = filename.Replace("/","");
                finalFilename = filename.Replace("/", "");
                finalFilename = filename.Replace("\"", "");
                finalFilename = filename.Replace(".md", "");
                finalFilename = filename.Replace(string.Format("{0}/",AppSettings.WwwFolderName), "");

                var newMarkdownPage = new MarkdownPageViewModel() {
                    Name = finalFilename,
                    Id = fileCounter,
                    ParentId = markdownHierachyViewModel.MarkdownPages.FirstOrDefault(p => p.Name == AppSettings.MarkdownDirectory).Id
                };

                fileCounter++;
                markdownHierachyViewModel.MarkdownPages.Add(newMarkdownPage);
            }
            return View(markdownHierachyViewModel);
        }

        [Route("markdown/markdownpage")]
        public async Task<IActionResult> MarkdownPage()
        {            
            var basePath = this.HostingEnvironment.WebRootPath;
            var relativePath = HttpContext.Items["MarkdownPath_OriginalPath"] as string;
            if (relativePath == null)
                return NotFound();

            relativePath = NormalizePath(relativePath).Substring(1);
            var pageFile = Path.Combine(basePath,relativePath);            
            if (!System.IO.File.Exists(pageFile))
                return NotFound();

            var markdown = await System.IO.File.ReadAllTextAsync(pageFile);
            if (string.IsNullOrEmpty(markdown))
                return NotFound();

            ViewBag.MarkdownText = Markdown.ParseHtmlString(markdown);
            return View("MarkdownPage");
        }

        /// <summary>
        /// Normalizes a file path to the operating system default
        /// slashes.
        /// </summary>
        /// <param name="path"></param>
        static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            char slash = Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }
    }
}