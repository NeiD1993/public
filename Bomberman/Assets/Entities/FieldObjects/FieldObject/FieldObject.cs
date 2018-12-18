using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject
{
    class FieldObject
    {
        private FieldObjectType objectType;

        private Field field;

        private GameObject gameObject;

        public FieldObject(FieldObjectType objectType, Field field, GameObject gameObject)
        {
            ObjectType = objectType;
            Field = field;
            GameObject = gameObject;
        }

        public FieldObjectType ObjectType
        {
            get
            {
                return objectType;
            }

            set
            {
                if (objectType != value)
                {
                    if (value == FieldObjectType.Empty)
                        gameObject = null;
                    objectType = value;
                }
            }
        }

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
                if (gameObject != value)
                {
                    if (value == null)
                        objectType = FieldObjectType.Empty;
                    gameObject = value;
                }
            }
        }
    }
}
