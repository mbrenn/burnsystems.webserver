using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Helper
{
    public class MimeTypeInfo
    {
        public string FileExtension
        {
            get;
            private set;
        }

        public string MimeType
        {
            get;
            private set;
        }

        public MimeTypeInfo(string fileExtension, string mimeType)
        {
            if (!fileExtension.StartsWith("."))
            {
                throw new ArgumentException("fileExtension does not starts with . (dot)", "fileExtension2");
            }

            this.FileExtension = fileExtension;
            this.MimeType = mimeType;
        }
    }
}
