using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Hx.Tools
{
    [Serializable]
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name)
        {
            _name = name;
        }

        public string AuthenticationType
        {
            get { return "custom"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
        }
    }
}
