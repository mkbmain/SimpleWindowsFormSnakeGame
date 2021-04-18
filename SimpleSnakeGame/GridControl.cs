using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleSnakeGame
{
    class GridControl : Control
    {
        public readonly Label[][] Labels;

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
            return Labels[x][y];
        }

        public GridControl(int x, int y, Size wantedSize)
        {
            Labels = new Label[x][];
            this.Size = wantedSize;

            for (int i = 0; i < x; i++)
            {
                Labels[i] = new Label[y];
                for (int j = 0; j < y; j++)
                {
                    var label = new Label {BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White};
                    this.Controls.Add(label);
                    Labels[i][j] = label;
                }
            }

            Resize();
        }

        public new void Resize()
        {
            var size = CalculateSize(Labels.Length, Labels.First().Length, this.Size);
            for (int i = 0; i < Labels.Length; i++)
            {
                for (int j = 0; j < Labels.First().Length; j++)
                {
                    Labels[i][j].Size = size;
                    Labels[i][j].Left = size.Width * i;
                    Labels[i][j].Top = size.Height * j;
                }
            }
        }
    }
}