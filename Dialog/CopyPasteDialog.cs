using Engine;
using Game;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class CopyPasteDialog : Dialog
    {
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
                this.Children.Find<BevelledButtonWidget>("选择方块").IsVisible = false;
            }
            public override void Update()
            {
                if (this.cancelButton.IsClicked)
                {
                    DialogsManager.HideDialog(this);
                }
                if (this.OK.IsClicked)
                {
                    string dir = CreatorMain.Export_CopyFile_Directory;
                    string name;
                    if (TextBox.Text.Length <= 0)
                    {
                        name = $"{DateTime.Now.ToString("yyyy-MM-dd")}_{DateTime.Now.ToLongTimeString().ToString()}";
                    }
                    else
                    {
                        name = TextBox.Text;
                    }
                    if (!FileOperation.IsFileInUse(CreatorMain.CopyFile))
                    {
                        try
                        {
                            FileStream fileStream = new FileStream(CreatorMain.CopyFile, FileMode.Open);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            FileStream files = new FileStream($"{dir}/{name}.w", FileMode.OpenOrCreate);
                            fileStream.CopyTo(files);
                            files.Dispose();
                            fileStream.Dispose();
                            this.player.ComponentGui.DisplaySmallMessage($"导出成功！文件所在位置：\n{dir}/{name}.w", true, false);
                            DialogsManager.HideDialog(this);
                            this.listView.ClearItems();
                            if (!Directory.Exists(CreatorMain.Export_CopyFile_Directory))
                                Directory.CreateDirectory(CreatorMain.Export_CopyFile_Directory);
                            foreach (string file in Directory.GetFiles(CreatorMain.Export_CopyFile_Directory))
                            {
                                if (Path.GetExtension(file) == ".w")
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

        private ButtonWidget cancelButton;

        private ButtonWidget PasteButton;

        private ButtonWidget DirectionSetButton;
        private ButtonWidget specialCopyButton;
        private ButtonWidget specialPasteButton;
        private ButtonWidget LimitButton;

        private ButtonWidget CopyButton, DerivedButton, ImportButton;

        private ComponentPlayer player;

        private ListPanelWidget ListView;

        private CreatorAPI creatorAPI;

        private ButtonWidget DeleteButton;

        private ButtonWidget MirrorButton;

        public CopyPasteDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/复制粘贴");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.CopyButton = Children.Find<ButtonWidget>("复制", true);
            this.DerivedButton = Children.Find<ButtonWidget>("导出", true);
            this.ImportButton = Children.Find<ButtonWidget>("导入", true);
            this.DeleteButton = Children.Find<ButtonWidget>("删除", true);
            this.cancelButton = Children.Find<ButtonWidget>("取消", true);
            this.PasteButton = Children.Find<ButtonWidget>("粘贴", true);
            this.MirrorButton = Children.Find<ButtonWidget>("镜像", true);
            this.LimitButton = Children.Find<ButtonWidget>("限制", true);
            this.DirectionSetButton = Children.Find<ButtonWidget>("旋转", true);
            this.specialCopyButton = Children.Find<ButtonWidget>("特殊复制", true);
            this.specialPasteButton = Children.Find<ButtonWidget>("特殊粘贴", true);
            if (creatorAPI.pasteLimit) LimitButton.Color = Color.Yellow; else LimitButton.Color = Color.Red;
            if (creatorAPI.pasteRotate) DirectionSetButton.Color = Color.Yellow; else DirectionSetButton.Color = Color.Red;
            this.ListView = Children.Find<ListPanelWidget>("ListView", true);
            UpList();
        }

        private void UpList()
        {
            this.ListView.ClearItems();
            if (!Directory.Exists(CreatorMain.Export_CopyFile_Directory))
                Directory.CreateDirectory(CreatorMain.Export_CopyFile_Directory);
            foreach (string file in Directory.GetFiles(CreatorMain.Export_CopyFile_Directory))
            {
                if (Path.GetExtension(file) == ".w")
                    this.ListView.AddItem(Path.GetFileName(file));
            }
        }

        public override void Update()
        {
            if (this.DirectionSetButton.IsClicked)
            {
                if (creatorAPI.pasteRotate)
                {
                    creatorAPI.pasteRotate = false;
                    this.DirectionSetButton.Color = Color.Red;
                }
                else
                {
                    creatorAPI.pasteRotate = true;
                    this.DirectionSetButton.Color = Color.Yellow;
                }
            }
            if (this.LimitButton.IsClicked)
            {
                if (creatorAPI.pasteLimit)
                {
                    creatorAPI.pasteLimit = false;
                    this.LimitButton.Color = Color.Red;
                }
                else
                {
                    creatorAPI.pasteLimit = true;
                    this.LimitButton.Color = Color.Yellow;
                }
            }
            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
            if (this.PasteButton.IsClicked)
            {
                if (!FileOperation.IsFileInUse(CreatorMain.CopyFile))
                    Task.Run(() =>
                    {
                        try
                        {
                            CopyAndPaste.PasetData(creatorAPI, CreatorMain.CopyFile, creatorAPI.Position[0], creatorAPI.Position[1]);
                        }catch(Exception e)
                        {
                            player.ComponentGui.DisplaySmallMessage(e.Message, true, false);
                        }
                    });
                else
                    player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                DialogsManager.HideDialog(this);
            }
            if (this.MirrorButton.IsClicked)
            {
                if (!FileOperation.IsFileInUse(CreatorMain.CopyFile))
                    Task.Run(() =>
                    {
                        try
                        {
                            CopyAndPaste.MirrorData(creatorAPI, CreatorMain.CopyFile, creatorAPI.Position[0], creatorAPI.Position[1]);
                        }
                        catch (Exception e)
                        {
                            player.ComponentGui.DisplaySmallMessage(e.Message, true, false);
                        }
                    });
                else
                    player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                DialogsManager.HideDialog(this);
            }
            if (this.ImportButton.IsClicked)
            {
                if (!Directory.Exists(CreatorMain.CacheDirectory))
                    Directory.CreateDirectory(CreatorMain.CacheDirectory);
                string sdFile = CreatorMain.CopyFile;
                string _sdFile = CreatorMain.Export_CopyFile_Directory+ "/" + (string)this.ListView.SelectedItem;
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
                string sdFile = CreatorMain.Export_CopyFile_Directory+"/" + (string)this.ListView.SelectedItem;
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
            this.DerivedButton.IsEnabled = File.Exists(CreatorMain.CopyFile);
            if (this.DerivedButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView));
            if (this.CopyButton.IsClicked)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!Directory.Exists(CreatorMain.CacheDirectory))
                            Directory.CreateDirectory(CreatorMain.CacheDirectory);
                        CopyAndPaste.CreateCopy(creatorAPI, CreatorMain.CacheDirectory, "CacheFile.cd", creatorAPI.Position[0], creatorAPI.Position[1]);
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
            this.specialCopyButton.IsEnabled = CreatorMain.professional;
            this.specialPasteButton.IsEnabled = CreatorMain.professional;
            if (this.specialCopyButton.IsClicked)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!Directory.Exists(CreatorMain.CacheDirectory))
                            Directory.CreateDirectory(CreatorMain.CacheDirectory);
                        CopyAndPaste.CreateSpecialCopy(creatorAPI, CreatorMain.SpecialCopyFile, creatorAPI.Position[0], creatorAPI.Position[1]);
                    }
                    catch (Exception e)
                    {
                        player.ComponentGui.DisplaySmallMessage(e.Message, true, false);
                    }
                });
                DialogsManager.HideDialog(this);
            }
            this.specialPasteButton.IsEnabled = File.Exists(CreatorMain.SpecialCopyFile);
            if (this.specialPasteButton.IsClicked)
            {
                Task.Run(() =>
                {
                    try
                    {
                        CopyAndPaste.SpecialPasetData(creatorAPI, CreatorMain.SpecialCopyFile, creatorAPI.Position[0], creatorAPI.Position[1]);
                    }
                    catch (Exception e)
                    {
                        player.ComponentGui.DisplaySmallMessage(e.Message, true, false);
                    }
                });
                DialogsManager.HideDialog(this);
            }




        }
    }
}