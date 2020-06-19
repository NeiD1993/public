using Loaders;
using Loaders.Enums;

namespace MenuScene.Behaviours.Button
{
    public class StartButtonBehaviour : BaseButtonBehaviour
    {
        public override void ProcessClick()
        {
            SceneLoader.LoadScene(SceneKind.GameScene);
        }
    }
}