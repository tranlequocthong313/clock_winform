using System.Collections.Specialized;
using System.Windows.Forms.VisualStyles;

namespace Drawing
{
    public partial class Clock : Form
    {
        private readonly Graphics graphics;
        private readonly Pen pen;
        private readonly int widthOfSquare;
        private readonly int radius;
        private readonly int halfClientWidth, halfClientHeight;
        private readonly Font font;
        private readonly Rectangle square;
        private readonly PointF[] timePoints;
        private int currentSecond, currentMinute, currentHour;
        private float sin, cos;

        public Clock()
        {
            InitializeComponent();

            timePoints = new PointF[3] {
                new PointF(0 ,0) , // hour
                new PointF(0 ,0) , // minute
                new PointF(0 ,0) , // second
            };
            var dateTime = DateTime.Now.ToString("hh:mm:ss").Split(":");
            currentHour = int.Parse(dateTime[0]);
            currentMinute = int.Parse(dateTime[1]);
            currentSecond = int.Parse(dateTime[2]);
            sin = cos = 0;
            widthOfSquare = Math.Min(ClientSize.Width, ClientSize.Height) - 20;
            graphics = CreateGraphics();
            pen = new Pen(Color.Black);
            radius = widthOfSquare / 2;
            halfClientWidth = ClientSize.Width / 2;
            halfClientHeight = ClientSize.Height / 2;
            timer1.Enabled = true;
            timer1.Interval = 1000;
            font = new Font("Arial", widthOfSquare / 50, FontStyle.Regular);
            square = new Rectangle(halfClientWidth - radius, halfClientHeight - radius, widthOfSquare, widthOfSquare);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawClock();
        }

        private void DrawClock()
        {
            var timeElementPoint = new PointF(0, 0);
            for (int hour = 0; hour < 12; hour++)
            {
                CalculateNextPointToDraw(6, hour, ref timeElementPoint);
                DrawClockELement(hour == 0 ? "12" : hour.ToString(), ref timeElementPoint);
            }

            graphics.DrawEllipse(pen, square);
        }

        private void CalculateNextPointToDraw(int radianDivision, int timeFactor, ref PointF point)
        {
            float radian = (float)(timeFactor * Math.PI / radianDivision + Math.PI / 2);

            cos = (float)Math.Cos(radian) * (-1);
            sin = (float)Math.Sin(radian) * (-1);

            point.X = cos * radius;
            point.Y = sin * radius;

            point.X += halfClientWidth;
            point.Y += halfClientHeight;
        }

        private void DrawClockELement(string content, ref PointF point)
        {
            SizeF textSize = graphics.MeasureString(content, font);
            point.X -= (textSize.Width / 2);
            point.Y -= (textSize.Height / 2);

            PaddingClockElement(ref textSize, ref point);

            graphics.DrawString(content, font, Brushes.Black, point);
            CalculateNextPointToDraw(30, GetTimeFactorForHourStick(), ref timePoints[0]);
        }

        private void PaddingClockElement(ref SizeF textSize, ref PointF point)
        {
            if (Math.Abs(sin) == 1)
                point.Y += sin * (-1) * textSize.Height / 3; // 6h or 12h
            else if (cos > 0)
                point.X -= textSize.Width / 2; // 1h - 5h 
            else if (cos < 0)
                point.X += textSize.Width / 2; // 7h - 11h
        }

        private int GetTimeFactorForHourStick()
        {
            return (currentMinute / 15 + (currentHour * 5)) + 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RunClock();
        }

        private void RunClock()
        {
            DrawSec();
            DrawMin();
            DrawHour();
        }

        private void DrawSec()
        {
            DrawClockStick(Color.White, 2);
            CalculateNextPointToDraw(30, currentSecond, ref timePoints[2]);
            DrawClockStick(Color.Black, 2);
            currentSecond++;
        }

        private void DrawMin()
        {
            if (currentSecond == 60)
            {
                DrawClockStick(Color.White, 1);
                currentMinute++;
                currentSecond = 0;
            }
            CalculateNextPointToDraw(30, currentMinute, ref timePoints[1]);
            DrawClockStick(Color.Black, 1);
        }

        private void DrawHour()
        {
            if (currentMinute % 15 == 0 && currentSecond == 0)
            {
                DrawClockStick(Color.White, 0);
                CalculateNextPointToDraw(30, GetTimeFactorForHourStick(), ref timePoints[0]);
            }
            if (currentMinute == 60)
            {
                currentHour++;
                currentMinute = 0;
            }
            DrawClockStick(Color.Black, 0);
            if (currentHour == 12) currentHour = 0;
        }

        private void DrawClockStick(Color color, int index)
        {
            pen.Color = color;
            graphics.DrawLine(pen, halfClientWidth, halfClientHeight, timePoints[index].X, timePoints[index].Y);
        }
    }
}