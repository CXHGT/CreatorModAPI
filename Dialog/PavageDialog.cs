using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class PavageDialog : Dialog
    {
        private CreatorAPI creatorAPI;
        private ComponentPlayer player;
        private ButtonWidget OKButton;
        private TextBoxWidget TextBox;
        private SliderWidget slider;

        public PavageDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/平铺界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.OKButton = Children.Find<ButtonWidget>("确定", true);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            this.TextBox = Children.Find<TextBoxWidget>("方块ID", true);
            this.slider = Children.Find<SliderWidget>("滑条1", true);
        }

        private ButtonWidget cancelButton;

        public override void Update()
        {
            if (slider.IsSliding)
            {
                slider.Value = (int)slider.Value;
            }
            this.slider.Text = "大小:" + (int)slider.Value;
            if (this.OKButton.IsClicked)
            {
                Point3 Start = creatorAPI.Position[0];
                Point3 End = creatorAPI.Position[1];
                CreatorMain.Math.StartEnd(ref Start, ref End);
                try
                {
                    List<int> BlockIDs = new List<int>();
                    foreach (string BlockIDString in TextBox.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        BlockIDs.Add(int.Parse(BlockIDString));
                    }
                    Task.Run(delegate
                    {
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        int num = 0;
                        for (int x = End.X; x <= Start.X; x++)
                        {
                            for (int y = End.Y; y <= Start.Y; y++)
                            {
                                for (int z = End.Z; z <= Start.Z; z++)
                                {
                                    if (!creatorAPI.launch) return;
                                    int mx = (x - End.X) / (int)slider.Value;
                                    int my = (y - End.Y) / (int)slider.Value;
                                    int mz = (z - End.Z) / (int)slider.Value;
                                    int blockID = BlockIDs[(mx + my + mz) % (int)BlockIDs.Count];
                                    creatorAPI.CreateBlock(x, y, z, blockID,chunkData);
                                    num++;
                                }
                            }
                        }
                        chunkData.Render();
                        this.player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                    });
                }
                catch
                {
                    this.player.ComponentGui.DisplaySmallMessage("操作失败", true, true);
                }
                DialogsManager.HideDialog(this);
            }
            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
        }
    }
}
