using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class CylindricalDialog : PillarsDialog
    {
        private SliderWidget ZRadius;
        private LabelWidget zRadiusLabelWidget;
        
        public CylindricalDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.Children.Find<LabelWidget>("名称", true).Text = "圆柱";
            this.Children.Find<StackPanelWidget>("Data3", true).IsVisible = true;
            this.ZRadius = this.Children.Find<SliderWidget>("滑条3", true);
            this.zRadiusLabelWidget = this.Children.Find<LabelWidget>("滑条3数据", true);
        }

        public override void Update()
        {
            base.Update();
            this.radiusDelayLabel.Text = $"X半径{(int)Radius.Value}格";
            this.zRadiusLabelWidget.Text = $"Z半径{(int)ZRadius.Value}格";
        }

        public override void upClickButton(int id)
        {
            if (this.SoildButton.IsClicked)
            {
                Task.Run(() =>
                {
                    int num = 0;
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Cylindrical(new Vector3(creatorAPI.Position[0]), (int)Radius.Value, (int)Height.Value,(int)ZRadius.Value, this.creatorType,typeBool))
                    {
                        creatorAPI.CreateBlock(point3, id,chunkData);
                        num++;
                        if (!creatorAPI.launch) return;
                    }
                    chunkData.Render();
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                });
                DialogsManager.HideDialog(this);
            }
            if (this.HollowButton.IsClicked)
            {
                Task.Run(() =>
                {
                    int num = 0;
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Cylindrical(new Vector3(creatorAPI.Position[0]), (int)Radius.Value, (int)Height.Value,(int)ZRadius.Value, this.creatorType,typeBool, true))
                    {
                        creatorAPI.CreateBlock(point3, id,chunkData);
                        num++;
                        if (!creatorAPI.launch) return;
                    }
                    chunkData.Render();
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                });
                DialogsManager.HideDialog(this);
            }
        }
    }
}