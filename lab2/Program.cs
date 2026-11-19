using System;
using System.Text;
namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            GameAccount player1 = new StandardAccount("Макар", 100);
            GameAccount player2 = new HalfLossAccount("Анелія", 100);
            GameAccount player3 = new WinStreakAccount("Данило", 100);

            GameFactory factory = new GameFactory();

            PlayMatch(winner: player3, loser: player1, game: factory.CreateStandardGame(25));
            PlayMatch(winner: player3, loser: player2, game: factory.CreateStandardGame(40));

            PlayMatch(winner: player1, loser: player2, game: factory.CreateTrainingGame());

            PlayMatch(winner: player2, loser: player1, game: factory.CreateSoloRatingGame(30, ratedPlayer: player1));

            PlayMatch(winner: player1, loser: player3, game: factory.CreateStandardGame(200));

            Console.WriteLine("Перевірка захисту від створення гри на від'ємний рейтинг:");
            try
            {
                Game invalidGame = factory.CreateStandardGame(-50);
                PlayMatch(winner: player1, loser: player2, game: invalidGame);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Помилку успішно перехоплено]: {ex.Message}\n");
            }

            Console.WriteLine(player1.GetStats());
            Console.WriteLine(player2.GetStats());
            Console.WriteLine(player3.GetStats());

            Console.ReadLine();
        }

        static void PlayMatch(GameAccount winner, GameAccount loser, Game game)
        {
            winner.WinGame(loser.UserName, game);
            loser.LoseGame(winner.UserName, game);
        }
    }
}