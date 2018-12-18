using Assets.Entities.FieldObjects;
using UnityEngine;

namespace Assets.Scripts.Behaviour.BaseBehaviour
{
    abstract class BaseBehaviour : MonoBehaviour
    {
        [SerializeField][HideInInspector]
        private Field field;

        public void Init(Field field)
        {
            Field = field;
        }

        public Field Field
        {
            get
            {
                return field;
            }

            set
            {
                if (field != value)
                    field = value;
            }
        }

        protected abstract void Start();

        protected abstract void Update();
    }
}
