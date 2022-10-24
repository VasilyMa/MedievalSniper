using UnityEngine;

namespace Client
{
    struct KillTargetEvent
    {
        public int KilledEntity;
        public Rigidbody TochedRigidbody;
        public bool IsHeadShot;

        public void Invoke(int killedEntity, Rigidbody tochedRigidbody, bool isHeadShot = false)
        {
            KilledEntity = killedEntity;
            TochedRigidbody = tochedRigidbody;
            IsHeadShot = isHeadShot;
        }
    }
}