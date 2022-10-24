using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    struct View
    {
        public int EntityNumber;
        public GameObject GameObject;
        public GameObject Model;
        public Transform Transform;

        public EcsInfoMB EcsInfoMB;

        public Rigidbody Rigidbody;

        public Animator Animator;
        public NavMeshAgent NavMeshAgent;

        public int LayerBeforeDeath;
    }
}