using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleSnakeGame
{
    public class SnakeGame : Form
    {
        private const int xLenght = 32;
        private const int yLenght = 32;
        private static Random _random = new Random(Guid.NewGuid().GetHashCode());
        private int _score = 0;
        private const int Speed = 15;
        private readonly Color _food = Color.Brown;
        private readonly Timer Timer;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SnakeGame());
        }

        private List<Point> Snake; // first elements tail , last elements head
        private Direction _direction = Direction.East;
        private Direction _lastDirection = Direction.East;
        private System.ComponentModel.IContainer components = null;
        private GridControl _gridControl;

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
            
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = $"Snake Score:{_score}";
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            _gridControl = new GridControl(xLenght, yLenght, new Size(this.Width - 15, this.Height - 40));
            this.Controls.Add(_gridControl);
            this.Resize += OnResize;
            this.KeyPreview = true;

            Timer = new Timer(components) {Interval = 1000 / Speed};
            components.Add(Timer);
            Timer.Tick += GameLoop;
            NewGame();
   
        }

        private void NewGame()
        {
            Snake = new List<Point> {new Point(13, 16), new Point(14, 16), new Point(15, 16), new Point(16, 16)};
            foreach (var item in Snake)
            {
                _gridControl.GetCord(item).BackColor = Color.Black;
            }
            GenFood();
            Timer.Enabled = true;
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.P:
                    Timer.Enabled = !Timer.Enabled;
                    if (!Timer.Enabled)
                    {
                        this.Text = this.Text + " {Paused}";
                    }
                    else
                    {
                        this.Text = this.Text.Replace(" {Paused}", "");
                    }

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
            var freeLabels = _gridControl._labels.SelectMany(t => t.Where(y => y.BackColor == Color.White)).ToArray();
            freeLabels[_random.Next(0, freeLabels.Length)].BackColor = _food;
        }


        private void GameLoop(object? sender, EventArgs e)
        {
            var head = Snake.Last();
            var tail = Snake.First();


            Point point = new Point(head.X, head.Y);
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


            if (point.X >= xLenght || point.Y >= yLenght || point.X < 0 || point.Y < 0 || Snake.Skip(1).Contains(point))
            {
                Timer.Enabled = false;
                MessageBox.Show($"end your score is {_score}");
                foreach (var item in   _gridControl._labels.SelectMany(f => f.Select(t=> t)))
                {
                    item.BackColor = Color.White;
                }

                NewGame();
              
                return;
            }

            Snake.Add(point);
            var label = _gridControl.GetCord(point);

            if (label.BackColor != _food)
            {
                Snake = Snake.Skip(1).ToList();
            }
            else
            {
                _score += Speed;
                this.Text = $"Snake Score:{_score}";
                label.Text = "";
                GenFood();
            }

            label.BackColor = Color.Black;
            _gridControl.GetCord(tail).BackColor = Color.White;
        }

        private void OnResize(object? sender, EventArgs e)
        {
            _gridControl.Size = new Size(this.Width - 15, this.Height - 40);
            _gridControl.Resize();
        }
    }
}