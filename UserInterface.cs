using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProj
{
    public partial class UserInterface : Form
    {
        private Dot[,] _dots; // 5x5 array of dots on the form
        private List<Line> _lines; // all potential lines connecting two dots in the game (???)
        private Square[,] _squares; // 4x4 array of all potential filled squares
        private int _redTotal; // red's current score
        private int _greenTotal; // Green's current score
        private string _turn = "Red"; // whose turn it is
        private bool _gameOver; // whether the current game is over
        private const int SIZE = 5; // dimensions of the dots grid
        private const int DIAM = 15; // diameter of a drawn dot
        private const int LEN = 65; // length of a drawn square
        private const int HEIGHT = 70; // height of a drawn square
        private const int OFFSET = 7; // an offset amount to center drawn shapes

        public UserInterface()
        {
            InitializeComponent();
            _dots = new Dot[5, 5];
            _lines = new List<Line>();
            _squares = new Square[4, 4];
            uxFrame.Image = null;
            uxFrame.BackColor = Color.White;

            // Call create methods 
            CreateDots();
            CreateLinesAndSquares();
            uxFrame.Paint += new PaintEventHandler(DrawGame);
        }

        private void CreateDots()
        {
            for (int i = 0; i < 5; i++) // Loop through rows
            {
                for (int j = 0; j < 5; j ++)
                {
                    int xCoord = 15 + j * LEN;
                    int yCoord = 15 + i * LEN;
                    _dots[i, j] = new Dot(xCoord, yCoord);
                }
            }
        }

        // dot1 is leftmost or topmost dot on the line
        private void CreateLinesAndSquares()
        {
            Line topBorder;
            Line bottomBorder;
            Line leftBorder;
            Line rightBorder;
            for (int i = 0; i < 4; i++) // X row
            {
                for (int j = 0; j < 4; j++) // Y row
                {
                    // Is there a square above us?
                    if (!(j == 0))
                    {
                        topBorder = _squares[i, j - 1].BottomLine;
                    } else
                    {
                        Dot lDot = _dots[i, j];
                        Dot rDot = _dots[i + 1, j];
                        topBorder = new Line(lDot, rDot);
                        _lines.Add(topBorder);
                    }
                    
                    // Is there a square to the left of us?
                    if (!(i == 0))
                    {
                        leftBorder = _squares[i - 1, j].RightLine;
                    } else
                    {
                        Dot tDot = _dots[i, j];
                        Dot bDot = _dots[i, j + 1];
                        leftBorder = new Line(tDot, bDot);
                        _lines.Add(leftBorder);
                    }

                    // Make bottom and right lines
                    bottomBorder = new Line(_dots[i, j + 1], _dots[i + 1, j + 1]);
                    _lines.Add(bottomBorder);
                    rightBorder = new Line(_dots[i + 1, j], _dots[i + 1, j + 1]);
                    _lines.Add(rightBorder);
                    Square s = new Square(topBorder, bottomBorder, leftBorder, rightBorder);
                    _squares[i, j] = s;

                }
            }
        }

        private void SetupGame()
        {
            _redTotal = 0;
            _greenTotal = 0;
            _turn = "Red";
            _gameOver = false;

            // Set lines to inactive
            foreach (Line l in _lines)
            {
                l.isActive = false;
            }
            //Set square fills to blank
            foreach (Square s in _squares)
            {
                s.Fill = SquareColor.Blank;
            }
            uxFrame.Image = null;
            uxFrame.BackColor = Color.White;
        }



        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Loop through squares array and draw each one
            for (int i = 0; i  < 4; i++) // X row
            {
                for (int j = 0; j < 4; j++) // Y row
                {
                    // Get dot coords
                    int upperLeftX = _squares[i, j].GetUpperLeftDot().x;
                    int upperLeftY = _squares[i, j].GetUpperLeftDot().y;
                    if (_squares[i,j].Fill == SquareColor.Blank)
                    {
                        g.FillRectangle(new SolidBrush(Color.White), upperLeftX + OFFSET, upperLeftY + OFFSET, LEN, LEN);
                    } else if (_squares[i,j].Fill == SquareColor.Red)
                    {
                        g.FillRectangle(new SolidBrush(Color.Red), upperLeftX + OFFSET, upperLeftY + OFFSET, LEN, LEN);
                    } else if (_squares[i,j].Fill == SquareColor.Green)
                    {
                        g.FillRectangle(new SolidBrush(Color.Green), upperLeftX + OFFSET, upperLeftY + OFFSET, LEN, LEN);
                    }
                }
            }
            // Loop through dots array and draw each one
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Dot thisDot = _dots[i, j];
                    g.FillEllipse(new SolidBrush(Color.Magenta), thisDot.x, thisDot.y, DIAM, DIAM);
                }
            }
            // Loop through lines array and draw each one
            foreach (Line l in _lines)
            {
                if (l.isActive)
                {
                    g.DrawLine(Pens.Gray, l.dot1.x + OFFSET, l.dot1.y + OFFSET, l.dot2.x + OFFSET, l.dot2.y + OFFSET);
                }
            }

            // Update score label
            int totalGreen = 0;
            int totalRed = 0;
            foreach (Square s in _squares)
            {
                if (s.Fill == SquareColor.Red)
                {
                    totalRed++;
                } else if (s.Fill == SquareColor.Green)
                {
                    totalGreen++;
                }
            }
            _greenTotal = totalGreen;
            _redTotal = totalRed;
            string scoreText = "Score:" + Environment.NewLine + "Red: " + _redTotal + Environment.NewLine + "Green: " + _greenTotal;
            uxScoreLabel.Text = scoreText;

            // Update turn label
            uxTurnLabel.Text = _turn + "'s turn!";
        
        }

        private Line GetClosest(int clickX, int clickY)
        {
            double minDist = Double.MaxValue;
            Line closest = null;
            // Loop through lines and find the closest one
            foreach (Line l in _lines)
            {
                double x1 = (double) l.dot1.x;
                double x2 = (double) l.dot2.x;
                double y1 = (double) l.dot1.y;
                double y2 = (double) l.dot2.y;
                double dist1 = Math.Sqrt(Math.Pow(clickY - y1, 2.0) + Math.Pow(clickX - x1, 2));
                double dist2 = Math.Sqrt(Math.Pow(clickY - y2, 2.0) + Math.Pow(clickX - x2, 2));
                double avg = (dist1 + dist2) / 2.0;
                if (avg < minDist)
                {
                    if ((clickX - x1 <= 15) || (clickX - x2 <= 15) || (clickY - y1 <= 15) || (clickY - y2 <= 15))
                    {
                        minDist = avg;
                        closest = l;
                    }
                }

            }
            return closest;
        }

        private void FillOne(Line placed)
        {
            SquareColor sc = getColorFromString(_turn);
            foreach (Square s in _squares)
            {
                if (s.AllActive() && s.Fill == SquareColor.Blank)
                {
                    s.Fill = sc;
                    break; 
                }
                // Extra credit
                fillEnclosed(placed);


            }
        }

        private string CheckGameOver()
        {
            bool anyBlank = false; // Make true if we find a blank square
            int totalGreen = 0;
            int totalRed = 0;
            foreach (Square s in _squares)
            {
                if (s.Fill == SquareColor.Blank)
                {
                    anyBlank = true;
                } else if (s.Fill == SquareColor.Green)
                {
                    totalGreen++;
                } else if (s.Fill == SquareColor.Red)
                {
                    totalRed++;
                }
            }
            if (!anyBlank)
            {
                _gameOver = true;
                if (totalRed == totalGreen)
                {
                    return "Tie game!";
                } else
                {
                    return (totalGreen > totalRed) ? "Green wins!" : "Red wins!";
                }
            } else
            {
                return null;
            }

        }




        private void uxFrame_MouseUp(object sender, MouseEventArgs e)
        {
            Line closestLine = GetClosest(e.X, e.Y);
            if (_gameOver)
            {
                MessageBox.Show("The game has ended, please start a new game.", "Dots Game");

            } else if (closestLine != null && closestLine.isActive)
            {
                MessageBox.Show("This spot is taken!", "Dots Game");
            } else if (closestLine != null)
            {
                closestLine.isActive = true;
                FillOne(closestLine);
                uxFrame.Refresh();
                // Check game over
                if (CheckGameOver() != null)
                {
                    MessageBox.Show(CheckGameOver(), "Dots Game");
                } else
                {
                    switchTurn();
                    uxTurnLabel.Text = _turn + "'s turn";
                }
            }
        }

        private void uxNewGameButton_Click(object sender, EventArgs e)
        {
            SetupGame();
            uxFrame.Refresh();
        }

        // Private helper methods
        private void fillEnclosed(Line placed)
        {
            // Need a list of all lines that are a part of the enclosing space
            List<Line> poly = new List<Line>();
            // Get the starting dot, we want the line whose dot2 is this
            Dot start = placed.dot1;
            foreach (Line l in _lines)
            {

            }
        }

        private List<Line> traverse(Dot dot)
        {
            List<Line> dots = new List<Line>();
            foreach (Line l in _lines)
            {
                List<Dot> temp = new List<Dot>();
                if (l.dot1 == dot)
                {
                    temp.Add(l.dot1);
                    traverse(l.dot2);
                }
                
                

            }
        }

        private SquareColor getColorFromString(String str)
        {
            if (str.ToLower().Equals("red"))
            {
                return SquareColor.Red;
            }
            else if (str.ToLower().Equals("green"))
            {
                return SquareColor.Green;
            }
            else
            {
                return SquareColor.Blank;
            }
        }

        private void switchTurn()
        {
            if (_turn.ToLower() == "red")
            {
                _turn = "Green";
            } else
            {
                _turn = "Red";
            }
        }


    }
}
