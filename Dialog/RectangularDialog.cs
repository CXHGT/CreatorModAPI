using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class RectangularDialog : InterfaceDialog
    {
        private ButtonWidget SoildButton;
        private ButtonWidget HollowButton;
        private ButtonWidget FrameworkButton;
        public RectangularDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/矩形界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            this.SoildButton = this.Children.Find<ButtonWidget>("实心", true);
            this.HollowButton = this.Children.Find<ButtonWidget>("空心", true);
            this.FrameworkButton = this.Children.Find<ButtonWidget>("框架", true);
        }

        public override void Update()
        {
            base.Update();
            if (this.SoildButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(creatorAPI);
                    creatorAPI.revokeData = new ChunkData(creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Rectangular(creatorAPI.Position[0], creatorAPI.Position[1]))
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
                    ChunkData chunkData = new ChunkData(creatorAPI);
                    creatorAPI.revokeData = new ChunkData(creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Rectangular(creatorAPI.Position[0], creatorAPI.Position[1],true))
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
            if (this.FrameworkButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(creatorAPI);
                    creatorAPI.revokeData = new ChunkData(creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Rectangular(creatorAPI.Position[0], creatorAPI.Position[1],false))
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