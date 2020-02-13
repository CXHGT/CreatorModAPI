using Engine;
using Game;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class MountainDialog : Dialog
    {
        private SliderWidget num1, num2, num3;
        private LabelWidget num_1, num_2, num_3;
        private ButtonWidget cancelButton;
        private ButtonWidget OK;
        private ButtonWidget restting;
        private TextBoxWidget TextBox;
        private ComponentPlayer player;
        private CreatorAPI creatorAPI;

        public MountainDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/山脉界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.OK = Children.Find<ButtonWidget>("确定", true);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            this.num_1 = Children.Find<LabelWidget>("滑条1数据", true);
            this.num_2 = Children.Find<LabelWidget>("滑条2数据", true);
            this.num_3 = Children.Find<LabelWidget>("滑条3数据", true);
            this.TextBox = Children.Find<TextBoxWidget>("方块ID", true);
            this.num1 = Children.Find<SliderWidget>("滑条1", true);
            this.num2 = Children.Find<SliderWidget>("滑条2", true);
            this.num3 = Children.Find<SliderWidget>("滑条3", true);
            num3.Value = 100f;
            this.restting = Children.Find<ButtonWidget>("重置", true);
        }
        public override void Update()
        {
            this.num_1.Text = $"参数1 :{(int)num1.Value}";
            this.num_2.Text = $"参数2 :{(int)num2.Value}";
            this.num_3.Text = $"参数3 :{(int)num3.Value}";
            if (this.restting.IsClicked)
            {
                this.num1.Value = 0f;
                this.num2.Value = 0f;
                this.num3.Value = 100f;
                this.TextBox.Text = "3:2:8";
            }
            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
            if (this.OK.IsClicked)
            {
                int BlockID_1;
                int? BlockID_2 = null, BlockID_3 = null;
                string[] texts = TextBox.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length == 2)
                {
                    if (!int.TryParse(texts[0], out BlockID_1)) BlockID_1 = 0;
                    int id;
                    if (int.TryParse(texts[1], out id)) BlockID_2 = id;
                }
                else if (texts.Length >= 3)
                {
                    if (!int.TryParse(texts[0], out BlockID_1)) BlockID_1 = 0;
                    int id;
                    if (int.TryParse(texts[1], out id)) BlockID_2 = id;
                    if (int.TryParse(texts[2], out id)) BlockID_3 = id;
                }
                else
                {
                    if (!int.TryParse(TextBox.Text, out BlockID_1)) BlockID_1 = 0;
                }
                Vector3 vector = new Vector3
                {
                    X = num1.Value,
                    Y = num2.Value,
                    Z = num3.Value
                };

                Point3 Start = creatorAPI.Position[0];
                Point3 End = creatorAPI.Position[1];
                CreatorMain.Math.StartEnd(ref Start, ref End);
                float X_Radius = (Start.X - End.X) / 2f;
                float Z_Radius = (Start.Z - End.Z) / 2f;
                float Radius = X_Radius > Z_Radius ? X_Radius : Z_Radius;
                float radius = X_Radius > Z_Radius ? Z_Radius : X_Radius;
                Radius = Math.Abs(Math.Abs(Radius) - 2f + vector.X);
                if (BlockID_2 == null) BlockID_2 = BlockID_1;
                if (BlockID_3 == null) BlockID_3 = BlockID_2;
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    Game.Random random = new Game.Random();
                    double pi = Math.PI * 0.5f;
                    int number = 0;
                    float num = 1.25f + (vector.Y / 99f);
                    float rand = random.UniformFloat(18f, 25f + (vector.Z / 10f));
                    for (int x = (int)-X_Radius; x <= (int)X_Radius; x++)
                    {
                        for (int z = (int)-Z_Radius; z <= Z_Radius; z++)
                        {
                            var a = (Math.Cos(pi * x / Radius) * Math.Cos(pi * z / radius)) * (Start.Y - End.Y);
                            var b = (Math.Sin(pi * x * num / radius + 2f) * Math.Cos(pi * z * num / Radius + 7f)) * (Start.Y - End.Y) * 0.35f;
                            var c = (Math.Sin(pi * x * num * 2f / Radius + 2f * rand) * Math.Sin(pi * z * num * 2f / radius + 8f * rand)) * (Start.Y - End.Y) * 0.2f;
                            var e = (Math.Sin(pi * x * num * 3.5f / radius + 2f * rand * 1.5f) * Math.Sin(pi * z * num * 3.5f / Radius + 12f * rand * 1.5f)) * (Start.Y - End.Y) * 0.15f;
                            var ls = a - b + c - e;
                            if (ls > 0f)
                            {
                                for (var i = 0; i <= ls; i++)
                                {
                                    Point3 point3 = new Point3((Start.X + End.X) / 2 + x, End.Y + (int)ls - i, (Start.Z + End.Z) / 2 + z);
                                    if (i > 5)
                                    {
                                        creatorAPI.CreateBlock(point3, BlockID_1,chunkData);
                                    }
                                    else if (i > 0)
                                    {
                                        creatorAPI.CreateBlock(point3, (int)BlockID_2,chunkData);
                                    }
                                    else if (i == 0)
                                    {
                                        creatorAPI.CreateBlock(point3, (int)BlockID_3,chunkData);
                                    }
                                    number++;
                                    if (!creatorAPI.launch) return;
                                }
                            }
                        }
                    }
                    chunkData.Render();
                    this.player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{number}个方块", true, true);
                });
                DialogsManager.HideDialog(this);
            }
        }
    }
}