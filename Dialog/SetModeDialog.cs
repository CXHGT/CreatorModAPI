using Engine;
using Game;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class SetModeDialog : Dialog
    {
        private ComponentPlayer player;

        private ButtonWidget CreativeButton;

        private ButtonWidget ChallengingButton;

        private ButtonWidget cancelButton;

        public ButtonWidget HarmlessButton;

        public ButtonWidget AdventureButton;

        public ButtonWidget CruelButton;

        private SubsystemGameInfo gameInfo;

        private CreatorAPI creatorAPI;

        public SetModeDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/模式设置");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.CreativeButton = Children.Find<ButtonWidget>("创造模式", true);
            this.ChallengingButton = Children.Find<ButtonWidget>("挑战模式", true);
            this.HarmlessButton = Children.Find<ButtonWidget>("无害模式", true);
            this.AdventureButton = Children.Find<ButtonWidget>("闯关模式", true);
            this.CruelButton = Children.Find<ButtonWidget>("残酷模式", true);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            gameInfo = player.Project.FindSubsystem<SubsystemGameInfo>(true);
            if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
            {
                CreativeButton.IsEnabled = false;
            }
            else if (gameInfo.WorldSettings.GameMode == GameMode.Challenging)
            {
                ChallengingButton.IsEnabled = false;
            }
            else if (gameInfo.WorldSettings.GameMode == GameMode.Harmless)
            {
                HarmlessButton.IsEnabled = false;
            }
            else if (gameInfo.WorldSettings.GameMode == GameMode.Adventure)
            {
                AdventureButton.IsEnabled = false;
            }
            else if (gameInfo.WorldSettings.GameMode == GameMode.Cruel)
            {
                CruelButton.IsEnabled = false;
            }
        }
        public override void Update()
        {
            this.CruelButton.IsEnabled = CreatorMain.professional;
            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
            if (CreativeButton.IsClicked)
            {
                gameInfo.WorldSettings.GameMode = GameMode.Creative;
                upDataWorld();
            }
            if (ChallengingButton.IsClicked)
            {
                if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Challenging;
                    upDataWorld();
                }
                else
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Challenging;
                    player.ComponentGui.DisplaySmallMessage("模式改变为 ： 挑战模式", true, true);
                    DialogsManager.HideDialog(this);
                }
            }
            if (HarmlessButton.IsClicked)
            {
                if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Harmless;
                    upDataWorld();
                }
                else
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Harmless;
                    player.ComponentGui.DisplaySmallMessage("模式改变为 ： 无害模式", true, true);
                    DialogsManager.HideDialog(this);
                }
            }
            if (AdventureButton.IsClicked)
            {
                if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Adventure;
                    upDataWorld();
                }
                else
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Adventure;
                    player.ComponentGui.DisplaySmallMessage("模式改变为 ： 闯关模式", true, true);
                    DialogsManager.HideDialog(this);
                }
            }
            if (CruelButton.IsClicked)
            {
                //player.ComponentGui.DisplaySmallMessage("由于该功能被很多生存dalao反对，所以该功能只在专业版开启", true, true);
                //DialogsManager.HideDialog(this);
                if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Cruel;
                    upDataWorld();
                }
                else
                {
                    gameInfo.WorldSettings.GameMode = GameMode.Cruel;
                    player.ComponentGui.DisplaySmallMessage("模式改变为 ： 残酷模式，虽然你这样做不太好", true, true);
                    DialogsManager.HideDialog(this);
                }
            }
        }
        public void upDataWorld()
        {
            WorldInfo world = GameManager.WorldInfo;
            GameManager.SaveProject(true, true);
            GameManager.DisposeProject();
            object[] expr_E9 = new object[2];
            expr_E9[0] = world;
            ScreensManager.SwitchScreen("GameLoading", expr_E9);
        }
    }
}