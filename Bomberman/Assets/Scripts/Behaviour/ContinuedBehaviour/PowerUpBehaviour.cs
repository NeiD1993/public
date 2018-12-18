using UnityEngine;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    class PowerUpBehaviour : ContinuedBehaviour
    {
        [SerializeField][HideInInspector]
        private static readonly float maxPowerUpDelay = 1;

        [SerializeField][HideInInspector]
        private static readonly float minPowerUpDelay = 0.02f;

        public override void SetDelay(object[] parameters)
        {
            if (parameters.Length == 1)
            {
                float? powerUpDelay = parameters[0] as float?;
                delay = ((powerUpDelay != null) && ((powerUpDelay >= minPowerUpDelay) && (powerUpDelay <= maxPowerUpDelay))) ?
                    (float)powerUpDelay : minPowerUpDelay;
            }
        }
    }
}
