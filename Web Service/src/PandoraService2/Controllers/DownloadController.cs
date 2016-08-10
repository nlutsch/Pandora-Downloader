﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using TagLib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PandoraService2.Controllers
{
    [Route("[controller]/[Action]")]
    public class DownloadController : Controller
    {
        private ILogger _logger;
        private AppSettings _settings;

        public DownloadController(ILoggerFactory logFactory, IOptions<AppSettings> settings)
        {
            _logger = logFactory.CreateLogger(nameof(DownloadController));
            _settings = settings.Value;
        }

        [HttpPost]
        public IActionResult Download(string title, string artist, string album, string station, string url, string artUrl)
        {
            var uri = new Uri(url);
            var artUri = new Uri(artUrl);
            var songfilename = _settings.DownloadLocation + "/" + station + "/" + artist + " - " + title + ".mp3";
            var artfilename = _settings.DownloadLocation + "/" + station + "/artwork/" + album + ".jpg";

            Directory.CreateDirectory(station);
            Directory.CreateDirectory(_settings.DownloadLocation + "/" + station + "/artwork");

            //Microsoft.Extensions.Logging.Console.
            _logger.LogInformation("Attempting to download '{0}' by '{1}'.", title, artist);

            DownloadAsync(uri, songfilename, title, artist, album, artUri, artfilename);
            
            return Ok();
        }

        private async Task DownloadAsync(Uri uri, string songfilename, string title, string artist, string album, Uri artUri, string artfilename)
        {
            AlbumArt objArt = null;
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    using (
                        Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(songfilename, FileMode.Create, FileAccess.Write, FileShare.None, 10000000, true))
                    {
                        await contentStream.CopyToAsync(stream);
                        _logger.LogInformation("Successfully Downloaded {0}!", songfilename);
                    }
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get,artUri))
                {
                    using (
                        Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(artfilename, FileMode.Create, FileAccess.Write, FileShare.None, 100000, true))
                    {
                        await contentStream.CopyToAsync(stream);                        
                    }
                }

                using (var stream = new FileStream(artfilename, FileMode.Open, FileAccess.ReadWrite))
                {
                    objArt = new AlbumArt(stream);
                }

                using (var stream = new FileStream(songfilename, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (var tagFile = TagLib.File.Create(new StreamFileAbstraction(songfilename, stream, stream)))
                    {
                        tagFile.Tag.Title = title;
                        tagFile.Tag.Performers = new[] { artist };
                        tagFile.Tag.Album = album;
                        tagFile.Tag.Pictures = new IPicture[] { objArt };
                        tagFile.Save();
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
