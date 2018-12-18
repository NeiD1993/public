using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Scripts.Behaviour;
using Assets.Scripts.Behaviour.BaseBehaviour;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator
{
    class FieldDynamicObjectsGenerator : BaseFieldDynamicObjectsGenerator
    {
        public FieldDynamicObjectsGenerator(Field field) : base(field) { }

        public Vector2 PlayerPosition { get; protected set; }

        protected void CreateMoveableDynamicObject(FieldObjectType moveableDynamicObjectType, GameObject moveableDynamicObjectPrefab, object[] parameters)
        {
            Nullable<Vector2> freePosition = null;

            if ((field != null) && (moveableDynamicObjectPrefab != null))
            {
                freePosition = FindFreePositionForDynamicObject();

                if (freePosition != null)
                {
                    BaseFieldBehaviour behaviour = null;
                    Vector2 freePositionValue = freePosition.Value;

                    if (moveableDynamicObjectType == FieldObjectType.Player)
                    {
                        behaviour = AddBehaviourToFieldObject<PlayerBehaviour>(moveableDynamicObjectPrefab, freePositionValue);
                        behaviour.AddFieldObjectBehaviour(CreateMovingBehaviourForMoveableDynamicObject<PlayerFieldObjectMovingBehaviour>(MoveDirection.Up, parameters[0] as int?));
                        PlayerFieldObjectBombBehaviour playerFieldObjectBombBehaviour = CreateFieldObjectBehaviour<PlayerFieldObjectBombBehaviour>();
                        playerFieldObjectBombBehaviour.Init(parameters[1] as GameObject, parameters[2] as GameObject, (parameters[3] as byte?).Value, (parameters[4] as int?).Value,
                                                           (parameters[5] as float?).Value, (parameters[6] as float?).Value, (parameters[7] as int?).Value);
                        behaviour.AddFieldObjectBehaviour(playerFieldObjectBombBehaviour);
                    }
                    else if (moveableDynamicObjectType == FieldObjectType.Enemy)
                    {
                        EnemyFieldObjectMovingBehaviour enemyFieldObjectMovingBehaviour =
                            CreateMovingBehaviourForMoveableDynamicObject<EnemyFieldObjectMovingBehaviour>(MoveDirection.Up, parameters[0] as int?);
                        enemyFieldObjectMovingBehaviour.EnemiesIntellectTypes = (parameters[1] as Nullable<EnemiesIntellectType>).Value;
                        behaviour = AddContinuedFieldObjectBehaviour<EnemyBehaviour>(moveableDynamicObjectPrefab, enemyFieldObjectMovingBehaviour, freePositionValue, (parameters[2] as byte?).Value);
                        behaviour.AddFieldObjectBehaviour<EnemyFieldObjectHitBehaviour>(CreateFieldObjectBehaviour<EnemyFieldObjectHitBehaviour>());
                    }

                    behaviour.AddFieldObjectBehaviour<MoveableFieldObjectDeathBehaviour>(CreateFieldObjectBehaviour<MoveableFieldObjectDeathBehaviour>());
                    behaviour.AddFieldObjectBehaviour<MoveableFieldObjectWinBehaviour>(CreateFieldObjectBehaviour<MoveableFieldObjectWinBehaviour>());

                    ChangeGameObjectOnField((int)freePositionValue.x, (int)freePositionValue.y, moveableDynamicObjectType, moveableDynamicObjectPrefab);
                    RemoveFreePosition(freePositionValue);

                    Field.PlayerPosition = freePositionValue;
                }
            }
        }

        protected Nullable<Vector2> FindFreePositionForDynamicObject()
        {
            Nullable<Vector2> freePosition = null;
            IList<Vector2> freePositions = field.FreePositions;
            int freePositionsCount = freePositions.Count;

            if (freePositionsCount > 0)
                freePosition = freePositions[randomGenerator.Next(0, freePositionsCount - 1)];

            return freePosition;
        }

        public override GameObject CreateBomb(GameObject bombPrefab, GameObject explosionWavePartPrefab, Vector2 freePosition, byte bombDelay, int explosionWaveDistance, float explosionWaveDelay,
                                              float explosionWaveStep)
        {
            GameObject bombClone = null;

            if ((bombPrefab != null) && (explosionWavePartPrefab != null))
            {
                Nullable<Vector2> playerPosition = Field.PlayerPosition;

                if ((playerPosition != null) && (playerPosition == freePosition))
                {
                    BombFieldObjectBehaviour bombFieldObjectBehaviour = CreateFieldObjectBehaviour<BombFieldObjectBehaviour>();
                    bombFieldObjectBehaviour.Init(explosionWavePartPrefab, explosionWaveDistance, explosionWaveStep, explosionWaveDelay);
                    AddContinuedFieldObjectBehaviour<BombBehaviour>(bombPrefab, bombFieldObjectBehaviour, freePosition, bombDelay);
                    bombClone = CreateGameObject(bombPrefab, Field.FieldGameObject);
                    ChangeGameObjectPosition(freePosition.x, freePosition.y, bombClone);
                }
            }

            return bombClone;
        }

        public override void CreateEnemy(int enemiesCount, GameObject[] enemyPrefabs, int movingSpeed, byte delay)
        {
            Array enemiesIntellectTypes = Enum.GetValues(typeof(EnemiesIntellectType));

            Field.EnemiesCount = enemiesCount;

            if ((enemyPrefabs != null) && (enemyPrefabs.Length == enemiesIntellectTypes.Length) && (enemiesCount > 0) && (enemiesCount < field.FreePositions.Count))
            {
                int enemiesIntellectTypeIndex;
                for (int i = 0; i < enemiesCount; i++)
                {
                    enemiesIntellectTypeIndex = randomGenerator.Next(0, enemiesIntellectTypes.Length);
                    CreateMoveableDynamicObject(FieldObjectType.Enemy, enemyPrefabs[enemiesIntellectTypeIndex], new object[] { movingSpeed, (EnemiesIntellectType)enemiesIntellectTypeIndex, delay });
                }
            }
        }

        public override void CreateExplosionWave(GameObject explosionWavePartPrefab, Vector2 bombPosition, int explosionWaveDistance, float explosionWaveDelay, float explosionWaveStep)
        {
            if (explosionWavePartPrefab != null)
            {
                GameObject explosionWaveGameObject;
                ExplosionWaveFieldObjectBehaviour explosionWaveFieldObjectBehaviour;
                ChangeGameObjectPosition((int)bombPosition.x, (int)bombPosition.y, explosionWavePartPrefab);

                foreach (MoveDirection moveDirection in Enum.GetValues(typeof(MoveDirection)))
                {
                    if (moveDirection != MoveDirection.None)
                    {
                        explosionWaveGameObject = new GameObject(Field.explosionWaveGameObjectName);
                        explosionWaveGameObject.transform.parent = Field.ExplosionsGameObject.transform;
                        explosionWaveGameObject.AddComponent<AudioSource>();
                        explosionWaveFieldObjectBehaviour = CreateFieldObjectBehaviour<ExplosionWaveFieldObjectBehaviour>();
                        explosionWaveFieldObjectBehaviour.MoveDirection = moveDirection;
                        AddContinuedFieldObjectBehaviour<ExplosionWaveBehaviour>(explosionWaveGameObject, explosionWaveFieldObjectBehaviour, bombPosition, explosionWaveDelay);
                        explosionWaveFieldObjectBehaviour.Init(explosionWaveDistance, explosionWaveStep, explosionWavePartPrefab, explosionWaveGameObject);
                    }
                }
            }
        }

        public override void CreatePlayer(GameObject playerPrefab, GameObject bombPrefab, GameObject explosionWavePartPrefab, int movingSpeed, byte bombDelay, int explosionWaveDistance,
                                          float explosionWaveDelay, float explosionWaveStep, int maxPlacedBombsCount)
        {
            if ((playerPrefab != null) && (bombPrefab != null) && (explosionWavePartPrefab != null))
            {
                CreateMoveableDynamicObject(FieldObjectType.Player, playerPrefab, new object[] { movingSpeed, bombPrefab, explosionWavePartPrefab, bombDelay, explosionWaveDistance, explosionWaveDelay,
                                                                                                 explosionWaveStep, maxPlacedBombsCount });
            }
        }

        public override GameObject CreateLight(string materialTag, string killedEnemyTag = null, int? killedEnemyPoints = null)
        {
            if ((Field.PlayerPosition != null) && (Field.LightMaterials.ContainsKey(materialTag)))
            {
                Vector2 playerPosition = Field.PlayerPosition.Value;
                int playerPositionX = (int)playerPosition.x;
                int playerPositionY = (int)playerPosition.y;
                Text lightText = Field.FieldObjectsComponentsGetter.GetLightGameObject();
                string text = null;

                if ((lightText != null) && (lightText.tag == materialTag))
                {
                    if (materialTag != "Kill Enemy")
                    {
                        string powerUpLightTextDigits = new Regex("\\d+", RegexOptions.RightToLeft).Match(lightText.text).Value;

                        text = lightText.text.Remove(lightText.text.Length - powerUpLightTextDigits.Length, powerUpLightTextDigits.Length) +
                               (Int32.Parse(powerUpLightTextDigits) + 1).ToString();
                    }
                    else
                    {
                        string killedEnemyTagText = new Regex(" x \\w+").Match(lightText.text).Value;

                        killedEnemyTagText = killedEnemyTagText.Substring(3, killedEnemyTagText.Length - 3);

                        if (killedEnemyTagText == killedEnemyTag)
                        {
                            int increasedKilledEnemiesCount = Int32.Parse(new Regex("\\d+").Match(lightText.text).Value) + 1;
                            text = increasedKilledEnemiesCount.ToString() + " x " + killedEnemyTagText + " + " + (increasedKilledEnemiesCount * killedEnemyPoints).ToString();
                        }
                    }
                }

                Field.FieldObjectsDestroyer.DestroyLight();

                GameObject lightPrefab = Field.LightPrefab;

                AddBehaviourToFieldObject<LightBehaviour>(lightPrefab);

                ChangeGameObjectPosition(playerPositionX, playerPositionY, lightPrefab);

                GameObject light = CreateGameObject(lightPrefab, Field.FieldObjectsComponentsGetter.GetPlayerGameObject(), Field.LightMaterials[materialTag]);
                GameObject textPrefab = Field.TextPrefab;

                textPrefab.GetComponentInChildren<Text>().enabled = false;

                GameObject textGameObject = CreateGameObject(textPrefab, light);

                lightText = textGameObject.GetComponentInChildren<Text>();

                if (lightText != null)
                {
                    lightText.tag = materialTag;
                    lightText.color = Field.LightMaterials[materialTag].color;

                    if (materialTag == "Wall Pass")
                        lightText.text = materialTag + " is True";
                    else
                    {
                        if (text != null)
                            lightText.text = text;
                        else
                        {
                            if (materialTag != "Kill Enemy")
                                lightText.text = materialTag + " + 1";
                            else
                                lightText.text = "1 x " + killedEnemyTag + " + " + killedEnemyPoints;
                        }
                    }
                }

                return light;
            }
            else
                return null;
        }

        public override void CreateResultsText(string resultsText)
        {
            IDictionary<string, Material> resultsTextsMaterials = Field.ResultsTextsMaterials;

            if (resultsTextsMaterials.ContainsKey(resultsText))
            {
                GameObject resultsTextGameObject = CreateGameObject(Field.ResultsTextPrefab, Field.FieldGameObject);
                Text text = resultsTextGameObject.GetComponentInChildren<Text>();

                text.color = resultsTextsMaterials[resultsText].color;
                text.text = resultsText;
            }
        }
    }
}
