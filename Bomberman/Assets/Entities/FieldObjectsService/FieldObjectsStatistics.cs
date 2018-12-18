using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour;
using System.Collections.Generic;

namespace Assets.Entities.FieldObjecsService.PlayerFieldObjectStatistics
{
    class FieldObjectsStatistics : BaseFieldObjectsService
    {
        private IDictionary<string, int> enemiesKillPoints;

        protected int currentPlayerKillPoints;

        public FieldObjectsStatistics(Field field, IDictionary<string, int> enemiesKillPoints) : base(field) 
        {
            EnemiesKillPoints = enemiesKillPoints;
        }

        public int CurrentPlayerKillPoints
        {
            get
            {
                return currentPlayerKillPoints;
            }

            set
            {
                if ((value >= 0) && (currentPlayerKillPoints != value))
                    currentPlayerKillPoints = value;
            }
        }

        public bool CurrentPlayerWallPass
        {
            get
            {
                if ((Field != null) && (Field.PlayerPosition != null))
                {
                    PlayerBehaviour playerBehaviour = Field.FieldObjectsComponentsGetter.GetPlayerBehaviour<PlayerBehaviour>();
                    PlayerFieldObjectMovingBehaviour playerFieldObjectMovingBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectMovingBehaviour>();

                    return playerFieldObjectMovingBehaviour.IsWallPassed;
                }
                else
                    return false;
            }
        }

        public int CurrentPlayerMovingSpeed
        {
            get
            {
                if ((Field != null) && (Field.PlayerPosition != null))
                {
                    PlayerBehaviour playerBehaviour = Field.FieldObjectsComponentsGetter.GetPlayerBehaviour<PlayerBehaviour>();
                    PlayerFieldObjectMovingBehaviour playerFieldObjectMovingBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectMovingBehaviour>();

                    return playerFieldObjectMovingBehaviour.MovingSpeed;
                }
                else
                    return 0;
            }
        }

        public int CurrentExplosionWaveDistance
        {
            get
            {
                if ((Field != null) && (Field.PlayerPosition != null))
                {
                    PlayerBehaviour playerBehaviour = Field.FieldObjectsComponentsGetter.GetPlayerBehaviour<PlayerBehaviour>();
                    PlayerFieldObjectBombBehaviour playerFieldObjectMovingBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectBombBehaviour>();

                    return playerFieldObjectMovingBehaviour.ExplosionWaveDistance;
                }
                else
                    return 0;
            }
        }

        public int RemainedPlacedBombsCount
        {
            get
            {
                if ((Field != null) && (Field.PlayerPosition != null))
                {
                    PlayerBehaviour playerBehaviour = Field.FieldObjectsComponentsGetter.GetPlayerBehaviour<PlayerBehaviour>();
                    PlayerFieldObjectBombBehaviour playerFieldObjectMovingBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectBombBehaviour>();

                    return (playerFieldObjectMovingBehaviour.MaxPlacedBombsCount - Field.GetBombGameObjectsCount());
                }
                else
                    return 0;
            }
        }

        public IDictionary<string, int> EnemiesKillPoints 
        {
            get
            {
                return enemiesKillPoints;
            }

            set
            {
                if ((enemiesKillPoints != value) && (value != null))
                    enemiesKillPoints = value;
            }
        }
    }
}
