using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProj
{
    class Line
    {

        public Dot dot1;
        public Dot dot2;
        public Boolean isActive;

        public Line(Dot d1, Dot d2)
        {
            dot1 = d1;
            dot2 = d2;
            isActive = false;
        }
    }
}
