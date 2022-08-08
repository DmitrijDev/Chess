using System.Windows.Forms;

namespace Temp
{
    public partial class ChessGameForm : Form
    {
        private Button[,] _buttons = new SquareButton[8, 8];
        private List<int> _clickedButtons = new();

        public int ButtonSize { get; private set; } = Screen.PrimaryScreen.WorkingArea.Height / 16;

        public Color LightSquaresColor { get; set; } = Color.Gold;

        public Color DarkSquaresColor { get; set; } = Color.Chocolate;

        public ChessGameForm()
        {
            InitializeComponent();
            AutoSize = true;
            SetButtons();
        }

        private void ChessGameForm_Load(object sender, EventArgs e)
        {
        }

        public void SetButtons()
        {
            MinimumSize = new Size(0, 0);
            MaximumSize = new Size(int.MaxValue, int.MaxValue);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var boardSize = _buttons.GetLength(0);
            var shift = ButtonSize / 2;
            var buttonColor = LightSquaresColor;
            var buttonX = shift;
            var buttonY = shift;

            for (var i = 0; i < boardSize; ++i)
            {
                for (var j = boardSize - 1; j >= 0; --j)
                {
                    // Если кнопки ранее уже созданы, а теперь мы хотим изменить размер полей, то старые кнопки нужно удалить.
                    if (_buttons[i, j] != null)
                    {
                        Controls.Remove(_buttons[i, j]);
                    }

                    var newButton = new SquareButton(this, i, j)
                    {
                        BackColor = buttonColor,
                        Location = new Point(buttonX, buttonY)
                    };

                    _buttons[i, j] = newButton;
                    Controls.Add(newButton);

                    buttonColor = buttonColor == LightSquaresColor ? DarkSquaresColor : LightSquaresColor;
                    buttonY += ButtonSize;
                }

                buttonColor = buttonColor == LightSquaresColor ? DarkSquaresColor : LightSquaresColor;
                buttonX += ButtonSize;
                buttonY = shift;
            }

            AutoSizeMode = AutoSizeMode.GrowOnly;
            Height += shift;
            Width += shift;
            MinimumSize = new Size(Width, Height);
            MaximumSize = MinimumSize;
        }

        public void SetSizeAndColors(int buttonSize, Color lightSquaresColor, Color darkSquaresColor)
        {
            ButtonSize = buttonSize;
            LightSquaresColor = lightSquaresColor;
            DarkSquaresColor = darkSquaresColor;
            SetButtons();
        }

        public void AddClick(int x, int y)
        {
            _clickedButtons.Add(x);
            _clickedButtons.Add(y);
        }
    }
}