using System;
using System.Globalization;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace E2Edit
{
    // <TomyLobo> <ITSBTH_> HP: http://img130.imageshack.us/img130/8066/e2edit20100413173043.png
    // <TomyLobo> my forst thought was "pong?" 
    internal class PongGame : UIElement
    {
        private readonly DispatcherTimer _timer;
        private Point _ballPos;
        private Vector _ballSpeed;
        private double _computerPaddlePos;
        private int _computerScore;
        private double _playerPaddlePos;
        private int _playerScore;

        public PongGame()
        {
            _playerPaddlePos = 50;
            _computerPaddlePos = 50;

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(10)};
            _timer.Tick += Update;
            _timer.Start();
        }

        public void SetupGame(double mul)
        {
            _ballPos = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
            _ballSpeed = new Vector(100 * mul, 0);
        }

        ~PongGame()
        {
            _timer.Stop();
        }

        private void Update(object sender, EventArgs e)
        {
            _ballPos += _ballSpeed/100;
            _playerPaddlePos = Mouse.GetPosition(this).Y;
            _computerPaddlePos -= (_computerPaddlePos - _ballPos.Y)/20;
            if (_ballPos.Y < 0 || _ballPos.Y > RenderSize.Height) _ballSpeed.Y *= -1;
            if (Math.Abs(_ballPos.X - 7.5) < 10 && Math.Abs(_playerPaddlePos - _ballPos.Y) < 40)
            {
                _ballSpeed.X *= -1.2;
                _ballSpeed.Y -= (_playerPaddlePos - _ballPos.Y);
                SystemSounds.Beep.Play();
            }
            else if (Math.Abs(_ballPos.X - (RenderSize.Width - 7.5)) < 10 &&
                     Math.Abs(_computerPaddlePos - _ballPos.Y) < 40)
            {
                _ballSpeed.X *= -1.2;
                _ballSpeed.Y -= (_computerPaddlePos - _ballPos.Y);
                SystemSounds.Beep.Play();
            }
            else if (_ballPos.X < 0)
            {
                _computerScore += 1;
                SetupGame(-1);
                SystemSounds.Exclamation.Play();
            }
            else if (_ballPos.X > RenderSize.Width)
            {
                _playerScore += 1;
                SetupGame(1);
                SystemSounds.Asterisk.Play();
            }
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Red, new Pen(), new Rect(5, _playerPaddlePos - 20, 10, 40));
            drawingContext.DrawRectangle(Brushes.Red, new Pen(),
                                         new Rect(RenderSize.Width - 7.5, _computerPaddlePos - 20, 10, 40));
            drawingContext.DrawEllipse(Brushes.WhiteSmoke, new Pen(), _ballPos, 5, 5);
            drawingContext.DrawText(
                new FormattedText(String.Format("{0} - {1}", _playerScore, _computerScore), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                  new Typeface("Courier New"), 12, Brushes.White), new Point(RenderSize.Width / 2, 10));
            base.OnRender(drawingContext);
        }
    }
}