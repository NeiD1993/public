using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class MoveableFieldObjectDeathBehaviour : BaseFieldObjectBehaviour
    {
        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            object[] objectsParameter = parameter as object[];

            if (objectsParameter != null)
            {
                GameObject gameObject = objectsParameter[0] as GameObject;

                if (gameObject != null)
                {
                    BaseChangableFieldObjectsService.ResetVerticalGameObjectCoordinate(gameObject);
                    gameObject.GetComponent<Animator>().Play("Death");

                    if ((objectsParameter[1] as FieldObjectType?).Value == FieldObjectType.Enemy)
                        PlayFieldObjectSound(field, gameObject, "Enemy Death");
                    else
                        PlayFieldObjectSound(field, gameObject, "Player Death");
                }
            }
        }
    }
}
