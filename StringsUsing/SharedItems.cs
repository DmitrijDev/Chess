using System.Text;

namespace Chess.StringsUsing
{
    public static class SharedItems
    {
        public static string RemoveSpacesAndToLower(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var result = new StringBuilder();

            foreach (var c in s)
            {
                if (c != ' ')
                {
                    result.Append(char.ToLower(c));
                }
            }

            return result.ToString();
        }

        public static int[] GetChessSquareCoordinates(string squareName)
        {
            if (squareName == null)
            {
                throw new ArgumentNullException("Не указано имя поля.");
            }

            var trimmedName = RemoveSpacesAndToLower(squareName);

            if (trimmedName.Length != 2)
            {
                throw new ArgumentException("Поля с указанным именем на доске не существует.");
            }

            var vertical = -1;
            const string verticalNames = "abcdefgh";

            for (var i = 0; i < verticalNames.Length; ++i)
            {
                if (verticalNames[i] == trimmedName[0])
                {
                    vertical = i;
                    break;
                }
            }

            if (vertical == -1)
            {
                throw new ArgumentException("Поля с указанным именем на доске не существует.");
            }

            var horizontal = -1;
            const string horizontalIndices = "12345678";

            for (var i = 0; i < horizontalIndices.Length; ++i)
            {
                if (horizontalIndices[i] == trimmedName[1])
                {
                    horizontal = i;
                    break;
                }
            }

            if (horizontal == -1)
            {
                throw new ArgumentException("Поля с указанным именем на доске не существует.");
            }

            return new int[2] { vertical, horizontal };
        }

        public static string WritePosition(string whiteMaterial, string blackMaterial, int movingSideColor, int gameStatus)
        {
            if (gameStatus == 4)
            {
                return "Доска пуста.";
            }

            if (gameStatus == 5)
            {
                return "Невозможная начальная позиция.";
            }

            var result = new StringBuilder("Белые: ").Append(whiteMaterial).Append(". Черные: ").Append(blackMaterial).Append('.').Append(Environment.NewLine);

            if (gameStatus == 3)
            {
                result.Append(movingSideColor == 0 ? "Ход белых." : "Ход черных.");
            }
            else
            {
                if (gameStatus == 2)
                {
                    result.Append("Ничья.");
                }
                else
                {
                    result.Append(gameStatus == 0 ? "Мат черному королю." : "Мат белому королю.");
                }
            }

            return result.ToString();
        }
    }
}
