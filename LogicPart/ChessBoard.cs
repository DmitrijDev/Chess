using Chess.StringsUsing;

namespace Chess.LogicPart
{
    public class ChessBoard
    {
        private readonly Square[,] _board = new Square[8, 8];
        private readonly List<Move> _legalMoves = new();
        private int _legalMovesLastRenewMoment = -1;
        private readonly List<GamePosition> _positions = new();

        internal GameSide White { get; private set; }

        internal GameSide Black { get; private set; }

        internal GameSide MovingSide { get; private set; }

        public int MovesAfterCaptureOrPawnMoveCount { get; private set; }

        public int MovesCount { get; private set; }

        public GameStatus Status { get; private set; }

        public ChessBoard()
        {
            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    _board[i, j] = new Square(this, i, j);
                }
            }

            White = new GameSide(PieceColor.White, this);
            Black = new GameSide(PieceColor.Black, this);
            MovingSide = White;

            Status = GameStatus.ClearBoard;
        }

        public ChessBoard(IEnumerable<string> whiteMaterial, IEnumerable<string> whitePositions, IEnumerable<string> blackMaterial, IEnumerable<string> blackPositions,
            PieceColor movingSide) : this()
        {
            SetPosition(whiteMaterial, whitePositions, blackMaterial, blackPositions, movingSide);
        }

        internal Square this[int vertical, int horizontal]
        {
            get
            {
                if (vertical < 0 || horizontal < 0 || vertical >= 8 || horizontal >= 8)
                {
                    throw new IndexOutOfRangeException("Поля с указанными координатами не существует.");
                }

                return _board[vertical, horizontal];
            }
        }

        public void SetPosition(IEnumerable<string> whiteMaterial, IEnumerable<string> whitePositions, IEnumerable<string> blackMaterial, IEnumerable<string> blackPositions,
            PieceColor movingSide)

        {
            var whiteMaterialArray = whiteMaterial.Select(name => GetNewPiece(name, PieceColor.White)).ToArray();
            var whitePositionsArray = whitePositions.ToArray();

            var blackMaterialArray = blackMaterial.Select(name => GetNewPiece(name, PieceColor.Black)).ToArray();
            var blackPositionsArray = blackPositions.ToArray();

            if (whiteMaterialArray == null || whitePositionsArray == null || blackMaterialArray == null || blackPositionsArray == null)
            {
                throw new ArgumentException("Некорректные аргументы");
            }

            if (whiteMaterialArray.Length == 0 || whiteMaterialArray.Length != whitePositionsArray.Length)
            {
                throw new ArgumentException("Для белых должно быть указано равное положительное количество фигур и полей");
            }

            if (blackMaterialArray.Length == 0 || blackMaterialArray.Length != blackPositionsArray.Length)
            {
                throw new ArgumentException("Для черных должно быть указано равное положительное количество фигур и полей");
            }

            SetPosition(whiteMaterialArray.Concat(blackMaterialArray).ToArray(), whitePositionsArray.Concat(blackPositionsArray).ToArray(), movingSide);
        }

        private void SetPosition(ChessPiece[] material, string[] squareNames, PieceColor movingSideColor)
        {
            if (Status != GameStatus.ClearBoard)
            {
                Clear();
            }

            MovingSide = movingSideColor == PieceColor.White ? White : Black;

            for (var i = 0; i < material.Length; ++i)
            {
                var square = GetSquare(squareNames[i]);

                if (!square.IsEmpty)
                {
                    throw new ArgumentException("Для двух фигур указана одна и та же позиция");
                }

                material[i].Position = square;

                if (material[i].Color == PieceColor.White)
                {
                    White.Material.Add(material[i]);

                    if (material[i] is King)
                    {
                        White.King = (King)material[i];
                    }
                }
                else
                {
                    Black.Material.Add(material[i]);

                    if (material[i] is King)
                    {
                        Black.King = (King)material[i];
                    }
                }
            }

            _positions.Add(new GamePosition(this));

            if (!CheckPositionLegacy())
            {
                Status = GameStatus.IllegalPosition;
                return;
            }

            if (IsDrawByMaterial())
            {
                Status = GameStatus.Draw;
                return;
            }

            if (RenewLegalMoves().Count == 0)
            {
                if (White.King.IsMenaced())
                {
                    Status = GameStatus.BlackWin;
                    return;
                }

                if (Black.King.IsMenaced())
                {
                    Status = GameStatus.WhiteWin;
                    return;
                }

                Status = GameStatus.Draw;
                return;
            }

            Status = GameStatus.GameCanContinue;
        }

        private static ChessPiece GetNewPiece(string name, PieceColor color)
        {
            if (name == null)
            {
                throw new ArgumentException("Не указано имя фигуры");
            }

            var pieces = new ChessPiece[2] { new King(color), new Rook(color) }; // Других фигур пока нет.
            var trimmedName = SharedItems.RemoveSpacesAndToLower(name);

            foreach (var piece in pieces)
            {
                if (SharedItems.RemoveSpacesAndToLower(piece.EnglishName) == trimmedName || SharedItems.RemoveSpacesAndToLower(piece.RussianName) == trimmedName
                    || SharedItems.RemoveSpacesAndToLower(piece.ShortEnglishName) == trimmedName || SharedItems.RemoveSpacesAndToLower(piece.ShortRussianName) == trimmedName)
                {
                    return piece;
                }
            }

            throw new ArgumentException("Фигуры с указанным именем не существует");
        }

        public void Clear()
        {
            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    _board[i, j].SetDefaultValues();
                }
            }

            _legalMoves.Clear();
            _legalMovesLastRenewMoment = -1;
            _positions.Clear();

            White = new GameSide(PieceColor.White, this);
            Black = new GameSide(PieceColor.Black, this);

            MovesAfterCaptureOrPawnMoveCount = 0;
            MovesCount = 0;
            Status = GameStatus.ClearBoard;
        }

        private Square GetSquare(string squareName)
        {
            var coordinates = SharedItems.GetChessSquareCoordinates(squareName);
            return _board[coordinates[0], coordinates[1]];
        }

        public bool CheckPositionLegacy()
        {
            if (White.King == null || Black.King == null)
            {
                return false;
            }

            foreach (var piece in White.Material)
            {
                if (piece is King && piece != White.King)
                {
                    return false;
                }

                if (MovingSide == White && piece.GetAttackedSquares().Contains(Black.King.Position))
                {
                    return false;
                }
            }

            foreach (var piece in Black.Material)
            {
                if (piece is King && piece != Black.King)
                {
                    return false;
                }

                if (MovingSide == Black && piece.GetAttackedSquares().Contains(White.King.Position))
                {
                    Status = GameStatus.IllegalPosition;
                    return false;
                }
            }

            // Когда будут пешки - добавить еще проверку, не стоит ли какая-нибудь пешка на крайней горизонтали.

            return true;
        }

        private List<Move> RenewLegalMoves()
        {
            if (Status == GameStatus.IllegalPosition || _legalMovesLastRenewMoment == MovesCount)
            {
                return _legalMoves;
            }

            _legalMoves.Clear();

            foreach (var piece in MovingSide.Material)
            {
                foreach (var square in piece.GetLegalMoveSquares())
                {
                    _legalMoves.Add(new Move(piece, square));
                }
            }

            _legalMovesLastRenewMoment = MovesCount;
            return _legalMoves;
        }

        // Пока нет других фигур, кроме ладей и королей - ничья фиксируется только если остались одни короли
        public bool IsDrawByMaterial() => White.Material.Count + Black.Material.Count == 2;

        public bool IsDrawByThreeRepeats()
        {
            var newPosition = new GamePosition(this);

            if (_positions.Count < 4)
            {
                _positions.Add(newPosition);
                return false;
            }

            var repeatsCount = 1;

            foreach (var position in _positions)
            {
                if (position.Equals(newPosition))
                {
                    ++repeatsCount;
                }

                if (repeatsCount == 3)
                {
                    _positions.Add(newPosition);
                    return true;
                }
            }

            _positions.Add(newPosition);
            return false;
        }

        public void MakeMove(int[] coordinates)
        {
            if (coordinates.Length < 4)
            {
                throw new ArgumentException("Некорректный аргумент: неподходящий по длине массив");
            }

            MakeMove(coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
        }

        public void MakeMove(int startVertical, int startHorizontal, int destinationVertical, int destinationHorizontal) =>
            MakeMove(_board[startVertical, startHorizontal].ContainedPiece, _board[destinationVertical, destinationHorizontal]);

        private void MakeMove(ChessPiece piece, Square moveSquare)
        {
            if (piece == null)
            {
                throw new ArgumentException("В качестве начального поля хода указано пустое поле.");
            }

            if (piece.Color != MovingSide.Color)
            {
                throw new InvalidOperationException("Указанный ход невозможен т.к. очередь хода за другой стороной.");
            }

            foreach (var move in RenewLegalMoves())
            {
                if (move.MovingPiece == piece && move.MoveSquare == moveSquare)
                {
                    MakeMove(move);
                    return;
                }
            }

            throw new InvalidOperationException("Невозможный ход.");
        }

        private void MakeMove(Move move)
        {
            if (move.IsCapture) // Когда будут пешки - рассмотреть здесь еще взятие на проходе.
            {
                MovingSide.Enemy.Material.Remove(move.MoveSquare.ContainedPiece);
            }

            move.MovingPiece.Position = move.MoveSquare;  // Когда будут пешки - рассмотреть здесь еще превращение пешки.
            move.MovingPiece.HasMoved = true;

            if (move.IsCastleKingside)
            {
                var rook = _board[7, move.MovingPiece.Horizontal].ContainedPiece;
                rook.Position = _board[5, rook.Horizontal];
                rook.HasMoved = true;
            }

            if (move.IsCastleQueenside)
            {
                var rook = _board[0, move.MovingPiece.Horizontal].ContainedPiece;
                rook.Position = _board[3, rook.Horizontal];
                rook.HasMoved = true;
            }

            // Когда будут пешки - рассмотреть здесь: какие поля теперь доступны для взятия на проходе.

            MovingSide = MovingSide.Enemy;

            if (!move.IsCapture) // И если ход не пешкой - но пешек пока нет.
            {
                ++MovesAfterCaptureOrPawnMoveCount;
            }
            else
            {
                MovesAfterCaptureOrPawnMoveCount = 0;
                _positions.Clear();
            }

            ++MovesCount;

            if (RenewLegalMoves().Count == 0)
            {
                if (White.King.IsMenaced())
                {
                    Status = GameStatus.BlackWin;
                    return;
                }

                if (Black.King.IsMenaced())
                {
                    Status = GameStatus.WhiteWin;
                    return;
                }

                Status = GameStatus.Draw;
                return;
            }

            if (IsDrawByMaterial() || IsDrawByThreeRepeats() || MovesAfterCaptureOrPawnMoveCount == 100)
            {
                Status = GameStatus.Draw;
                _legalMoves.Clear();
            }
        }

        public List<int[]> GetLegalMoves() => RenewLegalMoves().Select(move => new int[4] { move.MovingPiece.Vertical, move.MovingPiece.Horizontal, move.MoveSquare.Vertical,
            move.MoveSquare.Horizontal}).ToList();

        public string WhiteMaterialToString() => string.Join(", ", White.Material.Select(piece => piece.ShortRussianName + piece.Position));

        public string BlackMaterialToString() => string.Join(", ", Black.Material.Select(piece => piece.ShortRussianName + piece.Position));

        public GamePosition CurrentPosition => _positions.Count > 0 ? _positions[_positions.Count - 1] : new GamePosition();

        public PieceColor MovingSideColor => MovingSide.Color;
    }
}