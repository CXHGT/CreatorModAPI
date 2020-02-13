using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class PillarsDialog : InterfaceDialog
    {
        public SliderWidget Radius;
        public SliderWidget Height;
        public LabelWidget radiusDelayLabel;
        public LabelWidget heightDelayLabel;
        public ButtonWidget SoildButton;
        public ButtonWidget HollowButton;
        public bool typeBool = true;
        public PillarsDialog(CreatorAPI creatorAPI) : base(creatorAPI)
        {
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/柱子界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            base.GeneralSet();
            base.setShaftXYZ();
            Y_Shaft.Text = "正Y轴";
            this.Children.Find<LabelWidget>("名称", true).Text = "柱子";
            this.Radius = this.Children.Find<SliderWidget>("滑条1", true);
            this.radiusDelayLabel = this.Children.Find<LabelWidget>("滑条1数据", true);
            this.Height = this.Children.Find<SliderWidget>("滑条2", true);
            this.heightDelayLabel = this.Children.Find<LabelWidget>("滑条2数据", true);
            this.Children.Find<StackPanelWidget>("Data3", true).IsVisible = false;
            this.SoildButton = this.Children.Find<ButtonWidget>("实心", true);
            this.HollowButton = this.Children.Find<ButtonWidget>("空心", true);
        }
        public override void Update()
        {
            base.Update();
            this.radiusDelayLabel.Text = $"半径{(int)Radius.Value}格";
            this.heightDelayLabel.Text = $"长度{(int)Height.Value}格";
            upDataButton();
            upClickButton(this.blockIconWidget.Value);
        }
        public virtual void upClickButton(int id)
        {
            if (this.SoildButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Pillars(creatorAPI.Position[0], (int)Radius.Value,(int)Height.Value ,this.creatorType,typeBool))
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
            if (this.HollowButton.IsClicked)
            {
                Task.Run(() =>
                {
                    ChunkData chunkData = new ChunkData(this.creatorAPI);
                    creatorAPI.revokeData = new ChunkData(this.creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.Pillars(creatorAPI.Position[0], (int)Radius.Value,(int)Height.Value, this.creatorType,typeBool,true))
                    {
                        creatorAPI.CreateBlock(point3, id,chunkData);
                        num++;
                        if (!creatorAPI.launch) return;
                    }
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                });
                DialogsManager.HideDialog(this);
            }
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