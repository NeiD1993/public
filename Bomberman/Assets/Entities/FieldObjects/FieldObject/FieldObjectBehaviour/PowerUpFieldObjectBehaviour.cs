using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    class PowerUpFieldObjectBehaviour : BaseFieldObjectBehaviour
    {
        public void Init(Vector3 rotationAngle)
        {
            RotationAngle = rotationAngle;
        }

        public Vector3 RotationAngle { get; set; }

        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            if (field.PowerUps.ContainsKey(fieldIndexes))
                field.FieldDynamicObjectsMover.RotateFieldGameObject(field.PowerUps[fieldIndexes].GameObject, RotationAngle);
        }
    }
}
