using System.IO;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.WebServer.Helper;

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

        public string VirtualPath
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
            var info = this.MimeTypeConverter.ConvertFromExtension(extension);
                        
            // Set some header
            if (info != null)
            {
                if (info.FileRequestDispatcher != null)
                {
                    // Finds file request dispatcher
                    var fileRequestDispatcher = container.Create(info.FileRequestDispatcher) as IFileRequestDispatcher;
                    fileRequestDispatcher.PhysicalPath = this.PhysicalPath;
                    fileRequestDispatcher.VirtualPath = this.VirtualPath;

                    fileRequestDispatcher.Dispatch(container, context);
                }

                if (info.MimeType != null)
                {
                    context.Response.ContentType = info.MimeType;
                }

                if (info.ContentEncoding != null)
                {
                    context.Response.ContentEncoding = info.ContentEncoding;
                }
            }

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
