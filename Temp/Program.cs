namespace Temp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new ChessGameForm());

            var board = new ChessGameForm();
            board.SetSizeAndColors(100, Color.Yellow, Color.Brown);
            Application.Run(board);

            var board1 = new ChessGameForm();
            board1.SetSizeAndColors(50, Color.Silver, Color.DarkSlateGray);
            Application.Run(board1);
        }
    }
}