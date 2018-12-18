using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class PlayerFieldObjectBombBehaviour : BaseFieldObjectBehaviour
    {
        private GameObject bombPrefab;

        private GameObject explosionWavePartPrefab;

        private byte bombDelay;

        private int explosionWaveDistance;

        private int maxPlacedBombsCount;

        private float explosionWaveDelay;

        private float explosionWaveStep;

        public void Init(GameObject bombPrefab, GameObject explosionWavePartPrefab, byte bombDelay, int explosionWaveDistance, float explosionWaveDelay, float explosionWaveStep, int maxPlacedBombsCount)
        {
            BombPrefab = bombPrefab;
            ExplosionWavePartPrefab = explosionWavePartPrefab;
            BombDelay = bombDelay;
            ExplosionWaveDistance = explosionWaveDistance;
            ExplosionWaveDelay = explosionWaveDelay;
            ExplosionWaveStep = explosionWaveStep;
            MaxPlacedBombsCount = maxPlacedBombsCount;
        }

        public byte BombDelay 
        {
            get
            {
                return bombDelay;
            }

            set
            {
                if ((value > 0) && (value != bombDelay))
                    bombDelay = value;
            }
        }

        public int ExplosionWaveDistance
        {
            get
            {
                return explosionWaveDistance;
            }

            set
            {
                explosionWaveDistance = ((value >= ExplosionWaveFieldObjectBehaviour.minExplosionWaveDistance) && (value <= ExplosionWaveFieldObjectBehaviour.maxExplosionWaveDistance)) ?
                    value : ExplosionWaveFieldObjectBehaviour.minExplosionWaveDistance;
            }
        }

        public float ExplosionWaveDelay
        {
            get
            {
                return explosionWaveDelay;
            }

            set
            {
                explosionWaveDelay = ((value >= ExplosionWaveBehaviour.minExplosionWaveDelay) && (value <= ExplosionWaveBehaviour.maxExplosionWaveDelay)) ?
                    value : ExplosionWaveBehaviour.minExplosionWaveDelay;
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
                explosionWaveStep = ((value > ExplosionWaveFieldObjectBehaviour.minExplosionWaveStep) && (value <= ExplosionWaveDistance)) ?
                    value : ExplosionWaveFieldObjectBehaviour.minExplosionWaveStep;
            }
        }

        public int MaxPlacedBombsCount
        {
            get
            {
                return maxPlacedBombsCount;
            }

            set
            {
                if ((value >= 1) && (maxPlacedBombsCount != value))
                    maxPlacedBombsCount = value;
            }
        }

        public GameObject BombPrefab 
        {
            get
            {
                return bombPrefab;
            }
            set
            {
                if ((value != null) && (bombPrefab != value))
                    bombPrefab = value;
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

        protected void SetBomb(Field field, Vector2 fieldIndexes)
        {
            if (field.GetBombGameObjectsCount() < MaxPlacedBombsCount)
            {
                int fieldIndexesX = (int)fieldIndexes.x;
                int fieldIndexesY = (int)fieldIndexes.y;

                if ((field != null) && ((!field.IsIndexesOutOfFieldIndexesRanges(fieldIndexesX, fieldIndexesY)) && 
                    (field.FieldObjects[fieldIndexesX][fieldIndexesY].ObjectType != FieldObjectType.PlayerAndBreakableWall)) && (field.FieldDynamicObjectsGenerator != null))
                {
                    Animator animator = field.FieldObjectsComponentsGetter.GetPlayerGameObject().GetComponent<Animator>();

                    if (!field.IsBombGameObjectsContainsKey(fieldIndexesX, fieldIndexesY) && (animator != null))
                    {
                        field.AddBombGameObject((int)fieldIndexes.x, (int)fieldIndexes.y, field.FieldDynamicObjectsGenerator.CreateBomb(BombPrefab, ExplosionWavePartPrefab, fieldIndexes, BombDelay,
                                                ExplosionWaveDistance, ExplosionWaveStep, ExplosionWaveDelay));
                        {
                            animator.Play("Sit Down");
                            PlayFieldObjectSound(field, field.FieldObjectsComponentsGetter.GetPlayerGameObject(), "Set Bomb");
                        }
                    }
                }
            }
        }

        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            if (parameter == null)
                SetBomb(field, fieldIndexes);
        }

        public bool CanIncreaseExplosionWaveDistance()
        {
            return !(ExplosionWaveDistance == ExplosionWaveFieldObjectBehaviour.maxExplosionWaveDistance);
        }
    }
}
