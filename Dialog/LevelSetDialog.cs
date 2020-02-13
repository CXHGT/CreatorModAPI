using Engine;
using Game;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class LevelSetDialog : Dialog
    {
        private SliderWidget Radius;

        private LabelWidget delayLabel;

        private ButtonWidget plusButton;

        private ButtonWidget minusButton;

        private ButtonWidget OK;

        private ComponentPlayer player;

        private ButtonWidget Cancel;

        public LevelSetDialog(CreatorAPI creatorAPI)
        {
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/等级设置");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.Radius = Children.Find<SliderWidget>("等级", true);
            this.plusButton = Children.Find<ButtonWidget>("增加按钮", true);
            this.minusButton = Children.Find<ButtonWidget>("减少按钮", true);
            this.delayLabel = Children.Find<LabelWidget>("滑条", true);
            this.OK = Children.Find<ButtonWidget>("确定", true);
            this.Cancel = Children.Find<ButtonWidget>("取消", true);
            this.Radius.Value = player.PlayerData.Level;
            this.UpdateControls();
        }

        public override void Update()
        {
            if (this.Cancel.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
            if (this.OK.IsClicked)
            {
                player.PlayerData.Level = Radius.Value;
                this.player.ComponentGui.DisplaySmallMessage($"操作成功，等级设置成：{(int)Radius.Value}级", true, true);
                DialogsManager.HideDialog(this);
            }
            if (this.minusButton.IsClicked)
            {
                this.Radius.Value = MathUtils.Max(this.Radius.Value - 1f, (float)((int)this.Radius.MinValue));
            }
            if (this.plusButton.IsClicked)
            {
                this.Radius.Value = MathUtils.Min(this.Radius.Value + 1f, (float)((int)this.Radius.MaxValue));
            }
            this.UpdateControls();
        }

        private void UpdateControls()
        {
            this.minusButton.IsEnabled = (this.Radius.Value > this.Radius.MinValue);
            this.plusButton.IsEnabled = (this.Radius.Value < this.Radius.MaxValue);
            this.delayLabel.Text = $"设置等级：{(int)this.Radius.Value}";
        }
    }
}