using Loaders.Enums;
using UnityEngine.SceneManagement;

namespace Loaders
{
    public static class SceneLoader
    {
        public static void LoadScene(SceneKind sceneKind)
        {
            SceneManager.LoadSceneAsync(sceneKind.ToString());
        }
    }
}

namespace Loaders.Enums
{
    public enum SceneKind
    {
        GameScene,

        MenuScene
    }
}