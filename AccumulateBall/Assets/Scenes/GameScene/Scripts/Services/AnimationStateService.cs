using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Services.Animation
{
    public class AnimationStateService : BaseSharedService
    {
        public bool IsStateOfTag(AnimatorStateInfo stateInfo, Enum tag)
        {
            return stateInfo.IsTag(tag.ToString());
        }

        public bool IsStateTagAmong(AnimatorStateInfo stateInfo, IEnumerable<Enum> tags)
        {
            bool result = false;
            IEnumerator<Enum> tagsEnumerator = tags.GetEnumerator();

            while (!result && tagsEnumerator.MoveNext())
                result = IsStateOfTag(stateInfo, tagsEnumerator.Current);

            return result;
        }
    }
}