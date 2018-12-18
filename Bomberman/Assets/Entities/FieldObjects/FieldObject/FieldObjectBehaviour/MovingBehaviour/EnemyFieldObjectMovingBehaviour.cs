using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour
{
    class EnemyFieldObjectMovingBehaviour : FieldObjectMovingBehaviour
    {
        public EnemiesIntellectType EnemiesIntellectTypes { get; set; }

        protected override void TryMove(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            Animator animator = field.FieldObjects[(int)fieldIndexes.x][(int)fieldIndexes.y].GameObject.GetComponent<Animator>();

            if ((parameter != null) && (animator != null))
            {
                switch (EnemiesIntellectTypes)
                {
                    case EnemiesIntellectType.Semismart:
                        TryMoveOnPosition(field, ref fieldIndexes, field.FieldDynamicObjectsMover.GetPriorityMoveDirection(fieldIndexes, MovingSpeed, FieldObjectType.Enemy), FieldObjectType.Enemy, animator);
                        break;
                    case EnemiesIntellectType.Smart:
                        TryMoveOnPosition(field, ref fieldIndexes, field.FieldDynamicObjectsMover.GetComplexPriorityMoveDirection(fieldIndexes, MovingSpeed, FieldObjectType.Enemy), FieldObjectType.Enemy, animator);
                        break;
                    case EnemiesIntellectType.Stupid:
                        TryMoveOnPosition(field, ref fieldIndexes, field.FieldDynamicObjectsMover.GetSimplePriorityMoveDirection(fieldIndexes, MovingSpeed, FieldObjectType.Enemy), FieldObjectType.Enemy, animator);
                        break;
                }
            }
        }
    }
}
