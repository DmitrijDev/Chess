using Chess.InterfacePart;

namespace Chess
{
    public static class Strategies
    {
        public static int[] ChooseMoveForHuman()
        {
            var legalMoves = Program.GameBoard.GetLegalMoves();

            for (; ; )
            {
                var userMove = UserInterface.ReadUserMove();

                if (!legalMoves.Any(move => move[0] == userMove[0] && move[1] == userMove[1] && move[2] == userMove[2] && move[3] == userMove[3]))
                {
                    try
                    {
                        Program.GameBoard.MakeMove(userMove);
                    }

                    catch (Exception exception)
                    {
                        UserInterface.ShowMessage(exception.Message);
                    }

                    continue;
                }

                return userMove;
            }
        }

        public static int[] ChooseMoveForVirtualFool()
        {
            var legalMoves = Program.GameBoard.GetLegalMoves();
            var moveIndex = new Random().Next(legalMoves.Count);
            return legalMoves[moveIndex];
        }
    }
}
