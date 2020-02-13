using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class OnekeyGenerationDialog : Dialog
    {
        public static readonly string SdcardOnekey = CreatorMain.Sdcard;
        private class DerivedDialog : Dialog
        {
            private ButtonWidget OK;

            private ButtonWidget cancelButton;

            private TextBoxWidget TextBox;

            private ComponentPlayer player;

            private Dialog dialog;

            private ListPanelWidget listView;

            public DerivedDialog(ComponentPlayer player, Dialog dialog, ListPanelWidget listView)
            {
                this.player = player;
                this.dialog = dialog;
                this.listView = listView;
                XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/通用界面3");
                WidgetsManager.LoadWidgetContents(this, this, node);
                Children.Find<LabelWidget>("名称", true).Text = "请输入名称";
                this.cancelButton = Children.Find<ButtonWidget>("取消", true);
                this.OK = Children.Find<ButtonWidget>("确定", true);
                this.TextBox = Children.Find<TextBoxWidget>("方块ID", true);
                this.TextBox.Text = "";
                this.Children.Find<BlockIconWidget>("方块").IsVisible = false;
            }
            public override void Update()
            {
                if (this.cancelButton.IsClicked)
                {
                    DialogsManager.HideDialog(this);
                }
                if (this.OK.IsClicked)
                {
                    string dir = CreatorMain.Export_OnekeyFile_Directory;
                    string name;
                    if (TextBox.Text.Length <= 0)
                    {
                        name = $"{DateTime.Now.ToString("yyyy-MM-dd")}_{DateTime.Now.ToLongTimeString().ToString()}";
                    }
                    else
                    {
                        name = TextBox.Text;
                    }
                    if (!FileOperation.IsFileInUse(CreatorMain.OneKeyFile))
                    {
                        try
                        {
                            FileStream fileStream = new FileStream(CreatorMain.OneKeyFile, FileMode.Open);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            FileStream files = new FileStream($"{dir}/{name}.o", FileMode.OpenOrCreate);
                            fileStream.CopyTo(files);
                            files.Dispose();
                            fileStream.Dispose();
                            this.player.ComponentGui.DisplaySmallMessage($"导出成功！文件所在位置：\n{dir}/{name}.o", true, false);
                            DialogsManager.HideDialog(this);
                            this.listView.ClearItems();
                            if (!Directory.Exists(CreatorMain.Export_OnekeyFile_Directory))
                                Directory.CreateDirectory(CreatorMain.Export_OnekeyFile_Directory);
                            foreach (string file in Directory.GetFiles(CreatorMain.Export_OnekeyFile_Directory))
                            {
                                if (Path.GetExtension(file) == ".o")
                                    this.listView.AddItem(Path.GetFileName(file));
                            }
                        }
                        catch (Exception e)
                        {
                            this.player.ComponentGui.DisplaySmallMessage($"发生了一个很严重的错误，\n 错误提示 :{e.Message}\n{dir}", true, false);
                            DialogsManager.HideDialog(this);
                            DialogsManager.HideDialog(this.dialog);
                        }
                    }
                    else
                        this.player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                    DialogsManager.HideDialog(this);
                }
            }
        }

        private ButtonWidget OKButton;

        private ButtonWidget TypeButton;

        private ButtonWidget CreateButton, DerivedButton, ImportButton, OnAndOffButton;

        private SubsystemTerrain subsystemTerrain;

        private ComponentPlayer player;

        private ListPanelWidget ListView;

        private CreatorAPI creatorAPI;

        private ButtonWidget DeleteButton;

        public OnekeyGenerationDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/一键界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.CreateButton = Children.Find<ButtonWidget>("创建", true);
            this.OnAndOffButton = Children.Find<ButtonWidget>("开启",true);
            this.DerivedButton = Children.Find<ButtonWidget>("导出", true);
            this.ImportButton = Children.Find<ButtonWidget>("导入", true);
            this.DeleteButton = Children.Find<ButtonWidget>("删除", true);
            this.OKButton = Children.Find<ButtonWidget>("确定", true);
            this.ListView = Children.Find<ListPanelWidget>("ListView", true);
            this.TypeButton = Children.Find<ButtonWidget>("类型",true);
            UpList();
        }

        private void UpList()
        {
            this.ListView.ClearItems();
            if (!Directory.Exists(CreatorMain.Export_OnekeyFile_Directory))
                Directory.CreateDirectory(CreatorMain.Export_OnekeyFile_Directory);
            foreach (string file in Directory.GetFiles(CreatorMain.Export_OnekeyFile_Directory))
            {
                if (Path.GetExtension(file) == ".o")
                    this.ListView.AddItem(Path.GetFileName(file));
            }
        }

        public override void Update()
        {
            this.TypeButton.IsEnabled = false;
            if (this.OKButton.IsClicked) DialogsManager.HideDialog(this);
            if (this.ImportButton.IsClicked)
            {
                if (!Directory.Exists(CreatorMain.CacheDirectory))
                    Directory.CreateDirectory(CreatorMain.CacheDirectory);
                string sdFile = CreatorMain.OneKeyFile;
                string _sdFile = CreatorMain.Export_OnekeyFile_Directory + "/" + (string)this.ListView.SelectedItem;
                if (!FileOperation.IsFileInUse(_sdFile) && (!File.Exists(sdFile) || !FileOperation.IsFileInUse(sdFile)))
                {
                    FileStream fileStream = new FileStream(_sdFile, FileMode.Open);
                    FileStream file = new FileStream(sdFile, FileMode.Create);
                    fileStream.CopyTo(file);
                    file.Dispose();
                    fileStream.Dispose();
                    player.ComponentGui.DisplaySmallMessage("导入成功！", true, false);
                }
                else
                    player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                DialogsManager.HideDialog(this);
            }
            if (DeleteButton.IsClicked)
            {
                string sdFile = CreatorMain.Export_OnekeyFile_Directory + "/" + (string)this.ListView.SelectedItem;
                if (FileOperation.Delete(sdFile))
                {
                    player.ComponentGui.DisplaySmallMessage("删除成功！", true, false);
                    UpList();
                }
                else
                {
                    player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                }
            }
            this.DerivedButton.IsEnabled = File.Exists(CreatorMain.OneKeyFile);
            if (this.DerivedButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView));
            if (this.CreateButton.IsClicked)
            {
                if(creatorAPI.Position[2] == new Point3(0, -1, 0))
                {
                    player.ComponentGui.DisplaySmallMessage("请设置点3！", true, false);
                }else
                Task.Run(() =>
                {
                    try
                    {
                        if (!Directory.Exists(CreatorMain.CacheDirectory))
                            Directory.CreateDirectory(CreatorMain.CacheDirectory);
                        OnekeyGeneration.CreateOnekey(creatorAPI, CreatorMain.CacheDirectory + "/", "CacheFile.od", creatorAPI.Position[0], creatorAPI.Position[1],creatorAPI.Position[2]);
                    }
                    catch (Exception e)
                    {
                        player.ComponentGui.DisplaySmallMessage(e.Message, true, false);
                    }
                });
                DialogsManager.HideDialog(this);
            }
            if (ListView.SelectedIndex == null)
            {
                this.ImportButton.IsEnabled = false;
                this.DeleteButton.IsEnabled = false;
            }
            else
            {
                this.ImportButton.IsEnabled = true;
                this.DeleteButton.IsEnabled = true;
            }
            if (creatorAPI.oneKeyGeneration)
            {
                OnAndOffButton.Color = Color.Yellow;
            }
            else
            {
                OnAndOffButton.Color = Color.Red;
            }
            if (OnAndOffButton.IsClicked)
            {
                if (creatorAPI.oneKeyGeneration)
                {
                    creatorAPI.oneKeyGeneration = false;
                }
                else
                {
                    creatorAPI.oneKeyGeneration = true;
                }
            }



        }














    }
}
