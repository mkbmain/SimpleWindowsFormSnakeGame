using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleSnakeGame
{
    class GridControl : Control
    {
        public Label[][] _labels;

        private static Size CalculateSize(int x, int y, Size windowSize)
        {
            return new Size(windowSize.Width / x, windowSize.Height / y);
        }

        public Label GetCord(Point point)
        {
            return GetCord(point.X, point.Y);
        }

        public Label GetCord(int x, int y)
        {
            return _labels[x][y];
        }

        public GridControl(int x, int y, Size wantedSize)
        {
            _labels = new Label[x][];
            this.Size = wantedSize;

            for (int i = 0; i < x; i++)
            {
                _labels[i] = new Label[y];
                for (int j = 0; j < y; j++)
                {
                    var label = new Label {BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White};
                    this.Controls.Add(label);
                    _labels[i][j] = label;
                }
            }

            Resize();
        }

        public new void Resize()
        {
            var size = CalculateSize(_labels.Length, _labels.First().Length, this.Size);
            for (int i = 0; i < _labels.Length; i++)
            {
                for (int j = 0; j < _labels.First().Length; j++)
                {
                    _labels[i][j].Size = size;
                    _labels[i][j].Left = size.Width * i;
                    _labels[i][j].Top = size.Height * j;
                }
            }
        }
    }
}