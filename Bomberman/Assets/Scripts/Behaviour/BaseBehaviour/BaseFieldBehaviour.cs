using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Behaviour.BaseBehaviour
{
    abstract class BaseFieldBehaviour : BaseBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private Vector2 fieldIndexes;

        [SerializeField]
        [HideInInspector]
        private List<BaseFieldObjectBehaviour> fieldObjectBehaviours;

        [SerializeField]
        [HideInInspector]
        protected bool isTimerOn;

        public void Init(Vector2 fieldIndexes)
        {
            FieldIndexes = fieldIndexes;
            fieldObjectBehaviours = new List<BaseFieldObjectBehaviour>();
        }

        public Vector2 FieldIndexes
        {
            get
            {
                return fieldIndexes;
            }

            set
            {
                if ((Field != null) && (!Field.IsIndexesOutOfFieldIndexesRanges((int)value.x, (int)value.y)))
                    fieldIndexes = value;
            }
        }

        protected void Execute(object parameter)
        {
            fieldObjectBehaviours.ForEach(fieldObjectBehaviour => fieldObjectBehaviour.TryExecute(Field, ref fieldIndexes, parameter));
        }

        protected void ExecuteOnTimer(FieldObjectType fieldObjectType)
        {
            if (!isTimerOn)
            {
                Execute(new object[] { gameObject, fieldObjectType });
                StartCoroutine(Field.FieldObjectsDestroyer.DestroyGameObjectInTimer(gameObject));
                isTimerOn = true;
            }
        }

        public void AddFieldObjectBehaviour<FieldObjectBehaviourType>(FieldObjectBehaviourType fieldObjectBehaviour) where FieldObjectBehaviourType : BaseFieldObjectBehaviour
        {
            if (fieldObjectBehaviours.FindAll(addedFieldObjectBehaviour => addedFieldObjectBehaviour is FieldObjectBehaviourType).Count == 0)
                fieldObjectBehaviours.Add(fieldObjectBehaviour);
        }

        public void ClearFieldObjectBehaviours()
        {
            fieldObjectBehaviours.Clear();
        }

        public FieldObjectBehaviourType GetFieldObjectBehaviour<FieldObjectBehaviourType>() where FieldObjectBehaviourType : BaseFieldObjectBehaviour
        {
            BaseFieldObjectBehaviour containedFieldObjectsBehaviour = fieldObjectBehaviours.Find(addedFieldObjectBehaviour => addedFieldObjectBehaviour is FieldObjectBehaviourType);
            if (containedFieldObjectsBehaviour != null)
                return (FieldObjectBehaviourType)containedFieldObjectsBehaviour;
            else
                return null;
        }

        public void RemoveFieldObjectBehaviour(BaseFieldObjectBehaviour fieldObjectBehaviour)
        {
            fieldObjectBehaviours.Remove(fieldObjectBehaviour);
        }
    }
}
