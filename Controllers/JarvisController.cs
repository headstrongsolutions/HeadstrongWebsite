﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Headstrong.Models;

using System.Net.Http;
using Newtonsoft.Json;

namespace Headstrong.Controllers
{
    public class JarvisController : Controller
    {
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
                    client.BaseAddress = new Uri("http://garage:5002");
                    var response = await client.GetAsync($"/externalcomms/GetLast7DaysTemperature/{id}");
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