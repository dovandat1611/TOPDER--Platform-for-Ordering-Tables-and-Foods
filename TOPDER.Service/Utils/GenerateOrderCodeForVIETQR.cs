using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Utils
{
    public class GenerateOrderCodeForVIETQR
    {
        public static long GenerateOrderCode(int id, int prefix)
        {
            return prefix * 1_000_000_000L + id;
        }
    }
}
