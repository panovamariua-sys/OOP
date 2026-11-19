namespace Lab1
{
    public class GameFactory
    {
        public Game CreateStandardGame(int rating)
        {
            return new StandardGame(rating);
        }

        public Game CreateTrainingGame()
        {
            return new TrainingGame();
        }

        public Game CreateSoloRatingGame(int rating, GameAccount ratedPlayer)
        {
            return new SoloRatingGame(rating, ratedPlayer);
        }
    }
}