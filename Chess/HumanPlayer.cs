
namespace Chess
{
    internal class HumanPlayer : ChessPlayer
    {
        public HumanPlayer() => ChooseMove = Strategies.ChooseMoveForHuman;
    }
}
