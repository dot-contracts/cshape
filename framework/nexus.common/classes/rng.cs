using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nexus.common
{
    public class RNG
    {
        private System.Random mRND;
        public RNG()
        {
            mRND = new System.Random(System.Convert.ToInt32(System.DateTime.Now.Ticks % System.Int32.MaxValue));
        }
        public int GetBall(int minBall, int maxBall)
        {
            return mRND.Next(minBall, maxBall);
        }
    }
}
