using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public class Token
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
