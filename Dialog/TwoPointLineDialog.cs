using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class TwoPointLineDialog : InterfaceDialog
    {
        private ButtonWidget OKButton;
        public TwoPointLineDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/通用界面3");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            this.Children.Find<LabelWidget>("名称", true).Text = "2点连线";
            this.OKButton = this.Children.Find<ButtonWidget>("确定", true);
        }
        public override void Update()
        {
            base.Update();
            if (this.OKButton.IsClicked)
            {
                Task.Run(() =>
                {
                    int num = 0;
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.TwoPointLineGeneration2(creatorAPI.Position[0], creatorAPI.Position[1]))
                    {
                        creatorAPI.CreateBlock(point3, this.blockIconWidget.Value, chunkData);
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
