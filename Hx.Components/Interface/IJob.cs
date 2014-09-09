using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Hx.Components.Interface
{
    public interface IJob
    {
        void Execute(XmlNode node);
    }
}
