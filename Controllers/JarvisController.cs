using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Headstrong.Models;

using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Headstrong.Controllers
{
    public class JarvisController : Controller
    {
        private readonly JarvisConfigurationModel _settings;

        public JarvisController(IOptions<JarvisConfigurationModel> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetTempData(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_settings.TemperatureReadings.BaseAddress);
                    var response = await client.GetAsync(_settings.TemperatureReadings.TemperatureUrl);
                    response.EnsureSuccessStatusCode();

                    return Content(await response.Content.ReadAsStringAsync());
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting temperatures: {httpRequestException.Message}");
                }
            }
        }
    }
}