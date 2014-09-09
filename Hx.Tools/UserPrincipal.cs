using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Web.Security;

namespace Hx.Tools
{
    public class UserPrincipal : MarshalByRefObject, IPrincipal
    {
        private IIdentity _identity;
        private int _userID;
        private string _userName;
        private int[] _roleIDs;
        private bool _isAdmin;


        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }


        public int[] Roles
        {
            get { return _roleIDs; }
            set { _roleIDs = value; }
        }

        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        public UserPrincipal()
        {
        }

        public UserPrincipal(IIdentity identity)
        {
            this._identity = identity;
        }

        public static UserPrincipal GetPrincipal(FormsAuthenticationTicket ticket)
        {
            UserPrincipal pal = null;
            try
            {
                pal = (UserPrincipal)Serializer.ConvertToObject(ticket.UserData);
                pal.Identity = new FormsIdentity(ticket);
            }
            catch
            {
                pal = null;
            }
            return pal;

        }

        public static UserPrincipal GetPrincipal(string value)
        {
            UserPrincipal pal = null;
            try
            {
                pal = Serializer.ConvertToObject(value) as UserPrincipal;
                pal.Identity = new CustomIdentity(pal._userName);
            }
            catch
            {
                pal = null;
            }
            return pal;

        }

        public bool IsInRole(string role)
        {
            return true;
        }

        public bool IsInRole(int roleid)
        {
            if (_roleIDs != null && _roleIDs.Length > 0)
            {
                foreach (int i in _roleIDs)
                {
                    if (roleid == i)
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        public string SerializeToString()
        {
            return Serializer.ConvertToString(this);
        }

        public IIdentity Identity
        {
            get
            {
                return this._identity;
            }
            set
            {
                this._identity = value;
            }
        }
    }
}
