using Assets.Scripts.Behaviour.BaseBehaviour;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    abstract class ContinuedBehaviour : BaseFieldBehaviour
    {
        [SerializeField][HideInInspector]
        protected float delay;

        protected override void Start() 
        {
            StartCoroutine(ContinueExecute(delay));
        }

        protected override void Update() { }

        protected virtual IEnumerator ContinueExecute(float delay)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(delay);
                Execute(delay);
            }
        }

        public abstract void SetDelay(object[] parameters);
    }
}
