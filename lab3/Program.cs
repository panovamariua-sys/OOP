using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            DbContext context = new DbContext();
            IAccountRepository accountRepository = new AccountRepository(context);
            IGameRepository gameRepository = new GameRepository(context);
            IAccountService accounts = new AccountService(accountRepository, gameRepository);
            IGameService games = new GameService(accountRepository, gameRepository);
            GameFactory factory = new GameFactory();

            Console.WriteLine("ПОЧАТКОВІ ГРАВЦІ:");
            PrintAccounts(accounts.ReadAccounts());

            List<GameAccount> initialPlayers = accounts.ReadAccounts();
            GameAccount player1 = accounts.ReadAccountById(initialPlayers[0].Id);
            GameAccount player2 = accounts.ReadAccountById(initialPlayers[1].Id);
            GameAccount player3 = new WinStreakAccount("Данило", 100);
            accounts.CreateAccount(player3);

            GameAccount temporaryPlayer = new StandardAccount("Новий гравець");
            accounts.CreateAccount(temporaryPlayer);
            accounts.RenameAccount(temporaryPlayer.Id, "Олена");
            Console.WriteLine("Оновлений гравець: " + accounts.ReadAccountById(temporaryPlayer.Id).UserName);
            accounts.DeleteAccount(temporaryPlayer.Id);
            Console.WriteLine("Тимчасовий акаунт видалено.\n");

            Console.WriteLine("СПИСОК ГРАВЦІВ ПІСЛЯ СТВОРЕННЯ:");
            PrintAccounts(accounts.ReadAccounts());

            games.CreateGame(player3.Id, player1.Id, factory.CreateStandardGame(25));
            games.CreateGame(player3.Id, player2.Id, factory.CreateStandardGame(40));
            games.CreateGame(player1.Id, player2.Id, factory.CreateTrainingGame());
            games.CreateGame(player2.Id, player1.Id, factory.CreateSoloRatingGame(30, player1));
            games.CreateGame(player1.Id, player3.Id, factory.CreateStandardGame(200));

            Console.WriteLine("ПЕРЕВІРКА ВІД'ЄМНОГО РЕЙТИНГУ:");
            try
            {
                games.CreateGame(player1.Id, player2.Id, factory.CreateStandardGame(-50));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message + "\n");
            }

            Console.WriteLine("УСІ ІГРИ (РАЗОМ ІЗ ПОЧАТКОВОЮ):");
            PrintGames(games.ReadGames(), accounts);

            Console.WriteLine("ІГРИ ГРАВЦЯ " + player3.UserName + ":");
            PrintGames(games.ReadPlayerGamesByPlayerId(player3.Id), accounts);

            Console.WriteLine("СТАТИСТИКА ГРАВЦІВ:");
            foreach (GameAccount account in accounts.ReadAccounts())
                Console.WriteLine(account.GetStats());

            Console.WriteLine("Натисніть Enter для завершення.");
            Console.ReadLine();
        }

        static void PrintAccounts(List<GameAccount> accounts)
        {
            Console.WriteLine($"{"ID",-5} | {"Ім'я",-14} | {"Тип акаунту",-18} | {"Рейтинг",-8} | Ігор");
            foreach (GameAccount account in accounts)
                Console.WriteLine($"{account.Id,-5} | {account.UserName,-14} | {account.GetType().Name,-18} | {account.CurrentRating,-8} | {account.GamesCount}");
            Console.WriteLine();
        }

        static void PrintGames(List<Game> games, IAccountService accounts)
        {
            Console.WriteLine($"{"ID гри",-8} | {"Тип гри",-16} | {"Переможець",-14} | {"Переможений",-14}");
            foreach (Game game in games)
            {
                string winnerName = accounts.ReadAccountById(game.WinnerId).UserName;
                string loserName = accounts.ReadAccountById(game.LoserId).UserName;
                Console.WriteLine($"{game.GameId,-8} | {game.GetType().Name,-16} | {winnerName,-14} | {loserName,-14}");
            }
            Console.WriteLine();
        }
    }
}
