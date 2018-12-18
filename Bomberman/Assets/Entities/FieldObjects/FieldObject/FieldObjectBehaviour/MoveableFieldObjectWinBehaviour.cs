using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class MoveableFieldObjectWinBehaviour : BaseFieldObjectBehaviour
    {
        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            object[] objectsParameter = parameter as object[];

            if (objectsParameter != null)
            {
                FieldObjectType? fieldObjectType = objectsParameter[0] as FieldObjectType?;

                if (fieldObjectType != null)
                {
                    string stringParameter = objectsParameter[1] as string;

                    if (stringParameter != null)
                    {
                        if ((fieldObjectType == FieldObjectType.Player) && (stringParameter == "Player Win"))
                        {
                            GameObject playerGameObject = field.FieldObjectsComponentsGetter.GetPlayerGameObject();

                            playerGameObject.GetComponent<Animator>().Play("Win");
                            PlayFieldObjectSound(field, playerGameObject, "Player Win");
                        }
                        if ((fieldObjectType == FieldObjectType.Enemy) && (stringParameter == "Enemy Win"))
                        {
                            GameObject enemyGameObject = field.FieldObjects[(int)fieldIndexes.x][(int)fieldIndexes.y].GameObject;
                            Animator animator = enemyGameObject.GetComponent<Animator>();

                            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                                enemyGameObject.GetComponent<Animator>().Play("Win");
                            PlayFieldObjectSound(field, enemyGameObject, "Enemy Win");
                        }
                    }
                }
            }
        }
    }
}
