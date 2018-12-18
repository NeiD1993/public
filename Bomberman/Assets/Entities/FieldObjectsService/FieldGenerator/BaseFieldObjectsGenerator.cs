using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using Assets.Scripts.Behaviour.BaseBehaviour;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using UnityEngine;

namespace Assets.Entities.FieldObjectsService.FieldGenerator
{
    abstract class BaseFieldObjectsGenerator : BaseChangableFieldObjectsService
    {
        public BaseFieldObjectsGenerator(Field field) : base(field) { }

        protected static FieldObjectBehaviourType CreateFieldObjectBehaviour<FieldObjectBehaviourType>() where FieldObjectBehaviourType : BaseFieldObjectBehaviour
        {
            FieldObjectBehaviourType fieldObjectBehaviour = ScriptableObject.CreateInstance<FieldObjectBehaviourType>();
            return fieldObjectBehaviour;
        }

        protected void ChangeGameObjectOnField(int horizontalIndex, int verticalIndex, FieldObjectType objectType, GameObject gameObjectPrefab = null, Material gameObjectMaterial = null)
        {
            FieldObject fieldObject;
            GameObject gameObject = null;

            if (gameObjectPrefab != null)
            {
                gameObject = CreateGameObject(gameObjectPrefab, Field.FieldGameObjects, gameObjectMaterial);
                ChangeGameObjectPosition(horizontalIndex, verticalIndex, gameObject);
            }

            fieldObject = Field.FieldObjects[horizontalIndex][verticalIndex];
            fieldObject.ObjectType = objectType;
            fieldObject.GameObject = gameObject;
        }

        protected FieldObjectBehaviourType AddBehaviourToFieldObject<FieldObjectBehaviourType>(GameObject fieldObjectPrefab) where FieldObjectBehaviourType : BaseBehaviour
        {
            FieldObjectBehaviourType fieldObjectBehaviour = fieldObjectPrefab.GetComponent<FieldObjectBehaviourType>();

            if (fieldObjectBehaviour == null)
                fieldObjectBehaviour = fieldObjectPrefab.AddComponent<FieldObjectBehaviourType>();

            fieldObjectBehaviour.Init(Field);

            return fieldObjectBehaviour;
        }

        protected FieldObjectBehaviourType AddBehaviourToFieldObject<FieldObjectBehaviourType>(GameObject fieldObjectPrefab, Vector2 freePosition) where FieldObjectBehaviourType : BaseFieldBehaviour
        {
            FieldObjectBehaviourType fieldObjectBehaviour = AddBehaviourToFieldObject<FieldObjectBehaviourType>(fieldObjectPrefab);
            fieldObjectBehaviour.Init(freePosition);

            return fieldObjectBehaviour;
        }

        protected BaseFieldBehaviour AddContinuedFieldObjectBehaviour<ContinuedBehaviourType>(GameObject fieldObjectPrefab, BaseFieldObjectBehaviour fieldObjectBehaviour, Vector2 freePosition,
                                                                                object delay)
            where ContinuedBehaviourType : ContinuedBehaviour
        {
            BaseFieldBehaviour behaviour = AddBehaviourToFieldObject<ContinuedBehaviourType>(fieldObjectPrefab, freePosition);
            behaviour.AddFieldObjectBehaviour<BaseFieldObjectBehaviour>(fieldObjectBehaviour);
            ((ContinuedBehaviourType)behaviour).SetDelay(new object[] { delay });

            return behaviour;
        }

        public static GameObject CreateGameObject(GameObject gameObjectPrefab, GameObject parentGameObject = null, Material gameObjectMaterial = null)
        {
            GameObject gameObject = GameObject.Instantiate(gameObjectPrefab);
            if (gameObjectMaterial != null)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                renderer.material = gameObjectMaterial;
            }

            if (parentGameObject != null)
                gameObject.transform.SetParent(parentGameObject.transform);

            return gameObject;
        }
    }
}
