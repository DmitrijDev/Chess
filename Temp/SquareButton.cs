using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp
{
    internal class SquareButton: Button
    {
        private readonly ChessGameForm _board;
        private readonly int _x;
        private readonly int _y;

        public SquareButton(ChessGameForm board, int x, int y)
        {
            _board = board;
            _x = x;
            _y = y;
            Height = _board.ButtonSize;
            Width = _board.ButtonSize;
            FlatStyle = FlatStyle.Flat;            
            Click += new EventHandler(ClickSquare);
        }

        private void ClickSquare(object sender, EventArgs e) => _board.AddClick(_x, _y);
    }
}
