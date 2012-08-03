using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BurnSystems.WebServer.Helper;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Responses
{
    public class PhysicalFileResponse : IRequestDispatcher
    {
        /// <summary>
        /// Gets or sets converter
        /// </summary>
        [Inject]
        public MimeTypeConverter MimeTypeConverter
        {
            get;
            set;
        }

        public int FileChunkSize
        {
            get;
            set;
        }

        public string PhysicalPath
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the PhysicalFileResponse class;
        /// </summary>
        public PhysicalFileResponse()
        {
            this.FileChunkSize = 64 * 1024;
        }

        public bool IsResponsible(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            return File.Exists(this.PhysicalPath);
        }

        public void Dispatch(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            var extension = Path.GetExtension(this.PhysicalPath);
            var mimeType = this.MimeTypeConverter.ConvertFromExtension(extension);
            
            // Set some header
            context.Response.AddHeader("Content-Type", mimeType);

            // File is not sent out at once, file is sent out by by chunks
            using (var responseStream = context.Response.OutputStream)
            {
                using (var stream = new FileStream(this.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read))                
                {
                    context.Response.ContentLength64 = stream.Length;

                    var restSize = stream.Length;
                    var byteBuffer = new byte[this.FileChunkSize];

                    while (restSize > 0)
                    {
                        var read = stream.Read(byteBuffer, 0, this.FileChunkSize);
                        responseStream.Write(byteBuffer, 0, read);

                        restSize -= read;
                    }
                }
            }
        }
    }
}
