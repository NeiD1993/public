using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour
{
    abstract class BaseFieldObjectBehaviour : ScriptableObject
    {
        public abstract void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null);

        public virtual void PlayFieldObjectSound(Field field, GameObject fieldGameObject, string soundKey)
        {
            if (fieldGameObject != null)
            {
                AudioSource audioSource;

                if (fieldGameObject.transform.childCount > 0)
                    audioSource = fieldGameObject.GetComponentInChildren<AudioSource>();
                else
                    audioSource = fieldGameObject.GetComponent<AudioSource>();

                string playerWinSoundKey = "Player Win";
                string enemyWinSoundKey = "Enemy Win";

                if ((audioSource != null) && ((((soundKey == playerWinSoundKey) || (soundKey == enemyWinSoundKey)) && (!audioSource.isPlaying)) ||
                                              ((soundKey != playerWinSoundKey) && (soundKey != enemyWinSoundKey))))
                {
                    IDictionary<string, AudioClip> fieldObjectsSounds = field.FieldObjectsSounds;

                    if (fieldObjectsSounds.ContainsKey(soundKey))
                    {
                        audioSource.clip = fieldObjectsSounds[soundKey];
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
