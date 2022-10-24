using UnityEngine;

namespace Client
{
    struct Arrow
    {
        public GameObject Tip;
        public float StrengthLaunching;
        public float BulletTimeChargeCurrent;
        public float BulletTimeChargeMax;

        public CollisionDetectorMB CollisionDetectorMB;

        public ParticleSystem UsualTrail;
        public ParticleSystem EpicTrail;
    }
}