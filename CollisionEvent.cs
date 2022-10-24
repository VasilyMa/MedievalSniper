using UnityEngine;

namespace Client
{
    struct CollisionEvent
    {
        public int TargetEntity;
        public Rigidbody TargetRigidbody;

        public GameObject TargetObject;

        /// <summary>
        /// If Destructible or Killable objects
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <param name="targetRigidbody"></param>
        /// <param name="targetObject"></param>
        public void Invoke(int targetEntity, Rigidbody targetRigidbody, GameObject targetObject)
        {
            TargetEntity = targetEntity;
            TargetRigidbody = targetRigidbody;
            TargetObject = targetObject;
        }

        /// <summary>
        /// If object is Undestructible
        /// </summary>
        /// <param name="targetObject"></param>
        public void Invoke(GameObject targetObject)
        {
            TargetObject = targetObject;
        }
    }
}