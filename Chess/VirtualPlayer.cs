
namespace Chess
{
    internal class VirtualPlayer : ChessPlayer
    {
        public VirtualPlayer(Func<int[]> chooseMove) => ChooseMove = chooseMove;
    }
}
