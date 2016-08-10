using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagLib;

namespace PandoraService2
{
    public class AppSettings
    {
        public string DownloadLocation { get; set; }
        public bool KeepAlbumArt { get; set; }
    }
}
