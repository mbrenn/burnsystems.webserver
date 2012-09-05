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

        public bool IsResponsible(ObjectActivation.IActivates container, ContextDispatchInformation info)
        {
            return File.Exists(this.PhysicalPath);
        }

        public void Dispatch(ObjectActivation.IActivates container, ContextDispatchInformation info)
        {
            var extension = Path.GetExtension(this.PhysicalPath);
            var mimeInfo = this.MimeTypeConverter.ConvertFromExtension(extension);
                        
            // Set some header
            if (mimeInfo != null)
            {
                if (mimeInfo.FileRequestDispatcher != null)
                {
                    // Finds file request dispatcher
                    var fileRequestDispatcher = container.Create(mimeInfo.FileRequestDispatcher) as IFileRequestDispatcher;
                    fileRequestDispatcher.PhysicalPath = this.PhysicalPath;
                    fileRequestDispatcher.VirtualPath = this.VirtualPath;

                    fileRequestDispatcher.Dispatch(container, info);
                }

                if (mimeInfo.MimeType != null)
                {
                    if (mimeInfo.CharsetEncoding != null)
                    {
                        info.Context.Response.ContentType = string.Format("{0}; charset={1}", mimeInfo.MimeType, mimeInfo.CharsetEncoding.WebName);                        
                    }
                    else
                    {
                        info.Context.Response.ContentType = mimeInfo.MimeType;
                    }
                }
            }

            // File is not sent out at once, file is sent out by by chunks
            using (var responseStream = info.Context.Response.OutputStream)
            {
                using (var stream = new FileStream(this.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read))                
                {
                    info.Context.Response.ContentLength64 = stream.Length;

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
