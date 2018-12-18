using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject
{
    class PowerUp
    {
        private Field field;

        private GameObject gameObject;

        public PowerUp(PowerUpTypes powerUpType, Field field, GameObject gameObject)
        {
            PowerUpType = powerUpType;
            Field = field;
            GameObject = gameObject;
        }

        public PowerUpTypes PowerUpType { get; set; }

        public Field Field
        {
            get
            {
                return field;
            }

            set
            {
                if ((field != value) && (value != null))
                    field = value;
            }
        }

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }

            set
            {
                if ((gameObject != value) && (value != null))
                    gameObject = value;
            }
        }
    }
}
