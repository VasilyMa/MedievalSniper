namespace Client
{
    struct DestroyEvent
    {
        public int DestroyedEntity;

        public void Invoke(int destroydEntity)
        {
            DestroyedEntity = destroydEntity;
        }
    }
}