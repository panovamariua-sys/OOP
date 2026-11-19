using System;
using System.Collections.Generic;

namespace Lab3
{
    public interface IAccountService
    {
        void CreateAccount(GameAccount account);
        List<GameAccount> ReadAccounts();
        GameAccount ReadAccountById(int id);
        void RenameAccount(int id, string userName);
        void DeleteAccount(int id);
    }

    public interface IGameService
    {
        void CreateGame(int winnerId, int loserId, Game game);
        List<Game> ReadGames();
        List<Game> ReadPlayerGamesByPlayerId(int playerId);
    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accounts;
        private readonly IGameRepository _games;

        public AccountService(IAccountRepository accounts, IGameRepository games)
        {
            _accounts = accounts;
            _games = games;
        }

        public void CreateAccount(GameAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(account.UserName))
                throw new ArgumentException("Ім'я не може бути порожнім.");
            _accounts.Create(account);
        }

        public List<GameAccount> ReadAccounts()
        {
            return _accounts.ReadAll();
        }

        public GameAccount ReadAccountById(int id)
        {
            GameAccount account = _accounts.ReadById(id);
            if (account == null)
                throw new ArgumentException("Гравця не знайдено.");
            return account;
        }

        public void RenameAccount(int id, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Ім'я не може бути порожнім.");

            GameAccount account = ReadAccountById(id);
            account.UserName = userName;
            _accounts.Update(account);
        }

        public void DeleteAccount(int id)
        {
            ReadAccountById(id);
            foreach (Game game in _games.ReadAll())
                if (game.WinnerId == id || game.LoserId == id)
                    throw new InvalidOperationException("Не можна видалити учасника зіграних ігор.");
            _accounts.Delete(id);
        }
    }

    public class GameService : IGameService
    {
        private readonly IAccountRepository _accounts;
        private readonly IGameRepository _games;

        public GameService(IAccountRepository accounts, IGameRepository games)
        {
            _accounts = accounts;
            _games = games;
        }

        public void CreateGame(int winnerId, int loserId, Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            if (winnerId == loserId)
                throw new ArgumentException("Гравець не може грати сам із собою.");
            if (game.IsPlayed || _games.ReadById(game.GameId) != null)
                throw new InvalidOperationException("Ця гра вже була записана.");

            GameAccount winner = _accounts.ReadById(winnerId);
            GameAccount loser = _accounts.ReadById(loserId);
            if (winner == null || loser == null)
                throw new ArgumentException("Одного з гравців не знайдено.");
            if (string.IsNullOrWhiteSpace(winner.UserName) || string.IsNullOrWhiteSpace(loser.UserName))
                throw new ArgumentException("Імена гравців не можуть бути порожніми.");

            SoloRatingGame soloGame = game as SoloRatingGame;
            if (soloGame != null && soloGame.RatedPlayer != winner && soloGame.RatedPlayer != loser)
                throw new ArgumentException("Рейтинговий гравець повинен брати участь у грі.");

            winner.WinGame(loser.UserName, game);
            loser.LoseGame(winner.UserName, game);
            game.SetResult(winnerId, loserId);

            _accounts.Update(winner);
            _accounts.Update(loser);
            _games.Create(game);
        }

        public List<Game> ReadGames()
        {
            return _games.ReadAll();
        }

        public List<Game> ReadPlayerGamesByPlayerId(int playerId)
        {
            if (_accounts.ReadById(playerId) == null)
                throw new ArgumentException("Гравця не знайдено.");

            List<Game> result = new List<Game>();
            foreach (Game game in _games.ReadAll())
                if (game.WinnerId == playerId || game.LoserId == playerId)
                    result.Add(game);
            return result;
        }
    }
}
