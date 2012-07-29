using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BurnSystems.WebServer.Dispatcher
{
    /// <summary>
    /// Dispatches the request by url. 
    /// Uses the same IRequestDispatcher than the listener, but performs a caching for found url
    /// </summary>
    public class UrlDispatcher : BaseDispatcher
    {
        /// <summary>
        /// Cache for already found urls
        /// </summary>
        private Dictionary<string, IRequestDispatcher> cache = new Dictionary<string, IRequestDispatcher>();

        /// <summary>
        /// List of dispatchers
        /// </summary>
        private List<IRequestDispatcher> dispatchers = new List<IRequestDispatcher>();

        /// <summary>
        /// Initializes a new instance of the UrlDispatcher class
        /// </summary>
        /// <param name="filter">Filter to be used</param>
        public UrlDispatcher(Func<HttpListenerContext, bool> filter)
            : base(filter)
        {
        }

        public void Add(IRequestDispatcher dispatcher)
        {
            this.dispatchers.Add(dispatcher);
        }

        public override void Dispatch(System.Net.HttpListenerContext context)
        {
            var url = context.Request.Url.AbsolutePath;

            // Found
            IRequestDispatcher foundRequest;
            if (this.cache.TryGetValue(url, out foundRequest))
            {
                if (foundRequest != null)
                {
                    foundRequest.Dispatch(context);
                }
            }

            // Not found, go through list
            foundRequest = this.dispatchers.Where(x => x.IsResponsible(context)).FirstOrDefault();
            this.cache[url] = foundRequest;
            if (foundRequest == null)
            {
                // Nobody... But this information is also cacheable
                this.cache[url] = null;
            }
            else
            {
                foundRequest.Dispatch(context);
            }
        }
    }
}
