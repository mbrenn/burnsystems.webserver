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

        public string PhysicalPath
        {
            get;
            set;
        }

        public bool IsResponsible(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            return File.Exists(this.PhysicalPath);
        }

        public void Dispatch(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            var extension = Path.GetExtension(this.PhysicalPath);
            var mimeType = this.MimeTypeConverter.ConvertFromExtension(extension);

            throw new NotImplementedException();
        }
    }
}
