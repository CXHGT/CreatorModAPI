using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class CircleDialog : PillarsDialog
    {
        public CircleDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.Children.Find<LabelWidget>("名称", true).Text = "圆环";
            Y_Shaft.Text = "Y轴";
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
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Circle(creatorAPI.Position[0], (int)Height.Value, (int)Radius.Value, this.creatorType))
                    {
                        creatorAPI.CreateBlock(point3,id,chunkData);
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
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Circle(creatorAPI.Position[0], (int)Height.Value, (int)Radius.Value, this.creatorType, true))
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
        public override void upDataButton(CreatorMain.CreatorType creatorType, ButtonWidget button)
        {
            if (this.creatorType != creatorType)
            {
                this.creatorType = creatorType;
                button.Color = Color.Green;
                if (X_Shaft != button) X_Shaft.Color = Color.White;
                if (Y_Shaft != button) Y_Shaft.Color = Color.White;
                if (Z_Shaft != button) Z_Shaft.Color = Color.White;
            }
        }
    }
}