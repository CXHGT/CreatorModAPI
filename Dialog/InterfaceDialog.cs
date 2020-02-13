using Game;
using System.Xml.Linq;
using Engine;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CreatorModAPI
{
    public class InterfaceDialog : Dialog
    {
        public CreatorAPI creatorAPI;
        public SubsystemTerrain subsystemTerrain;
        public ComponentPlayer player;
        public ButtonWidget cancelButton;
        public TextBoxWidget blockID;
        public BlockIconWidget blockIconWidget;
        public ButtonWidget SelectBlockButton;
        public CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y;
        public ButtonWidget X_Shaft, Y_Shaft, Z_Shaft;
        public InterfaceDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
        }
        public override void Update()
        {
            try
            {
                this.blockIconWidget.Value = int.Parse(blockID.Text);
            }
            catch
            {
                this.blockIconWidget.Value = 0;
            }



            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
            if (this.SelectBlockButton.IsClicked)
            {
                int[] items = new int[] {0, 2, 8, 7, 3, 67, 66, 4, 5, 26, 73, 21, 46, 47, 15, 62, 68, 126, 71, 1, 92, 18 };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", items, 72f, delegate (object index)
                {
                    XElement node = ContentManager.Get<XElement>("Widgets/SelectBlockItem");
                    ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget(null, node, null);
                    containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
                    containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));
                    return containerWidget;
                }, delegate (object index)
                {
                    this.blockID.Text = ((int)index).ToString();
                }));
            }
        }
        public void GeneralSet()
        {
            this.cancelButton = this.Children.Find<ButtonWidget>("取消");
            this.blockID = this.Children.Find<TextBoxWidget>("方块ID", true);
            this.blockIconWidget = this.Children.Find<BlockIconWidget>("方块");
            this.SelectBlockButton = this.Children.Find<ButtonWidget>("选择方块");
        }
        public void setShaftXYZ()
        {
            this.X_Shaft = Children.Find<ButtonWidget>("X轴", true);
            this.Y_Shaft = Children.Find<ButtonWidget>("Y轴", true);
            this.Z_Shaft = Children.Find<ButtonWidget>("Z轴", true);
            this.Y_Shaft.Color = Color.Green;
        }
        public virtual void upDataButton()
        {
            if (this.X_Shaft.IsClicked) upDataButton(CreatorMain.CreatorType.X, X_Shaft);
            if (this.Y_Shaft.IsClicked) upDataButton(CreatorMain.CreatorType.Y, Y_Shaft);
            if (this.Z_Shaft.IsClicked) upDataButton(CreatorMain.CreatorType.Z, Z_Shaft);
        }
        public virtual void upDataButton(CreatorMain.CreatorType creatorType, ButtonWidget button)
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
        public string getTypeName(CreatorMain.CreatorType typeName)
        {
            string name = "Z";
            if (typeName == CreatorMain.CreatorType.X) name = "X";
            if (typeName == CreatorMain.CreatorType.Y) name = "Y";
            return name;
        }
    }
}