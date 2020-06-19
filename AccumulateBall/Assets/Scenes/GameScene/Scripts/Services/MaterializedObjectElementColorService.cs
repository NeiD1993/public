using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Services.Colors
{
    public class MaterializedObjectElementColorService : BaseSharedService
    {
        private static readonly string colorPropertyName;

        static MaterializedObjectElementColorService()
        {
            colorPropertyName = string.Concat((char)95, nameof(Color));
        }

        private static void PerformMaterialPropertyBlockColorActions(GameObject gameObject, IEnumerable<Action<MaterialPropertyBlock>> colorActions, bool setPropertyBlock = true)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.GetPropertyBlock(materialPropertyBlock);

            foreach (Action<MaterialPropertyBlock> colorAction in colorActions)
                colorAction(materialPropertyBlock);

            if (setPropertyBlock)
                meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetElementColor(UnityEngine.Object element, Color color)
        {
            void SetGameObjectColor(GameObject gameObject)
            {
                PerformMaterialPropertyBlockColorActions(gameObject, new Action<MaterialPropertyBlock>[]
                {
                    (materialPropertyBlockParameter) => materialPropertyBlockParameter.SetColor(colorPropertyName, color)
                });
            }

            void SetTextColor(Text text)
            {
                text.color = color;
            }

            if (element is GameObject gameObjectElement)
                SetGameObjectColor(gameObjectElement);
            else if (element is Text textElement)
                SetTextColor(textElement);
        }

        public Color GetElementColor(UnityEngine.Object element)
        {
            Color GetGameObjectColor(GameObject gameObject)
            {
                Color gameObjectColor = default;

                PerformMaterialPropertyBlockColorActions(gameObject, new Action<MaterialPropertyBlock>[]
                {
                    (materialPropertyBlockParameter) => gameObjectColor = materialPropertyBlockParameter.GetColor(colorPropertyName)
                }, false);

                return gameObjectColor;
            }

            Color color = default;

            if (element is GameObject gameObjectElement)
                color = GetGameObjectColor(gameObjectElement);

            return color;
        }
    }
}