using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    class EnemyBehaviour : ContinuedBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private static readonly byte intellectLevelDivisor = 2;

        [SerializeField]
        [HideInInspector]
        private static readonly byte maxIntellectLevel = 3;

        [SerializeField]
        [HideInInspector]
        private static readonly byte minIntellectLevel = 1;

        [SerializeField]
        [HideInInspector]
        public bool IsDead { get; set; }

        [SerializeField]
        [HideInInspector]
        public bool IsHit { get; set; }

        public override void SetDelay(object[] parameters)
        {
            if (parameters.Length == 1)
            {
                byte? intellectLevel = parameters[0] as byte?;
                delay = ((intellectLevel != null) && ((intellectLevel >= minIntellectLevel) && (intellectLevel <= maxIntellectLevel))) ?
                    ((float)intellectLevel) / intellectLevelDivisor : minIntellectLevel;
            }
        }

        protected override IEnumerator ContinueExecute(float delay)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(delay);

                if (!IsDead)
                {
                    if (Field.PlayerPosition != null)
                        Execute(delay);
                    else
                    {
                        if (!IsHit)
                            Execute(new object[] { FieldObjectType.Enemy, "Enemy Win" });
                        else
                            Execute(null);
                    }
                }
                else
                    ExecuteOnTimer(FieldObjectType.Enemy);
            }
        }
    }
}
