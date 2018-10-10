namespace Sandbox.RGC
{
    public class GameResultInfo
    {
         public string CurrencyId;
         
         public int GameScore;
     
        public float Rate;
    }
     
    public static class GameResultFactory
    {
        public static GameResultInfo Create(string currency, int score, float rate)
        {
            GameResultInfo data = new GameResultInfo();
            data.CurrencyId = currency;
            data.GameScore = score;
            data.Rate = rate;

            return data;
        }

        public static GameResultInfo CreateDefault()
        {
            return Create(RGCConst.SCORE_SLUG, 1000, 1f);
        }
    }
}