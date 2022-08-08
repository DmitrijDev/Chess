using Chess.StringsUsing;

namespace Chess.InterfacePart
{

    public static class UserInterface
    {
        public static void ShowMessage(string message) => Console.WriteLine(message);

        public static void ShowPosition(string whiteMaterial, string blackMaterial, int movingSideColor, int gameStatus)
        {
            Console.WriteLine(SharedItems.WritePosition(whiteMaterial, blackMaterial, movingSideColor, gameStatus));
        }

        public static int[] ReadUserMove()
        {
            var userMove = new int[4];

            for (; ; )
            {
                var squareName = Console.ReadLine();                

                try
                {
                    var squareCoordinates = SharedItems.GetChessSquareCoordinates(squareName);
                    userMove[0] = squareCoordinates[0];
                    userMove[1] = squareCoordinates[1];
                }

                catch (Exception exception)
                {
                    ShowMessage(exception.Message);
                    continue;
                }

                break;
            }

            for (; ; )
            {
                var squareName = Console.ReadLine();

                try
                {
                    var squareCoordinates = SharedItems.GetChessSquareCoordinates(squareName);
                    userMove[2] = squareCoordinates[0];
                    userMove[3] = squareCoordinates[1];
                }

                catch (Exception exception)
                {
                    ShowMessage(exception.Message);
                    continue;
                }

                break;
            }

            return userMove;
        }

        public static void Wait() => Console.ReadKey();
    }
}
