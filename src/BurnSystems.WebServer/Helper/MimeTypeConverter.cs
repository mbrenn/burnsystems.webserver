using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Helper
{
    public class MimeTypeConverter
    {
        /// <summary>
        /// Stores the list of known mimetypes
        /// </summary>
        private List<MimeTypeInfo> infos = new List<MimeTypeInfo>();

        public void Add(MimeTypeInfo info)
        {
            this.infos.Add(info);
        }

        /// <summary>
        /// Converts given file extenstion to mimetype or returns null, if unknwon
        /// </summary>
        /// <param name="fileExtension">File extension to be converted</param>
        /// <returns>Found mimetype or null</returns>
        public string ConvertFromExtension(string fileExtension)
        {
            return
                this.infos
                    .Where(x => x.FileExtension == fileExtension)
                    .Select(x => x.MimeType)
                    .FirstOrDefault();                    
        }

        /// <summary>
        /// Gets the default mimetype converter with the most important mimetypes
        /// </summary>
        public static MimeTypeConverter Default
        {
            get
            {
                var result = new MimeTypeConverter();

                result.Add(new MimeTypeInfo(".txt", "text/plain"));
                result.Add(new MimeTypeInfo(".jpg", "image/jpeg"));
                result.Add(new MimeTypeInfo(".html", "text/html"));
                result.Add(new MimeTypeInfo(".htm", "text/html"));

                return result;
            }
        }
    }
}
