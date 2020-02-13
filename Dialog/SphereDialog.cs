using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class SphereDialog : InterfaceDialog
    {
        private SliderWidget XRadius;
        private SliderWidget YRadius;
        private SliderWidget ZRadius;
        private LabelWidget XdelayLabel;
        private LabelWidget YdelayLabel;
        private bool advanced = false;
        private LabelWidget ZdelayLabel;
        private ButtonWidget SoildButton;
        private ButtonWidget HollowButton;
        private ButtonWidget AdvancedButton;
        private Widget AdvancedGenerate;

        private ButtonWidget DoublePositionButton;
        private bool DoublePosition = false;
        public SphereDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/球体界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            this.XRadius = this.Children.Find<SliderWidget>("X半径滑条", true);
            this.YRadius = this.Children.Find<SliderWidget>("Y半径滑条", true);
            this.ZRadius = this.Children.Find<SliderWidget>("Z半径滑条", true);
            this.XdelayLabel = this.Children.Find<LabelWidget>("X半径", true);
            this.YdelayLabel = this.Children.Find<LabelWidget>("Y半径", true);
            this.ZdelayLabel = this.Children.Find<LabelWidget>("Z半径", true);
            this.SoildButton = this.Children.Find<ButtonWidget>("实心", true);
            this.HollowButton = this.Children.Find<ButtonWidget>("空心", true);
            this.AdvancedButton = this.Children.Find<ButtonWidget>("高级");
            this.AdvancedGenerate = this.Children.Find<Widget>("高级生成");
            this.DoublePositionButton = this.Children.Find<ButtonWidget>("2点模式");
        }

        public override void Update()
        {
            base.Update();
            AdvancedButton.Color = advanced ? Color.Yellow : Color.White;
            DoublePositionButton.Color = DoublePosition ? Color.Yellow : Color.White;
            AdvancedGenerate.IsVisible = advanced;
            DoublePositionButton.IsVisible = advanced;
            this.XdelayLabel.Text = advanced ? $"X半径大小{(int)this.XRadius.Value}块" : $"半径大小{(int)this.XRadius.Value}块";
            this.YdelayLabel.Text = $"Y半径大小{(int)this.YRadius.Value}块";
            this.ZdelayLabel.Text = $"Z半径大小{(int)this.ZRadius.Value}块";
            int id = this.blockIconWidget.Value;
            if (DoublePositionButton.IsClicked) DoublePosition = !DoublePosition;
            if (AdvancedButton.IsClicked) advanced = !advanced;
            if (SoildButton.IsClicked)
            {
                if (advanced)
                {
                    if(!DoublePosition)
                    Task.Run(() =>
                    {
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        int num = 0;
                        foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(creatorAPI.Position[0]), (int)XRadius.Value, (int)YRadius.Value, (int)ZRadius.Value))
                        {
                            if (!creatorAPI.launch) return;
                            creatorAPI.CreateBlock(point3, id,chunkData);
                            num++;
                        }
                        chunkData.Render();
                        player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                    });
                    else
                    {
                        if(creatorAPI.Position[1].Y != -1)
                        {
                            Point3 Start = creatorAPI.Position[0];
                            Point3 End = creatorAPI.Position[1];
                            CreatorMain.Math.StartEnd(ref Start,ref End);
                            float x = Math.Abs((float)Start.X - End.X) / 2f;
                            float y = Math.Abs((float)Start.Y - End.Y) / 2f;
                            float z = Math.Abs((float)Start.Z - End.Z) / 2f;
                            Task.Run(() =>
                            {
                                ChunkData chunkData = new ChunkData(this.creatorAPI);
                                creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                                int num = 0;
                                foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(End.X+x,End.Y+y,End.Z+z), (int)x, (int)y, (int)z))
                                {
                                    if (!creatorAPI.launch) return;
                                    creatorAPI.CreateBlock(point3, id,chunkData);
                                    num++;
                                }
                                chunkData.Render();
                                player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                            });
                        }
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        int num = 0;
                        foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(creatorAPI.Position[0]), (int)XRadius.Value, (int)XRadius.Value, (int)XRadius.Value))
                        {
                            if (!creatorAPI.launch) return;
                            creatorAPI.CreateBlock(point3, id,chunkData);
                            num++;
                        }
                        chunkData.Render();
                        player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                    });
                }
                DialogsManager.HideDialog(this);
            }

            if (HollowButton.IsClicked)
            {
                if (advanced)
                {
                    if (!DoublePosition)
                        Task.Run(() =>
                        {
                            int num = 0;
                            ChunkData chunkData = new ChunkData(this.creatorAPI);
                            creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                            foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(creatorAPI.Position[0]), (int)XRadius.Value, (int)YRadius.Value, (int)ZRadius.Value,true))
                            {
                                if (!creatorAPI.launch) return;
                                creatorAPI.CreateBlock(point3, id,chunkData);
                                num++;
                            }
                            chunkData.Render();
                            player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                        });
                    else
                    {
                        if (creatorAPI.Position[1].Y != -1)
                        {
                            Point3 Start = creatorAPI.Position[0];
                            Point3 End = creatorAPI.Position[1];
                            CreatorMain.Math.StartEnd(ref Start, ref End);
                            float x = Math.Abs((float)Start.X - End.X) / 2f;
                            float y = Math.Abs((float)Start.Y - End.Y) / 2f;
                            float z = Math.Abs((float)Start.Z - End.Z) / 2f;
                            Task.Run(() =>
                            {
                                ChunkData chunkData = new ChunkData(this.creatorAPI);
                                creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                                int num = 0;
                                foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(End.X + x, End.Y + y, End.Z + z), (int)x, (int)y, (int)z,true))
                                {
                                    if (!creatorAPI.launch) return;
                                    creatorAPI.CreateBlock(point3, id,chunkData);
                                    num++;
                                }
                                chunkData.Render();
                                player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                            });
                        }
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        ChunkData chunkData = new ChunkData(this.creatorAPI);
                        creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                        int num = 0;
                        foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Sphere(new Vector3(creatorAPI.Position[0]), (int)XRadius.Value, (int)XRadius.Value, (int)XRadius.Value, true))
                        {
                            if (!creatorAPI.launch) return;
                            creatorAPI.CreateBlock(point3, id,chunkData);
                            num++;
                        }
                        chunkData.Render();
                        player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                    });
                }
                DialogsManager.HideDialog(this);
            }
        }
    }
}