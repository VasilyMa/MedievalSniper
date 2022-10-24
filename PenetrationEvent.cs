using UnityEngine;

namespace Client
{
    struct PenetrationEvent
    {
        public GameObject PenetrationObject;

        public void Invoke(GameObject penetrationObject)
        {
            PenetrationObject = penetrationObject;
        }
    }
}