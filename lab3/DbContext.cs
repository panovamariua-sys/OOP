using System.Collections.Generic;

namespace Lab3
{
    public class DbContext
    {
        public List<GameAccount> Accounts { get; } = new List<GameAccount>();
        public List<Game> Games { get; } = new List<Game>();

        public DbContext()
        {
            GameAccount player1 = new StandardAccount("Макар", 100);
            GameAccount player2 = new HalfLossAccount("Анелія", 100);
            Accounts.Add(player1);
            Accounts.Add(player2);

            Game game = new TrainingGame();
            player1.WinGame(player2.UserName, game);
            player2.LoseGame(player1.UserName, game);
            game.SetResult(player1.Id, player2.Id);
            Games.Add(game);
        }
    }
}
