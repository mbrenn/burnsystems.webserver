﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Responses;

namespace BurnSystems.WebServer.Dispatcher
{
    /// <summary>
    /// Dispatches the requests for the filesystem
    /// </summary>
    public class FileSystemDispatcher : BaseDispatcher
    {
        /// <summary>
        /// Gets or sets the physical root path
        /// </summary>
        public string PhysicalRootPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the webprefix
        /// </summary>
        public string WebPrefix
        {
            get;
            set;
        }

        public FileSystemDispatcher(Func<HttpListenerContext, bool> filter)
            : base(filter)
        {
        }

        public FileSystemDispatcher(Func<HttpListenerContext, bool> filter, string physicalRootPath)
            : this(filter)
        {
            this.PhysicalRootPath = physicalRootPath;
        }

        public FileSystemDispatcher(Func<HttpListenerContext, bool> filter, string physicalRootPath, string webPrefix)
            : this(filter, physicalRootPath)
        {
            this.WebPrefix = webPrefix;
        }

        public override void Dispatch(IActivates container, System.Net.HttpListenerContext context)
        {
            // Substring(1) to remove '/'
            string relativePath;
            var absolutePath = context.Request.Url.AbsolutePath;

            if (string.IsNullOrEmpty(this.WebPrefix))
            {
                relativePath = absolutePath.Substring(1);
            }
            else
            {
                if (absolutePath.StartsWith(this.WebPrefix))
                {
                    relativePath = absolutePath.Substring(this.WebPrefix.Length);
                }
                else
                {
                    var errorResponse = container.Create<ErrorResponse>();
                    errorResponse.Set(HttpStatusCode.NotFound);
                    errorResponse.Dispatch(container, context);
                    return;
                }
            }

            var physicalPath = Path.Combine(this.PhysicalRootPath, relativePath);

            if (relativePath.Contains("..") || Path.IsPathRooted(relativePath) || !physicalPath.StartsWith(this.PhysicalRootPath))
            {
                var errorResponse = container.Create<ErrorResponse>();
                errorResponse.Set(HttpStatusCode.Forbidden);
                errorResponse.Dispatch(container, context);

                return;
            }

            if (File.Exists(physicalPath))
            {
                var physicalFileResponse = container.Create<PhysicalFileResponse>();
                physicalFileResponse.PhysicalPath = physicalPath;
                physicalFileResponse.Dispatch(container, context);
            }
            else
            {
                var errorResponse = container.Create<ErrorResponse>();
                errorResponse.Set(HttpStatusCode.NotFound);
                errorResponse.Dispatch(container, context);
            }
        }
    }
}
