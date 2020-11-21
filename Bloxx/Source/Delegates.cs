using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source
{
    public delegate void EmptyDel();
    public delegate void PacketDel(string message);
    public delegate void UIEvent(object s);
}
