using System.Linq;
using GameScene.Services.ControlPanel.Enums;
using GameScene.Services.ControlPanel.Info;
using UnityEngine;

namespace GameScene.Services.ControlPanel
{
    public class PanelsAccessor : BaseSharedService
    {
        public GameObject GetPanel(PanelInfo panelInfo)
        {
            return GameObject.FindGameObjectsWithTag(panelInfo.Type.ToString()).First(panelParameter => panelParameter.name == panelInfo.Name);
        }
    }
}

namespace GameScene.Services.ControlPanel.Enums
{
    public enum PanelType
    {
        ButtonsPanel,

        ControlPanel,

        ScoreItemsPanel
    }
}

namespace GameScene.Services.ControlPanel.Info
{
    public struct PanelInfo
    {
        public PanelInfo(PanelType type, string name)
        {
            Type = type;
            Name = name;
        }

        public PanelType Type { get; private set; }

        public string Name { get; private set; }
    }
}