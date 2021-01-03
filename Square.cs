using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProj
{
    public enum SquareColor {Blank, Red, Green}

    class Square
    {
        public Line LeftLine { get; private set;}
        public Line RightLine { get; private set; }
        public Line TopLine { get; private set; }
        public Line BottomLine { get; private set; }
        public SquareColor Fill { get; set; }

        public Dot GetUpperLeftDot()
        {
            return LeftLine.dot1;
        }

        public bool AllActive()
        {
            if (LeftLine.isActive && RightLine.isActive && TopLine.isActive && BottomLine.isActive)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool ContainsLine(Line ln)
        {
            if (ln.Equals(LeftLine) || ln.Equals(RightLine) || ln.Equals(TopLine) || ln.Equals(BottomLine)) {
                return true;
            } else
            {
                return false;
            }
        }

        public Square(Line t, Line b, Line l, Line r)
        {
            TopLine = t;
            BottomLine = b;
            LeftLine = l;
            RightLine = r;
            Fill = SquareColor.Blank;
        }
    }
}
