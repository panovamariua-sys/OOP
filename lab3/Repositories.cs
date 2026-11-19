using System;
using System.Collections.Generic;

namespace Lab3
{
    public interface IAccountRepository
    {
        void Create(GameAccount account);
        List<GameAccount> ReadAll();
        GameAccount ReadById(int id);
        void Update(GameAccount account);
        void Delete(int id);
    }

    public interface IGameRepository
    {
        void Create(Game game);
        List<Game> ReadAll();
        Game ReadById(int id);
        void Update(Game game);
        void Delete(int id);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly DbContext _context;

        public AccountRepository(DbContext context)
        {
            _context = context;
        }

        public void Create(GameAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (ReadById(account.Id) != null)
                throw new InvalidOperationException("Гравець уже існує.");
            _context.Accounts.Add(account);
        }

        public List<GameAccount> ReadAll()
        {
            return new List<GameAccount>(_context.Accounts);
        }

        public GameAccount ReadById(int id)
        {
            foreach (GameAccount account in _context.Accounts)
                if (account.Id == id)
                    return account;
            return null;
        }

        public void Update(GameAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            for (int i = 0; i < _context.Accounts.Count; i++)
            {
                if (_context.Accounts[i].Id == account.Id)
                {
                    _context.Accounts[i] = account;
                    return;
                }
            }
            throw new ArgumentException("Гравця не знайдено.");
        }

        public void Delete(int id)
        {
            GameAccount account = ReadById(id);
            if (account == null)
                throw new ArgumentException("Гравця не знайдено.");
            _context.Accounts.Remove(account);
        }
    }

    public class GameRepository : IGameRepository
    {
        private readonly DbContext _context;

        public GameRepository(DbContext context)
        {
            _context = context;
        }

        public void Create(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            if (ReadById(game.GameId) != null)
                throw new InvalidOperationException("Гра вже існує.");
            _context.Games.Add(game);
        }

        public List<Game> ReadAll()
        {
            return new List<Game>(_context.Games);
        }

        public Game ReadById(int id)
        {
            foreach (Game game in _context.Games)
                if (game.GameId == id)
                    return game;
            return null;
        }

        public void Update(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            for (int i = 0; i < _context.Games.Count; i++)
            {
                if (_context.Games[i].GameId == game.GameId)
                {
                    _context.Games[i] = game;
                    return;
                }
            }
            throw new ArgumentException("Гру не знайдено.");
        }

        public void Delete(int id)
        {
            Game game = ReadById(id);
            if (game == null)
                throw new ArgumentException("Гру не знайдено.");
            _context.Games.Remove(game);
        }
    }
}
