using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour
{
    class PlayerFieldObjectMovingBehaviour : FieldObjectMovingBehaviour
    {
        public bool IsWallPassed { get; set; }

        protected override void TryMove(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            Nullable<MoveDirection> moveDirection = parameter as Nullable<MoveDirection>;
            Animator animator = field.FieldObjectsComponentsGetter.GetPlayerGameObject().GetComponent<Animator>();

            if ((moveDirection != null) && (animator != null))
            {
                Vector2 playerPosition = field.PlayerPosition.Value;

                TryMoveOnPosition(field, ref fieldIndexes, moveDirection.Value, field.FieldObjects[(int)playerPosition.x][(int)playerPosition.y].ObjectType, animator);
            }
        }
    }
}
