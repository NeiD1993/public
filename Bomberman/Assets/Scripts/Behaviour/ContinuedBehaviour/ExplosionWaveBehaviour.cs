using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    class ExplosionWaveBehaviour : ContinuedBehaviour
    {
        [SerializeField][HideInInspector]
        public static readonly float maxExplosionWaveDelay = 0.8f;

        [SerializeField][HideInInspector]
        public static readonly float minExplosionWaveDelay = 0.01f;

        public override void SetDelay(object[] parameters)
        {
            if (parameters.Length == 1)
            {
                float? explosionWaveDelay = parameters[0] as float?;
                delay = ((explosionWaveDelay != null) && ((explosionWaveDelay >= minExplosionWaveDelay) && (explosionWaveDelay <= maxExplosionWaveDelay))) ?
                    (float)explosionWaveDelay : minExplosionWaveDelay;
            }
        }

        protected override IEnumerator ContinueExecute(float delay)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(delay);
                Execute(new object[] {delay, gameObject});
            }
        }
    }
}
