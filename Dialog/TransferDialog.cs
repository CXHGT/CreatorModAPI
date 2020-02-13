using Engine;
using Game;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class TransferDialog : Dialog
    {
        private ButtonWidget cancelButton;

        private ButtonWidget Transfer;

        private TextBoxWidget X;

        private TextBoxWidget Y;

        private TextBoxWidget Z;

        private SubsystemTerrain subsystemTerrain;

        private ComponentPlayer player;

        public TransferDialog(CreatorAPI creatorAPI)
        {
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/传送设置");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            this.Transfer = Children.Find<ButtonWidget>("传送", true);
            this.X = Children.Find<TextBoxWidget>("X", true);
            this.Y = Children.Find<TextBoxWidget>("Y", true);
            this.Z = Children.Find<TextBoxWidget>("Z", true);
        }

        public override void Update()
        {
            if (this.cancelButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
            if (this.Transfer.IsClicked)
            {
                Point3 p;
                if (!int.TryParse(this.X.Text, out p.X) || !int.TryParse(this.Y.Text, out p.Y) || !int.TryParse(this.Z.Text, out p.Z))
                {
                    this.player.ComponentGui.DisplaySmallMessage("请输入正当的做标", true, true);
                }
                else
                {
                    Vector3 v = new Vector3(p.X, p.Y, p.Z);
                    this.player.ComponentBody.Position = v;
                    this.player.ComponentGui.DisplaySmallMessage($"成功传送到：\nX:{(int)v.X} , Y:{(int)v.Y} , Z:{(int)v.Z}", true, true);
                }
                DialogsManager.HideDialog(this);
            }
        }
    }
}