using System.Collections.Generic;
using GameScene.Behaviours.ScoreItem;
using GameScene.Behaviours.ScoreItem.Enums;
using GameScene.Services.Status;
using UnityEngine;

namespace GameScene.Services.ScoreItems
{
    public class ScoreItemsSummaryInformingStatusFormer : BaseSummaryStatusFormer<GameObject, ICollection<GameObject>, InformingStatus>
    {
        public override InformingStatus FormSummaryStatus(ICollection<GameObject> elements)
        {
            ScoreItemBehaviour scoreItemBehaviour;
            (int active, int prepared) informingStatusesStatistics = (0, 0);

            foreach (GameObject scoreItem in elements)
            {
                scoreItemBehaviour = scoreItem.GetComponent<ScoreItemBehaviour>();

                if (scoreItemBehaviour.InformingStatus == InformingStatus.Active)
                    informingStatusesStatistics.active++;
                else if (scoreItemBehaviour.InformingStatus == InformingStatus.Prepared)
                    informingStatusesStatistics.prepared++;
            }

            if (informingStatusesStatistics.active == elements.Count)
                return InformingStatus.Active;
            else if (informingStatusesStatistics.prepared == elements.Count)
                return InformingStatus.Prepared;
            else
                return InformingStatus.NonActive;
        }
    }
}