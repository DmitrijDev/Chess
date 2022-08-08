
namespace Chess
{
    internal abstract class ChessPlayer
    {
        public Func<int[]> ChooseMove { get; protected set; }
    }
}
