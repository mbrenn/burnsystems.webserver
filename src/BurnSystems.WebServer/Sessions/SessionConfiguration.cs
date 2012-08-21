﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Sessions
{
    /// <summary>
    /// Stores the configuration for sessions
    /// </summary>
    public class SessionConfiguration
    {
        /// <summary>
        /// Stores the maximum age for cookies  
        /// </summary>
        private TimeSpan maximumAge = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Probability that collector will be called for each session query
        /// </summary>
        private double collectorProbability = 0.01;

        /// <summary>
        /// Gets or sets the maximum age of a session
        /// </summary>
        public TimeSpan MaximumAge
        {
            get { return this.maximumAge; }
            set { this.maximumAge = value; }
        }

        /// <summary>
        /// Gets or sets the probability that a garbage collection for old session will be executed
        /// </summary>
        public double CollectorProbability
        {
            get { return this.collectorProbability; }
            set { this.collectorProbability = value; }
        }
    }
}
