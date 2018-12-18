using Assets.Entities.FieldObjects;

namespace Assets.Entities.FieldObjectsService.BaseFieldObjectService
{
    class BaseFieldObjectsService
    {
        protected Field field;

        public BaseFieldObjectsService(Field field)
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
                if ((field == null) && (value != null))
                    field = value;
            }
        }
    }
}
