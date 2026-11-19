using System;
using System.Collections.Generic;
using System.Text;
namespace Lab3
{
    public class GameAccount
    {
        private static int _accountIdSeed = 1;

        public int Id { get; }
        public string UserName { get; set; }

        private int _currentRating;
        public int CurrentRating
        {
            get => _currentRating;
            protected set
            {
                _currentRating = value < 1 ? 1 : value;
            }
        }

        public int GamesCount => _gamesHistory.Count;

        private readonly List<GameHistoryRecord> _gamesHistory = new List<GameHistoryRecord>();

        public GameAccount(string userName, int initialRating = 100)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Ім'я користувача не може бути порожнім.", nameof(userName));
            }

            if (initialRating < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialRating), "Початковий рейтинг не може бути менше 1.");
            }

            Id = _accountIdSeed++;
            UserName = userName;
            CurrentRating = initialRating;
        }

        protected virtual int CalculateWinPoints(int rating)
        {
            return rating;
        }

        protected virtual int CalculateLosePoints(int rating)
        {
            return rating;
        }

        public void WinGame(string opponentName, Game game)
        {
            if (string.IsNullOrWhiteSpace(opponentName))
            {
                throw new ArgumentException("Ім'я опонента не може бути порожнім.", nameof(opponentName));
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Об'єкт гри не може бути null.");
            }

            int playRating = game.GetPlayRating(this);
            int points = CalculateWinPoints(playRating);

            CurrentRating += points;
            var record = new GameHistoryRecord(game.GameId, opponentName, isWin: true, rating: points, gameType: game.GetType().Name);
            _gamesHistory.Add(record);
        }

        public void LoseGame(string opponentName, Game game)
        {
            if (string.IsNullOrWhiteSpace(opponentName))
            {
                throw new ArgumentException("Ім'я опонента не може бути порожнім.", nameof(opponentName));
            }

            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Об'єкт гри не може бути null.");
            }

            int playRating = game.GetPlayRating(this);
            int points = CalculateLosePoints(playRating);

            CurrentRating -= points;
            var record = new GameHistoryRecord(game.GameId, opponentName, isWin: false, rating: points, gameType: game.GetType().Name);
            _gamesHistory.Add(record);
        }

        public string GetStats()
        {
            var report = new StringBuilder();
            report.AppendLine($"=====================================================================================");
            report.AppendLine($"Статистика гравця: {UserName} (Тип акаунту: {this.GetType().Name})");
            report.AppendLine($"Поточний рейтинг: {CurrentRating} | Зіграно партій: {GamesCount}");
            report.AppendLine($"-------------------------------------------------------------------------------------");
            report.AppendLine($"{"ID гри",-8} | {"Опонент",-14} | {"Тип гри",-16} | {"Результат",-10} | {"Зміна балів",-12}");
            report.AppendLine($"-------------------------------------------------------------------------------------");

            foreach (var game in _gamesHistory)
            {
                string resultText = game.IsWin ? "Перемога" : "Поразка";
                report.AppendLine($"{game.GameId,-8} | {game.OpponentName,-14} | {game.GameType,-16} | {resultText,-10} | {game.Rating,-12}");
            }

            report.AppendLine($"=====================================================================================");
            return report.ToString();
        }
    }

    public class StandardAccount : GameAccount
    {
        public StandardAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
        }
    }

    public class HalfLossAccount : GameAccount
    {
        public HalfLossAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
        }

        protected override int CalculateLosePoints(int rating)
        {
            return rating / 2;
        }
    }

    public class WinStreakAccount : GameAccount
    {
        private int _winStreak = 0;

        public WinStreakAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
        }

        protected override int CalculateWinPoints(int rating)
        {
            _winStreak++;
            if (_winStreak >= 2 && rating > 0)
            {
                return rating + 10;
            }
            return rating;
        }

        protected override int CalculateLosePoints(int rating)
        {
            _winStreak = 0;
            return rating;
        }
    }
}