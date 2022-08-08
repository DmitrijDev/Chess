using Chess.InterfacePart;
using Chess.LogicPart;

namespace Chess
{
    public class Program
    {
        public static ChessBoard GameBoard { get; private set; } = new ChessBoard();

        internal static ChessPlayer WhitePlayer { get; private set; } = new HumanPlayer();

        internal static ChessPlayer BlackPlayer { get; private set; } = new VirtualPlayer(Strategies.ChooseMoveForVirtualFool);

        public static void Play()
        {
            for (; ; )
            {
                UserInterface.ShowPosition(GameBoard.WhiteMaterialToString(), GameBoard.BlackMaterialToString(), (int)GameBoard.MovingSideColor, (int)GameBoard.Status);

                if (GameBoard.Status != GameStatus.GameCanContinue)
                {
                    UserInterface.Wait();
                    return;
                }

                var move = GameBoard.MovingSideColor == PieceColor.White ? WhitePlayer.ChooseMove() : BlackPlayer.ChooseMove();
                GameBoard.MakeMove(move);
            }
        }

        static void Main()
        {
            var whiteMaterial = new string[3] { "King" , "ROOK", "rook" };
            var whitePositions = new string[3] { "e1" , "a1", "h1" };

            var blackMaterial = new string[3] { "Король " , " ладья ", "Л" };
            var blackPositions = new string[3] { "e8" , "a8", "h8" };

            GameBoard.SetPosition(whiteMaterial, whitePositions, blackMaterial, blackPositions, PieceColor.White);
            Play();
        }

        public static GamePosition CurrentPosition => GameBoard.CurrentPosition;
    }
}