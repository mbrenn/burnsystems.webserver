﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BurnSystems.WebServer.MVC
{
    internal class WebMethodInfo
    {
        /// <summary>
        /// Gets or sets the name of the webinfo
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the method info being used
        /// </summary>
        public MethodInfo MethodInfo
        {
            get;
            set;
        }
    }
}
