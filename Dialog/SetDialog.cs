using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace CreatorModAPI
{
    public class SetDialog : Dialog
    {
        private ButtonWidget OK;

        private SubsystemTerrain subsystemTerrain;

        private ComponentPlayer player;

        private ButtonWidget resettingButton;

        private ButtonWidget generatingMod;

        private ButtonWidget generatingSet;

        private ButtonWidget SetPositionMode;

        private ButtonWidget unLimited;

        private ButtonWidget RevokeButton;

        private ButtonWidget AirIdentifyButton;

        private ButtonWidget professionButton;

        private CreatorAPI creatorAPI;

        private ButtonWidget setMainWidgetButton;

        public SetDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/设置界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.OK = Children.Find<ButtonWidget>("确定", true);
            this.generatingMod = Children.Find<ButtonWidget>("生成模式", true);
            this.resettingButton = Children.Find<ButtonWidget>("重置", true);
            this.generatingSet = Children.Find<ButtonWidget>("生成开关", true);
            this.SetPositionMode = Children.Find<ButtonWidget>("设点模式");
            this.unLimited = Children.Find<ButtonWidget>("超距生成");
            this.AirIdentifyButton = Children.Find<ButtonWidget>("空气识别");
            this.RevokeButton = Children.Find<ButtonWidget>("撤回开关");
            this.professionButton = Children.Find<ButtonWidget>("专业模式");
            this.setMainWidgetButton = this.Children.Find<ButtonWidget>("旧主界面");
        }

        public override void Update()
        {
            if (creatorAPI.AirIdentify) AirIdentifyButton.Color = Color.Yellow; else AirIdentifyButton.Color = Color.Red;
            if (creatorAPI.launch) generatingSet.Color = Color.Yellow; else generatingSet.Color = Color.Red;
            if (creatorAPI.UnLimitedOfCreate) this.unLimited.Color = Color.Yellow; else this.unLimited.Color = Color.Red;
            if (this.creatorAPI.RevokeSwitch) this.RevokeButton.Color = Color.Yellow; else this.RevokeButton.Color = Color.Red;
            if (this.creatorAPI.oldMainWidget) this.setMainWidgetButton.Color = Color.Yellow; else this.setMainWidgetButton.Color = Color.Red;
            if(this.professionButton.IsClicked) DialogsManager.ShowDialog(this.creatorAPI.componentMiner.ComponentPlayer.View.GameWidget, new PasswordDialog(this.creatorAPI.componentMiner.ComponentPlayer));
            this.professionButton.IsEnabled = !CreatorMain.professional;
            this.unLimited.IsEnabled = CreatorMain.professional;
            //this.RevokeButton.IsEnabled = CreatorMain.professional;
            switch (creatorAPI.amountPoint)
            {
                case CreatorAPI.NumberPoint.One:
                    SetPositionMode.Text = "1点模式";
                    break;
                case CreatorAPI.NumberPoint.Two:
                    SetPositionMode.Text = "2点模式";
                    break;
                case CreatorAPI.NumberPoint.Three:
                    SetPositionMode.Text = "3点模式";
                    break;
                case CreatorAPI.NumberPoint.Four:
                    SetPositionMode.Text = "4点模式";
                    break;
            }
            if (SetPositionMode.IsClicked)
            {
                int[] p = new int[4] { 1, 2, 3, 4 };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("选择设置点模式", p, 56f, (object e) => $"{(int)e}点模式", delegate (object e)
                {
                    creatorAPI.amountPoint = (CreatorAPI.NumberPoint)((int)e - 1);
                }));
            }
            if (this.unLimited.IsClicked)
            {
                this.creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage("超距模式建议在性能强劲的设备上使用，而且请不要空间过大，不然可能造成内存溢出而导致毁图", true, false);
                this.creatorAPI.UnLimitedOfCreate = !this.creatorAPI.UnLimitedOfCreate;
            }
            if (this.AirIdentifyButton.IsClicked)
            {
                this.creatorAPI.AirIdentify = !this.creatorAPI.AirIdentify;
            }
            if (this.RevokeButton.IsClicked)
            {
                this.creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage("超距模式下尽量不要使用撤回功能", true, false);
                this.creatorAPI.RevokeSwitch = !this.creatorAPI.RevokeSwitch;
            }
            switch (creatorAPI.CreateBlockType)
            {
                case CreateBlockType.Normal:
                    this.generatingMod.Text = "正常生成";
                    break;
                case CreateBlockType.Fast:
                    this.generatingMod.Text = "快速生成";
                    break;
                case CreateBlockType.Catch:
                    this.generatingMod.Text = "缓存生成";
                    break;
            }
            if (this.resettingButton.IsClicked)
            {
                creatorAPI.launch = true;
                creatorAPI.CreateBlockType = CreateBlockType.Fast;
                generatingSet.Color = Color.Yellow;
                creatorAPI.amountPoint = CreatorAPI.NumberPoint.Two;
            }
            if (generatingMod.IsClicked)
            {
                IList<int> enumValues = EnumUtils.GetEnumValues(typeof(CreateBlockType));
                string[] createZhString = new string[3]{"正常生成","快速生成","缓存生成" };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("选择生成类型", enumValues, 56f, (object e) => createZhString[(int)e], delegate (object e)
                {
                    this.creatorAPI.CreateBlockType = (CreateBlockType)e;
                }));
            }
            if (generatingSet.IsClicked) creatorAPI.launch = !creatorAPI.launch;
            if (this.OK.IsClicked) DialogsManager.HideDialog(this);
            if (this.setMainWidgetButton.IsClicked) this.creatorAPI.oldMainWidget = !this.creatorAPI.oldMainWidget;




        }

        private class PasswordDialog : Dialog
        {
            private ButtonWidget OK;

            private ButtonWidget cancelButton;

            private TextBoxWidget TextBox;

            private ComponentPlayer player;

            public PasswordDialog(ComponentPlayer player)
            {
                this.player = player;
                XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/通用界面3");
                WidgetsManager.LoadWidgetContents(this, this, node);
                Children.Find<LabelWidget>("名称", true).Text = "请输入密匙";
                this.cancelButton = Children.Find<ButtonWidget>("取消", true);
                this.OK = Children.Find<ButtonWidget>("确定", true);
                this.TextBox = Children.Find<TextBoxWidget>("方块ID", true);
                this.TextBox.Title = "请输入密匙";
                this.TextBox.Text = "";
                this.Children.Find<BlockIconWidget>("方块").IsVisible = false;
                this.Children.Find<ButtonWidget>("选择方块").IsVisible = false;
            }
            public override void Update()
            {
                if (this.cancelButton.IsClicked)
                {
                    DialogsManager.HideDialog(this);
                }
                if (this.OK.IsClicked)
                {
                    if (this.TextBox.Text == "HelloWorld")
                    {
                        CreatorMain.professional = true;
                        this.player.ComponentGui.DisplaySmallMessage($"创世神{CreatorMain.version}专业模式开启", true, false);
                    }
                    else
                    {
                        this.player.ComponentGui.DisplaySmallMessage($"创世神{CreatorMain.version}专业模式开启失败", true, false);
                    }
                    DialogsManager.HideDialog(this);
                }
            }

        }
    }
}