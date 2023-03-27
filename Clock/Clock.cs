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
        private PointF generalPoint;
        private int currentSecond;
        private float sin, cos;

        public Clock()
        {
            InitializeComponent();

            currentSecond = 0;
            sin = cos = 0;
            WindowState = FormWindowState.Maximized;
            widthOfSquare = 1000;
            graphics = CreateGraphics();
            pen = new Pen(Color.Black);
            generalPoint = new PointF(0, 0);
            radius = widthOfSquare / 2;
            halfClientWidth = ClientSize.Width / 2;
            halfClientHeight = ClientSize.Height / 2;
            timer1.Enabled = true;
            timer1.Interval = 1000;
            font = new Font("Arial", 24, FontStyle.Regular);
            square = new Rectangle(halfClientWidth - radius, halfClientHeight - radius, widthOfSquare, widthOfSquare);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawClock();
        }

        private void DrawClock()
        {
            for (int hour = 0; hour < 12; hour++)
            {
                CalculateNextPointToDraw(6, hour);
                DrawClockELement(hour == 0 ? "12" : hour.ToString());
            }

            graphics.DrawEllipse(pen, square);
        }

        private void DrawClockELement(string content)
        {
            SizeF textSize = graphics.MeasureString(content, font);
            generalPoint.X = generalPoint.X - (textSize.Width / 2);
            generalPoint.Y = generalPoint.Y - (textSize.Height / 2);

            PaddingClockElement(textSize);

            graphics.DrawString(content, font, Brushes.Black, generalPoint);
        }

        private void PaddingClockElement(SizeF textSize)
        {
            if (Math.Abs(sin) == 1)
            {
                generalPoint.Y += sin * (-1) * textSize.Height / 3; // 6h or 12h
            }
            else if (cos > 0)
            {
                generalPoint.X -= textSize.Width / 2; // 1h - 5h 
            }
            else if (cos < 0)
            {
                generalPoint.X += textSize.Width / 2; // 7h - 11h
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RunClock();
        }

        private void RunClock()
        {
            pen.Color = Color.White;
            graphics.DrawLine(pen, halfClientWidth, halfClientHeight, generalPoint.X, generalPoint.Y);

            CalculateNextPointToDraw(30, currentSecond);

            pen.Color = Color.Black;
            graphics.DrawLine(pen, halfClientWidth, halfClientHeight, generalPoint.X, generalPoint.Y);

            currentSecond++;
        }

        private void CalculateNextPointToDraw(int radianDivision, int timeFactor)
        {
            float radian = (float)(timeFactor * Math.PI / radianDivision + Math.PI / 2);

            cos = (float)Math.Cos(radian) * -1;
            sin = (float)Math.Sin(radian) * -1;

            generalPoint.X = cos * radius;
            generalPoint.Y = sin * radius;

            generalPoint.X += halfClientWidth;
            generalPoint.Y += halfClientHeight;
        }
    }
}