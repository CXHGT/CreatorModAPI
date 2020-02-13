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
    public class FillingDialog : InterfaceDialog
    {
        public bool typeBool = true;
        private ButtonWidget pillingButton;
        private ButtonWidget pilling2Button;

        public FillingDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/填充界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            base.setShaftXYZ();
            Y_Shaft.Text = "正Y轴";
            this.pillingButton = this.Children.Find<ButtonWidget>("填充", true);
            this.pilling2Button = this.Children.Find<ButtonWidget>("填充2", true);
        }
        public override void Update()
        {
            base.Update();
            if (this.pillingButton.IsClicked)
            {
                FillingBlock();
                DialogsManager.HideDialog(this);
            }
            if (this.pilling2Button.IsClicked)
            {
                FillingBlock(true);
                DialogsManager.HideDialog(this);
            }
            upDataButton();
        }
        public void FillingBlock(bool limit = false)
        {
            Point3 Start = creatorAPI.Position[0];
            Point3 End = creatorAPI.Position[1];
            CreatorMain.Math.StartEnd(ref Start,ref End);
            int bort;
            if (this.creatorType == CreatorMain.CreatorType.X)
            {
                bort = Start.X;
                Start.X = Start.Y;
                Start.Y = bort;
                bort = End.X;
                End.X = End.Y;
                End.Y = bort;
            }
            else if (this.creatorType == CreatorMain.CreatorType.Z)
            {
                bort = Start.Z;
                Start.Z = Start.Y;
                Start.Y = bort;
                bort = End.Z;
                End.Z = End.Y;
                End.Y = bort;
            }
            Task.Run(delegate
            {
                int num = 0;
                ChunkData chunkData = new ChunkData(this.creatorAPI);
                creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                for (int x = End.X; x <= Start.X; x++)
                {
                    for (int z = End.Z; z <= Start.Z; z++)
                    {
                        bool b = false;
                        bool c = false;
                        for (int y = End.Y; y <= Start.Y; y++)
                        {
                            if (!creatorAPI.launch) return;
                            int _y;
                            if (this.creatorType != CreatorMain.CreatorType.Y)
                            {
                                if (!typeBool)
                                {
                                    _y = Start.Y + End.Y - y;
                                }
                                else
                                {
                                    _y = y;
                                }
                            }
                            else
                            {
                                if (!this.typeBool)
                                {
                                    _y = y;
                                }
                                else
                                {
                                    _y = Start.Y + End.Y - y;
                                }
                            }
                            int id;
                            if (this.creatorType == CreatorMain.CreatorType.X)
                            {
                                id = this.subsystemTerrain.Terrain.GetCellValueFast(_y, x, z);
                            }
                            else if (this.creatorType == CreatorMain.CreatorType.Y)
                            {
                                id = this.subsystemTerrain.Terrain.GetCellValueFast(x, _y, z);
                            }
                            else
                            {
                                id = this.subsystemTerrain.Terrain.GetCellValueFast(x, z, _y);
                            }
                            int blockID = Terrain.ExtractContents(id);
                            if (c && limit && blockID != 0) break;
                            if (!b && blockID != 0)
                            {
                                b = true;
                            }
                            else if (b && blockID == 0)
                            {
                                c = true;
                                if (this.creatorType == CreatorMain.CreatorType.X)
                                {
                                    this.creatorAPI.CreateBlock(_y, x, z, this.blockIconWidget.Value,chunkData);
                                    num++;
                                }
                                else if (this.creatorType == CreatorMain.CreatorType.Y)
                                {
                                    this.creatorAPI.CreateBlock(x, _y, z, this.blockIconWidget.Value,chunkData);
                                    num++;
                                }
                                else
                                {
                                    this.creatorAPI.CreateBlock(x, z, _y, this.blockIconWidget.Value,chunkData);
                                    num++;
                                }
                            }
                        }
                    }
                }
                chunkData.Render();
                this.player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
            });
        }
        public override void upDataButton(CreatorMain.CreatorType creatorType, ButtonWidget button)
        {
            if (this.creatorType == creatorType)
            {
                if (typeBool)
                {
                    typeBool = false;
                    button.Text = $"负{getTypeName(creatorType)}轴";
                    button.Color = Color.Red;
                }
                else
                {
                    typeBool = true;
                    button.Text = $"正{getTypeName(creatorType)}轴";
                    button.Color = Color.Green;
                }
            }
            else
            {
                typeBool = true;
                this.creatorType = creatorType;
                button.Text = $"正{getTypeName(creatorType)}轴";
                button.Color = Color.Green;
                if (X_Shaft != button)
                {
                    X_Shaft.Text = "X轴";
                    X_Shaft.Color = Color.White;
                }
                if (Y_Shaft != button)
                {
                    Y_Shaft.Text = "Y轴";
                    Y_Shaft.Color = Color.White;
                }
                if (Z_Shaft != button)
                {
                    Z_Shaft.Text = "Z轴";
                    Z_Shaft.Color = Color.White;
                }
            }
        }


    }
}
