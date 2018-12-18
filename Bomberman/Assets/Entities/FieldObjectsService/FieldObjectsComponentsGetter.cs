using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Entities.FieldObjectsService
{
    class FieldObjectsComponentsGetter : BaseFieldObjectsService
    {
        public FieldObjectsComponentsGetter(Field field) : base(field) { }

        public GameObject GetPlayerGameObject()
        {
            if (Field.PlayerPosition != null)
            {
                Vector2 playerPosition = Field.PlayerPosition.Value;
                FieldObject playerObject = field.FieldObjects[(int)playerPosition.x][(int)playerPosition.y];
                GameObject playerGameObject = playerObject.GameObject;

                if (playerObject.ObjectType == FieldObjectType.PlayerAndBreakableWall)
                    playerGameObject = playerGameObject.transform.GetChild(0).gameObject;

                return playerGameObject;
            }
            else
                return null;
        }

        public Text GetLightGameObject()
        {
            GameObject playerGameObject = GetPlayerGameObject();

            return ((playerGameObject != null) && (playerGameObject.transform.childCount == 3)) ? playerGameObject.transform.GetChild(2).GetComponentInChildren<Text>() : null;
        }

        public PlayerBehaviourType GetPlayerBehaviour<PlayerBehaviourType>() where PlayerBehaviourType : PlayerBehaviour
        {
            GameObject playerGameObject = GetPlayerGameObject();

            return (playerGameObject != null) ? playerGameObject.GetComponent<PlayerBehaviourType>() : null;
        }
    }
}
