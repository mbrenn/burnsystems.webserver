using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Helper
{
    public class PostVariableReaderConfig
    {
        public int MaxPostLength
        {
            get;
            set;
        }

        public PostVariableReaderConfig()
        {
            this.MaxPostLength = 4 * 1024 * 1024;
        }
    }
}
