using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map.BlockBehaviour
{
    public abstract class TileBehaviour
    {
        public virtual void BlockUpdate(World world, int x, int y) { }
    }
}
