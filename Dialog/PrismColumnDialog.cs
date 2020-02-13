using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class PrismColumnDialog : PillarsDialog
    {
        public PrismColumnDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            this.Children.Find<LabelWidget>("名称", true).Text = "棱柱";
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
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.PrismColumn(creatorAPI.Position[0], (int)Radius.Value, (int)Height.Value, this.creatorType,typeBool))
                    {
                        creatorAPI.CreateBlock(point3, id,chunkData);
                        num++;
                        if (!creatorAPI.launch) return;
                    }
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
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.PrismColumn(creatorAPI.Position[0], (int)Radius.Value, (int)Height.Value, this.creatorType,typeBool, true))
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