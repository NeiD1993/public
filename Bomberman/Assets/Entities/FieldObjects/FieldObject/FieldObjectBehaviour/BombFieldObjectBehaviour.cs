using Assets.Scripts.Behaviour.ContinuedBehaviour;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class BombFieldObjectBehaviour : BaseFieldObjectBehaviour
    {
        private GameObject explosionWavePartPrefab;

        private int explosionWaveDistance;

        private float explosionWaveDelay;

        private float explosionWaveStep;

        public void Init(GameObject explosionWavePartPrefab, int explosionWaveDistance, float explosionWaveDelay, float explosionWaveStep)
        {
            ExplosionWavePartPrefab = explosionWavePartPrefab;
            ExplosionWaveDistance = explosionWaveDistance;
            ExplosionWaveStep = explosionWaveStep;
            ExplosionWaveDelay = explosionWaveDelay;
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

        protected void Explode(Field field, Vector2 fieldIndexes)
        {
            if ((field != null))
            {
                field.FieldObjectsDestroyer.DestroyBomb(fieldIndexes);
                field.FieldDynamicObjectsGenerator.CreateExplosionWave(ExplosionWavePartPrefab, fieldIndexes, ExplosionWaveDistance, ExplosionWaveDelay, ExplosionWaveStep);
            }
        }

        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            Explode(field, fieldIndexes);
        }
    }
}
