using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source
{
    class Settings
    {
        public static class Version
        {
            public const int majorVer = 1,
                             minorVer = 0,
                             bugVer = 0;

            public static readonly string versionString = majorVer.ToString() + "." +
                                                          minorVer.ToString() + "." +
                                                          bugVer.ToString();
        }
    }
}
