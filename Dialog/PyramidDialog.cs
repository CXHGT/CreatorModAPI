using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class PyramidDialog : InterfaceDialog
    {
        private SliderWidget Radius;
        private LabelWidget delayLabel;
        private ButtonWidget SoildButton;
        private ButtonWidget HollowButton;
        public PyramidDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/棱体棱锥界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            this.Children.Find<StackPanelWidget>("XYZ", true).IsVisible = false;
            this.Children.Find<LabelWidget>("名称", true).Text = "棱锥";
            this.Radius = this.Children.Find<SliderWidget>("滑条", true);
            this.delayLabel = this.Children.Find<LabelWidget>("滑条数据", true);
            this.SoildButton = this.Children.Find<ButtonWidget>("实心", true);
            this.HollowButton = this.Children.Find<ButtonWidget>("空心", true);
        }

        public override void Update()
        {
            base.Update();
            this.delayLabel.Text = $"大小{(int)this.Radius.Value}块";
            if (this.SoildButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Pyramid(creatorAPI.Position[0], (int)Radius.Value))
                    {
                        if (!creatorAPI.launch) return;
                        creatorAPI.CreateBlock(point3, this.blockIconWidget.Value,chunkData);
                        num++;
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
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Pyramid(creatorAPI.Position[0], (int)Radius.Value, true))
                    {
                        if (!creatorAPI.launch) return;
                        creatorAPI.CreateBlock(point3, this.blockIconWidget.Value,chunkData);
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