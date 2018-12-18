using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.FieldObjectsMover;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjectsService.FieldObjectsDestroyer
{
    class FieldObjectsDestroyer : BaseFieldObjectsDestroyer
    {
        public FieldObjectsDestroyer(Field field, int objectSecondsLifetime) : base(field, objectSecondsLifetime) { }
        
        public override void DestroyBomb(Vector2 bombIndexes)
        {
            GameObject bombGameObject = Field.RemoveBombGameObject((int)bombIndexes.x, (int)bombIndexes.y);

            if (bombGameObject != null)
                GameObject.Destroy(bombGameObject);
        }

        public override void DestroyGameObject(int horizontalIndex, int verticalIndex, FieldObjectType fieldObjectType, GameObject destroyerGameObject = null)
        {
            if ((fieldObjectType != FieldObjectType.Empty) && (fieldObjectType != FieldObjectType.UnbreakableWall))
            {
                FieldObject fieldObject = Field.FieldObjects[horizontalIndex][verticalIndex];
                GameObject fieldGameObject;

                if ((fieldObjectType == FieldObjectType.Enemy) || (fieldObjectType == FieldObjectType.BreakableWall))
                {
                    fieldGameObject = fieldObject.GameObject;

                    if (fieldObjectType == FieldObjectType.Enemy)
                    {
                        string killedEnemyTag = fieldGameObject.tag;
                        int killedEnemyPoints = Field.FieldObjectsStatistics.EnemiesKillPoints[killedEnemyTag];

                        Field.EnemiesCount--;

                        if (Field.PlayerPosition != null)
                        {
                            Field.FieldObjectsStatistics.CurrentPlayerKillPoints += killedEnemyPoints;
                            Field.FieldDynamicObjectsGenerator.CreateLight("Kill Enemy", killedEnemyTag, killedEnemyPoints);
                        }

                        fieldGameObject.GetComponent<EnemyBehaviour>().IsDead = true;
                    }
                }
                else
                {
                    DestroyLight();

                    if (destroyerGameObject != null)
                        destroyerGameObject.GetComponent<EnemyBehaviour>().IsHit = true;

                    fieldGameObject = Field.FieldObjectsComponentsGetter.GetPlayerGameObject();
                    fieldGameObject.GetComponent<PlayerBehaviour>().IsActive = false;
                    Field.FieldDynamicObjectsGenerator.CreateResultsText("You Lose!");
                }

                if (((fieldObjectType != FieldObjectType.Player) && (fieldObjectType != FieldObjectType.PlayerAndBreakableWall)) && (fieldObjectType != FieldObjectType.Enemy))
                    GameObject.Destroy(fieldGameObject);
                else
                {
                    if (fieldObjectType == FieldObjectType.PlayerAndBreakableWall)
                    {
                        GameObject parentGameObject = fieldGameObject.transform.parent.gameObject;
                        fieldGameObject.transform.SetParent(Field.FieldGameObject.transform);
                        GameObject.Destroy(parentGameObject);
                    }

                    fieldGameObject = null;
                }

                if (fieldObjectType == FieldObjectType.PlayerAndBreakableWall)
                    fieldObject.ObjectType = FieldObjectType.BreakableWall;
                else
                    fieldObject.ObjectType = FieldObjectType.Empty;

                if ((fieldObjectType == FieldObjectType.Player) || (fieldObjectType == FieldObjectType.PlayerAndBreakableWall))
                    Field.ResetPlayerPosition();

                if (Field.EnemiesCount == 0)
                {
                    Field.FieldObjectsComponentsGetter.GetPlayerGameObject().GetComponent<PlayerBehaviour>().IsActive = false;
                    Field.FieldDynamicObjectsGenerator.CreateResultsText("You Win!");
                }
            }
        }

        public override void DestroyPowerUp(Vector2 powerUpIndexes)
        {
            if (Field.PowerUps.ContainsKey(powerUpIndexes))
            {
                GameObject powerUpGameObject = Field.PowerUps[powerUpIndexes].GameObject;

                powerUpGameObject.transform.parent = null;
                GameObject.Destroy(powerUpGameObject);
                Field.PowerUps.Remove(powerUpIndexes);

                if (Field.PowerUpsGameObject.transform.childCount == 0)
                    GameObject.Destroy(Field.PowerUpsGameObject);
            }
        }

        public override void DestroyLight()
        {
            if (Field.PlayerPosition != null)
            {
                GameObject playerGameObject = Field.FieldObjectsComponentsGetter.GetPlayerGameObject();
                
                if ((playerGameObject != null) && (playerGameObject.transform.childCount == 3))
                {
                    GameObject light = playerGameObject.transform.GetChild(2).gameObject;
                    GameObject.Destroy(light);
                    light.transform.SetParent(null);
                }
            }
        }

        public override void DestroyExplosionWaveGameObject(GameObject explosionWaveGameObject)
        {
            GameObject.Destroy(explosionWaveGameObject);
            explosionWaveGameObject.transform.parent = null;

            if (Field.ExplosionsGameObject.transform.childCount == 0)
                GameObject.Destroy(Field.ExplosionsGameObject);
        }

        public override void DestroyGameObjectBetweenFieldIndexes(Vector2 startFieldIndexes, Vector2 endFieldIndexes, MoveDirection moveDirection)
        {
            int initialStepsCount = 0;
            Vector2 moveDirectionIncrement;
            BaseFieldDynamicObjectsMover.GetIntegerFieldIndexesByMoveDirection(endFieldIndexes.x, endFieldIndexes.y, 1, moveDirection, out moveDirectionIncrement);
            int stepsCount = (int)(endFieldIndexes - startFieldIndexes).magnitude + 1;

            while (initialStepsCount < stepsCount)
            {
                DestroyGameObject((int)startFieldIndexes.x, (int)startFieldIndexes.y, field.FieldObjects[(int)startFieldIndexes.x][(int)startFieldIndexes.y].ObjectType);
                startFieldIndexes += moveDirectionIncrement;
                initialStepsCount++;
            }
        }

        public override IEnumerator DestroyGameObjectInTimer(GameObject gameObject)
        {
            yield return new WaitForSecondsRealtime(ObjectSecondsLifetime);
            GameObject.Destroy(gameObject);
        }
    }
}
