using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class SpiralDialog : PillarsDialog
    {
        private SliderWidget Number;
        private LabelWidget numberLabelWidget;

        public SpiralDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.Children.Find<LabelWidget>("名称", true).Text = "螺旋";
            this.Children.Find<StackPanelWidget>("Data3", true).IsVisible = true;
            this.Number = this.Children.Find<SliderWidget>("滑条3", true);
            this.numberLabelWidget = this.Children.Find<LabelWidget>("滑条3数据", true);
            this.SoildButton.Text = "确定";
            this.HollowButton.IsVisible = false;
        }

        public override void Update()
        {
            base.Update();
            this.radiusDelayLabel.Text = $"间距{(int)Radius.Value}格";
            this.numberLabelWidget.Text = $"圈数{(int)Number.Value}环";
        }

        public override void upClickButton(int id)
        {
            if (this.SoildButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Spiral(creatorAPI.Position[0], (int)Height.Value, (int)Radius.Value, (int)Number.Value,creatorType,typeBool))
                    {
                        if (!creatorAPI.launch) return;
                        creatorAPI.CreateBlock(point3, id,chunkData);
                        num++;
                    }
                    chunkData.Render();
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                });
                DialogsManager.HideDialog(this);
            }
        }
    }
}