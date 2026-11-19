using System;
namespace Lab1
{
    public class GameHistoryRecord
    {
        public int GameId { get; }
        public string OpponentName { get; }
        public bool IsWin { get; }
        public int Rating { get; }
        public string GameType { get; }

        public GameHistoryRecord(int gameId, string opponentName, bool isWin, int rating, string gameType)
        {
            GameId = gameId;
            OpponentName = opponentName;
            IsWin = isWin;
            Rating = rating;
            GameType = gameType;
        }
    }

    public abstract class Game
    {
        private static int _gameIdSeed = 1001;

        public int GameId { get; }
        protected int BaseRating { get; }

        protected Game(int rating)
        {
            if (rating < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Рейтинг, на який грають, не може бути від'ємним!");
            }

            GameId = _gameIdSeed++;
            BaseRating = rating;
        }

        public abstract int GetPlayRating(GameAccount player);
    }

    public class StandardGame : Game
    {
        public StandardGame(int rating) : base(rating) { }

        public override int GetPlayRating(GameAccount player)
        {
            return BaseRating;
        }
    }

    public class TrainingGame : Game
    {
        public TrainingGame() : base(0) { }

        public override int GetPlayRating(GameAccount player)
        {
            return 0;
        }
    }

    public class SoloRatingGame : Game
    {
        private readonly GameAccount _ratedPlayer;

        public SoloRatingGame(int rating, GameAccount ratedPlayer) : base(rating)
        {
            _ratedPlayer = ratedPlayer ?? throw new ArgumentNullException(nameof(ratedPlayer));
        }

        public override int GetPlayRating(GameAccount player)
        {
            return player == _ratedPlayer ? BaseRating : 0;
        }
    }
}