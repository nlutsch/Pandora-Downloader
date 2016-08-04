using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using TagLib;
using Microsoft.Extensions.Logging;

namespace PandoraService2.Controllers
{
    [Route("[controller]/[Action]")]
    public class DownloadController : Controller
    {
        private ILogger _logger;

        public DownloadController(ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger(nameof(DownloadController));
        }

        [HttpPost]
        public IActionResult Download(string title, string artist, string album, string station, string url, string artUrl)
        {
            var uri = new Uri(url);
            var filename = artist + " - " + title + ".mp3";

            //Microsoft.Extensions.Logging.Console.
            _logger.LogInformation("Attempting to download '{0}' by '{1}'.", title, artist);

            DownloadAsync(uri, filename, title, artist, album);
            
            return Ok();
        }

        private async Task DownloadAsync(Uri uri, string filename, string title, string artist, string album)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    using (
                        Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 10000000, true))
                    {
                        await contentStream.CopyToAsync(stream);
                        _logger.LogInformation("Successfully Downloaded {0}!", filename);

                        using (var tagFile = TagLib.File.Create(new StreamFileAbstraction(filename, stream, stream)))
                        {
                            tagFile.Tag.Title = title;
                            tagFile.Tag.Performers = new[] { artist };
                            tagFile.Tag.Album = album;
                            tagFile.Save();
                        }
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
