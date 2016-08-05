using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagLib;

namespace PandoraService2
{
    public class AlbumArt : IPicture
    {
        public ByteVector Data { get; set; }
        public string Description { get; set; }
        public string MimeType { get; set; }
        public PictureType Type { get; set; }

        public AlbumArt(Stream stream)
        {
            Data = ByteVector.FromStream(stream);
            Description = "AlbumArt";
            MimeType = "image/jpeg";
            Type = PictureType.FrontCover;
        }

    }
}
