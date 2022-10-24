namespace Client
{
    struct GiveScoreEvent
    {
        public int Value;
        public bool IsHeadShot;

        public void Invoke(int value, bool isHeadShot = false)
        {
            Value = value;
            IsHeadShot = isHeadShot;
        }
    }
}