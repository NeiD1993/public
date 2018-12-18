using UnityEngine.UI;

namespace Assets.Scripts.Behaviour.ContinuedBehaviour
{
    class StatisticsBehaviour : BaseBehaviour.BaseBehaviour
    {
        protected override void Start() { }

        protected override void Update()
        {
            Text[] texts = GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                switch (text.tag)
                {
                    case "Kill Points":
                        text.text = "Kill Points: " + Field.FieldObjectsStatistics.CurrentPlayerKillPoints.ToString();
                        break;
                    case "Wall Pass":
                        text.text = "Wall Pass: " + Field.FieldObjectsStatistics.CurrentPlayerWallPass.ToString();
                        break;
                    case "Moving Speed":
                        text.text = "Moving Speed: " + Field.FieldObjectsStatistics.CurrentPlayerMovingSpeed.ToString();
                        break;
                    case "Explosion Wave Distance":
                        text.text = "Explosion Wave Distance: " + Field.FieldObjectsStatistics.CurrentExplosionWaveDistance.ToString();
                        break;
                    default:
                        text.text = "Remained Bombs: " + Field.FieldObjectsStatistics.RemainedPlacedBombsCount.ToString();
                        break;
                }
            }
        }
    }
}
