using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class ReplaceDialog : Dialog
    {
        private ButtonWidget cancelButton;

        private ButtonWidget replaceButton;

        private ButtonWidget retainReplaceButton;

        private ButtonWidget roughReplaceButton;

        private TextBoxWidget Blockid, Blockid2;

        private ComponentPlayer player;

        private CreatorAPI creatorAPI;

        private SubsystemTerrain subsystemTerrain = GameManager.Project.FindSubsystem<SubsystemTerrain>();
        public ReplaceDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer ;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/替换界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            this.replaceButton = Children.Find<ButtonWidget>("替换", true);
            this.retainReplaceButton = Children.Find<ButtonWidget>("保留替换");
            this.roughReplaceButton = Children.Find<ButtonWidget>("粗糙替换");
            this.Blockid = Children.Find<TextBoxWidget>("方块ID", true);
            this.Blockid2 = Children.Find<TextBoxWidget>("方块ID2", true);
        }

        public override void Update()
        {
            if (this.cancelButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
            if (this.roughReplaceButton.IsClicked)
            {
                Regex regex = new Regex("^[0-9]+$");
                if (Blockid2.Text == "")
                {
                    Blockid2.Text = "0";
                }
                Match match = regex.Match(this.Blockid.Text);
                Match match2 = regex.Match(this.Blockid2.Text);
                if (match.Success && match2.Success)
                {
                    Point3 Start = creatorAPI.Position[0];
                    Point3 End = creatorAPI.Position[1];
                    CreatorMain.Math.StartEnd(ref Start, ref End);
                    Task.Run(() =>
                    {
                        int num = 0;
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        for (int x = 0; x <= Start.X - End.X; x++)
                        {
                            for (int y = 0; y <= Start.Y - End.Y; y++)
                            {
                                for (int z = 0; z <= Start.Z - End.Z; z++)
                                {
                                    int id = this.subsystemTerrain.Terrain.GetCellValueFast(End.X + x, End.Y + y, End.Z + z);
                                    if (!creatorAPI.AirIdentify && Terrain.ExtractContents(id) == 0) continue;
                                    if (id == int.Parse(this.Blockid.Text) || Terrain.ExtractContents(id) == int.Parse(this.Blockid.Text))
                                    {
                                        if (!creatorAPI.launch) return;
                                        creatorAPI.CreateBlock(End.X + x, End.Y + y, End.Z + z, int.Parse(this.Blockid2.Text),chunkData);
                                        num++;
                                    }
                                }
                            }
                        }
                        chunkData.Render();
                        this.player.ComponentGui.DisplaySmallMessage($"操作成功，共替换{num}个方块", true, true);
                    });
                }
                else
                {
                    this.player.ComponentGui.DisplaySmallMessage("操作失败", true, true);
                }
                DialogsManager.HideDialog(this);
            }



            if (this.replaceButton.IsClicked || this.retainReplaceButton.IsClicked)
            {
                bool type = replaceButton.IsClicked;
                Regex regex = new Regex("^[0-9]+$");
                if (Blockid2.Text == "")
                {
                    Blockid2.Text = "0";
                }
                Match match = regex.Match(this.Blockid.Text);
                Match match2 = regex.Match(this.Blockid2.Text);
                if (match.Success && match2.Success)
                {
                    Point3 Start = creatorAPI.Position[0];
                    Point3 End = creatorAPI.Position[1];
                    CreatorMain.Math.StartEnd(ref Start, ref End);
                    Task.Run(() =>
                    {
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        int num = 0;
                        for (int x = 0; x <= Start.X - End.X; x++)
                        {
                            for (int y = 0; y <= Start.Y - End.Y; y++)
                            {
                                for (int z = 0; z <= Start.Z - End.Z; z++)
                                {
                                    int id = this.subsystemTerrain.Terrain.GetCellValueFast(End.X + x, End.Y + y, End.Z + z);
                                    if (!creatorAPI.AirIdentify && Terrain.ExtractContents(id) == 0) continue;
                                    if (type)
                                    {
                                        if (id == int.Parse(this.Blockid.Text))
                                        {
                                            if (!creatorAPI.launch) return;
                                            creatorAPI.CreateBlock(End.X + x, End.Y + y, End.Z + z, int.Parse(this.Blockid2.Text),chunkData);
                                            num++;
                                        }
                                    }
                                    else
                                    {
                                        if (id != int.Parse(this.Blockid.Text))
                                        {
                                            if (!creatorAPI.launch) return;
                                            creatorAPI.CreateBlock(End.X + x, End.Y + y, End.Z + z, int.Parse(this.Blockid2.Text),chunkData);
                                            num++;
                                        }
                                    }
                                }
                            }
                        }
                        chunkData.Render();
                        this.player.ComponentGui.DisplaySmallMessage($"操作成功，共替换{num}个方块", true, true);
                    });
                }
                else
                {
                    this.player.ComponentGui.DisplaySmallMessage("操作失败", true, true);
                }
                DialogsManager.HideDialog(this);
            }
        }



    }
}
