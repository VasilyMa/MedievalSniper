namespace Client 
{
    struct ShowScoresEvent 
    {
        public int ScoreValue;
        public bool IsHeadShot;

        public void Invoke(int scoreValue, bool isHeadShot)
        {
            ScoreValue = scoreValue;
            IsHeadShot = isHeadShot;
        }
    }
}