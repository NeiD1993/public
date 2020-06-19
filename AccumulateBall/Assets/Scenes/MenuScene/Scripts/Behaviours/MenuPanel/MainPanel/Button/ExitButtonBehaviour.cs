using UnityEngine;

namespace MenuScene.Behaviours.Button
{
    public class ExitButtonBehaviour : BaseButtonBehaviour
    {
        public override void ProcessClick()
        {
            Application.Quit();
        }
    }
}