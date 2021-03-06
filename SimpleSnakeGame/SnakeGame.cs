using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleSnakeGame
{
    public class SnakeGame : Form
    {
        private const int XLenght = 32;
        private const int YLenght = 32;
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
        private int _score = 0;
        private const int Speed = 15;
        private readonly Color _food = Color.Brown;
        private readonly Timer _timer;

        private List<Point> _snake; // first elements tail , last elements head
        private Direction _direction = Direction.East;
        private Direction _lastDirection = Direction.East;
        private readonly System.ComponentModel.IContainer components = null;
        private readonly GridControl _gridControl;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SnakeGame());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        public SnakeGame()
        {
            components = new System.ComponentModel.Container();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = $"Snake Score:{_score}";
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            _gridControl = new GridControl(XLenght, YLenght, new Size(Width - 15, Height - 40));
            Controls.Add(_gridControl);
            KeyPreview = true;
            Resize += (sender, args) =>
            {
                _gridControl.Size = new Size(Width - 15, Height - 40);
                _gridControl.Resize();
            };

            _timer = new Timer(components) {Interval = 1000 / Speed};
            components.Add(_timer);
            _timer.Tick += GameLoop;
            NewGame();
        }

        private void NewGame()
        {
            foreach (var item in _gridControl.Labels.SelectMany(f => f.Select(t => t)))
            {
                item.BackColor = Color.White;
            }

            _score = 0;
            _snake = new List<Point> {new Point(13, 16), new Point(14, 16), new Point(15, 16), new Point(16, 16)};
            _direction = Direction.East;
            _lastDirection = Direction.East;
            foreach (var item in _snake)
            {
                _gridControl.GetCord(item).BackColor = Color.Black;
            }

            GenFood();
            _timer.Enabled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.P:
                    _timer.Enabled = !_timer.Enabled;
                    Text = _timer.Enabled ? Text.Replace(" {Paused}", "") : Text + " {Paused}";
                    break;
                case Keys.Right:
                    if (_lastDirection != Direction.West)
                    {
                        _direction = Direction.East;
                    }
                    break;
                case Keys.Up:
                    if (_lastDirection != Direction.South)
                    {
                        _direction = Direction.North;
                    }

                    break;
                case Keys.Left:
                    if (_lastDirection != Direction.East)
                    {
                        _direction = Direction.West;
                    }
                    break;
                case Keys.Down:
                    if (_lastDirection != Direction.North)
                    {
                        _direction = Direction.South;
                    }
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GenFood()
        {
            var freeLabels = _gridControl.Labels.SelectMany(t => t.Where(y => y.BackColor == Color.White)).ToArray();
            freeLabels[Random.Next(0, freeLabels.Length)].BackColor = _food;
        }


        private void GameLoop(object sender, EventArgs e)
        {
            var head = _snake.Last();
            var tail = _snake.First();

            var point = new Point(head.X, head.Y);
            _lastDirection = _direction;
            switch (_direction)
            {
                case Direction.North:
                    point.Y -= 1;
                    break;
                case Direction.South:
                    point.Y += 1;
                    break;
                case Direction.East:
                    point.X += 1;
                    break;
                case Direction.West:
                    point.X -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // check if out side bounds of map or we have hit our own tail\other part of snake
            if (point.X >= XLenght || point.Y >= YLenght || point.X < 0 || point.Y < 0 ||
                _snake.Skip(1).Contains(point))
            {
                _timer.Enabled = false;
                if (MessageBox.Show($"Game over your score is {_score}.{Environment.NewLine}New Game?", "Snake Game",
                        MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    NewGame();
                }

                return;
            }

            _snake.Add(point);

            var label = _gridControl.GetCord(point);

            if (label.BackColor != _food)
            {
                _snake = _snake.Skip(1).ToList();
            }
            else
            {
                _score += Speed;
                Text = $"Snake Score:{_score}";
                label.Text = "";
                GenFood();
            }

            label.BackColor = Color.Black;
            _gridControl.GetCord(tail).BackColor = Color.White;
        }
    }
}