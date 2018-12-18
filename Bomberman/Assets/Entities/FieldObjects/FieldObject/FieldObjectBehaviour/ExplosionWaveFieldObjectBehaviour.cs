using Assets.Entities.FieldObjectsService.FieldGenerator;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class ExplosionWaveFieldObjectBehaviour : BaseFieldObjectBehaviour
    {
        public static readonly byte maxExplosionWaveDistance = 8;

        public static readonly byte minExplosionWaveDistance = 1;

        public static readonly float minExplosionWaveStep = 0.05f;

        private int explosionWaveDistance;

        private bool isSoundPlayed;

        private float explosionWaveStep;

        private GameObject explosionWavePartPrefab;

        private GameObject parentExplosionWavePart;

        private MoveDirection moveDirection;

        Vector2 currentFieldIndexes;

        protected bool isExplosionWaveMoveable;

        protected float currentStepsCount;

        protected float stepsCount;

        public void Init(int explosionWaveDistance, float explosionWaveStep, GameObject explosionWavePartPrefab, GameObject parentExplosionWavePart)
        {
            ExplosionWaveDistance = explosionWaveDistance;
            ExplosionWaveStep = explosionWaveStep;
            ExplosionWavePartPrefab = explosionWavePartPrefab;
            ParentExplosionWavePart = parentExplosionWavePart;
            isExplosionWaveMoveable = true;
            currentStepsCount = 0;
            stepsCount = ExplosionWaveDistance / explosionWaveStep;
        }

        public int ExplosionWaveDistance
        {
            get
            {
                return explosionWaveDistance;
            }

            set
            {
                explosionWaveDistance = ((value >= minExplosionWaveDistance) && (value <= maxExplosionWaveDistance)) ? value : minExplosionWaveDistance;
            }
        }

        public float ExplosionWaveStep
        {
            get
            {
                return explosionWaveStep;
            }

            set
            {
                explosionWaveStep = ((value > minExplosionWaveStep) && (value <= ExplosionWaveDistance)) ? value : minExplosionWaveStep;
            }
        }

        public GameObject ExplosionWavePartPrefab
        {
            get
            {
                return explosionWavePartPrefab;
            }
            set
            {
                if ((value != null) && (explosionWavePartPrefab != value))
                    explosionWavePartPrefab = value;
            }
        }

        public GameObject ParentExplosionWavePart
        {
            get
            {
                return parentExplosionWavePart;
            }
            set
            {
                if ((value != null) && (parentExplosionWavePart != value))
                    parentExplosionWavePart = value;
            }
        }

        public MoveDirection MoveDirection
        {
            get
            {
                return moveDirection;
            }

            set
            {
                if ((value != MoveDirection.None) && (moveDirection != value))
                    moveDirection = value;
            }
        }

        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            object[] objectsParameter = parameter as object[];

            if (objectsParameter != null)
            {
                float? fieldIndexesStep = objectsParameter[0] as float?;

                if (fieldIndexesStep != null)
                {
                    if (currentStepsCount <= stepsCount)
                    {
                        if (!isSoundPlayed)
                        {
                            PlayFieldObjectSound(field, objectsParameter[1] as GameObject, "Bomb Explosion");
                            isSoundPlayed = true;
                        }

                        if (currentStepsCount == 0)
                            currentFieldIndexes = fieldIndexes;

                        if (isExplosionWaveMoveable)
                            isExplosionWaveMoveable = field.FieldDynamicObjectsMover.TryMoveNonEmptyFieldObjectOnFloatIndexes(ref currentFieldIndexes, ExplosionWaveStep, MoveDirection,
                                                                                                                              ExplosionWavePartPrefab =
                                                                                                                              BaseFieldObjectsGenerator.CreateGameObject(ExplosionWavePartPrefab,
                                                                                                                                                                         ParentExplosionWavePart));

                        field.FieldObjectsDestroyer.DestroyGameObjectBetweenFieldIndexes(fieldIndexes, currentFieldIndexes, MoveDirection);
                        currentStepsCount++;
                    }
                    else
                        field.FieldObjectsDestroyer.DestroyExplosionWaveGameObject(parentExplosionWavePart);
                }
            }
        }
    }
}