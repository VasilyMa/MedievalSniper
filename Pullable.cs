using UnityEngine;

namespace Client
{
    struct Pullable
    {
        public float CurrentStrength;
        public AnimationCurve PullongUpCurve;
        public AnimationCurve PullongDownCurve;
        public bool isPullingUp;

        public SniperChangeIKStates SniperChangeIKStates;
    }
}