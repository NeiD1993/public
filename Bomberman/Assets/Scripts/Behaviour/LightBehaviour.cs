using Assets.Entities.FieldObjectsService.FieldObjectsMover;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Behaviour
{
    class LightBehaviour : BaseBehaviour.BaseBehaviour
    {
        protected override void Start()
        {
            StartCoroutine(DestroyAssociatedGameObject());
        }

        protected override void Update()
        {
            Text text = gameObject.GetComponentInChildren<Text>();
            BaseFieldDynamicObjectsMover.MoveChildGameObject(gameObject, gameObject.transform.parent.gameObject);
            BaseFieldDynamicObjectsMover.MoveText(text, gameObject);

            text.enabled = true;
        }

        protected virtual IEnumerator DestroyAssociatedGameObject()
        {
            yield return new WaitForSecondsRealtime(Field.DestroyableActionDelay);
            Field.FieldObjectsDestroyer.DestroyLight();
        }
    }
}
