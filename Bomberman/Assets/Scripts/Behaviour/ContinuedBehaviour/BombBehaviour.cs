using UnityEngine;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    class BombBehaviour : ContinuedBehaviour
    {
        [SerializeField][HideInInspector]
        private static readonly byte maxBombTimer = 10;

        [SerializeField][HideInInspector]
        private static readonly byte minBombTimer = 1;

        public override void SetDelay(object[] parameters)
        {
            if (parameters.Length == 1)
            {
                byte? bombTimer = parameters[0] as byte?;
                delay = ((bombTimer != null) && ((bombTimer >= minBombTimer) && (bombTimer <= maxBombTimer))) ? (float)bombTimer : minBombTimer;
            }
        }
    }
}
