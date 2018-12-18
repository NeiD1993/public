using Assets.Entities.FieldObjects;
using UnityEngine;

namespace Assets.Entities.FieldObjectsService.BaseFieldObjectService
{
    abstract class BaseChangableFieldObjectsService : BaseFieldObjectsService
    {
        protected System.Random randomGenerator = new System.Random();

        public BaseChangableFieldObjectsService(Field field) : base(field) 
        {
            Field = field;
        }

        private static float GetWallPositionDisplacement(int wallIndex, float wallScale)
        {
            if (wallIndex % 2 == 0)
            {
                float wallPositionDisplacement = (1 / 2) * wallScale;
                if (wallIndex > 0)
                    wallPositionDisplacement = -wallPositionDisplacement;

                return wallPositionDisplacement;
            }
            else
                return 0;
        }

        public static void ResetVerticalGameObjectCoordinate(GameObject gameObject)
        {
            Vector3 gameObjectPosition = gameObject.transform.position;

            gameObject.transform.position = new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z);
        }

        protected void ChangeGameObjectPosition(float horizontalIndex, float verticalIndex, GameObject gameObject)
        {
            Vector3 localScale = Field.WallPrefab.transform.localScale;
            float localScaleX = localScale.x;
            float localScaleZ = localScale.z;

            gameObject.transform.position = new Vector3((horizontalIndex - Field.HorizontalSize / 2) * localScaleX + GetWallPositionDisplacement((int)horizontalIndex, localScaleX),
                                                        (gameObject.transform.position.y == 0) ? gameObject.transform.localScale.y / 2 : gameObject.transform.position.y,
                                                        (verticalIndex - Field.VerticalSize / 2) * localScaleZ + GetWallPositionDisplacement((int)horizontalIndex, localScaleZ));
        }

        protected void AddFreePosition(Vector2 freePosition)
        {
            field.FreePositions.Add(freePosition);
        }

        protected void RemoveFreePosition(Vector2 freePosition)
        {
            Field.FreePositions.Remove(freePosition);
        }
    }
}
