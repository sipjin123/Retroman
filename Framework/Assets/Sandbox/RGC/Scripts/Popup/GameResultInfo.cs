namespace Sandbox.RGC
{
    public class GameResultInfo
    {
        public string CurrencyId;

        public int GameScore;
    }

    public static class GameResultFactory
    {
        public static GameResultInfo Create(string currency, int score)
        {
            GameResultInfo data = new GameResultInfo();
            data.CurrencyId = currency;
            data.GameScore = score;

            return data;
        }

        public static GameResultInfo CreateDefault()
        {
            return Create(RGCConst.POINT_ID, 1000);
        }
    }
}