using Assets.Scripts.Behaviour.ContinuedBehaviour;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class EnemyFieldObjectHitBehaviour : BaseFieldObjectBehaviour
    {
        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            if (parameter == null)
            {
                GameObject fieldGameObject = field.FieldObjects[(int)fieldIndexes.x][(int)fieldIndexes.y].GameObject;

                fieldGameObject.GetComponent<Animator>().Play("Hit");
                fieldGameObject.GetComponent<EnemyBehaviour>().IsHit = false;
                PlayFieldObjectSound(field, fieldGameObject, "Enemy Hit");
            }
        }
    }
}
