using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class ModDialog : Dialog
    {
        private enum DataType
        {
            OneKey,
            Copy,
            OldOneKey,
            OldCopy,
            SpecialCopy
        }

        private class DerivedDialog : Dialog
        {
            private ButtonWidget OK;

            private ButtonWidget cancelButton;

            private TextBoxWidget TextBox;

            private ComponentPlayer player;

            private Dialog dialog;

            private ListPanelWidget listView;

            private DataType dataType;

            public DerivedDialog(ComponentPlayer player, Dialog dialog, ListPanelWidget listView,DataType dataType = DataType.Copy)
            {
                this.dataType = dataType;
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
                    string dir = CreatorMain.Export_ModFile_Directory;
                    string name;
                    if (TextBox.Text.Length <= 0)
                    {
                        name = $"{DateTime.Now.ToString("yyyy-MM-dd")}_{DateTime.Now.ToLongTimeString().ToString()}";
                    }
                    else
                    {
                        name = TextBox.Text;
                    }
                    try
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        string fileName = dir + "/" + name + ".wMod2";;
                        if (this.dataType == DataType.Copy)
                        {
                            CopyAndPaste.ExportCopywMod2(CreatorMain.CopyFile,fileName);
                        }
                        else if (this.dataType == DataType.OneKey)
                        {
                            fileName = dir + "/" + name + ".oMod2";
                            OnekeyGeneration.ExportOnekeyoMod2(CreatorMain.OneKeyFile, fileName);
                        }
                        else if (this.dataType == DataType.OldCopy)
                        {
                            fileName = dir + "/" + name + ".wMod";
                            CopyAndPaste.ExportCopywMod(CreatorMain.CopyFile, fileName);
                        }
                        else if (this.dataType == DataType.OldOneKey)
                        {
                            fileName = dir + "/" + name + ".oMod";
                            OnekeyGeneration.ExportOnekeyoMod(CreatorMain.OneKeyFile, fileName);
                        }
                        else if (this.dataType == DataType.SpecialCopy)
                        {
                            fileName = dir + "/" + name + ".sMod";
                            FileStream fileStream = new FileStream(CreatorMain.SpecialCopyFile, FileMode.Open);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            FileStream files = new FileStream(fileName, FileMode.OpenOrCreate);
                            fileStream.CopyTo(files);
                            files.Dispose();
                            fileStream.Dispose();
                        }
                        this.player.ComponentGui.DisplaySmallMessage($"导出成功！文件所在位置：\n{fileName}", true, false);
                        DialogsManager.HideDialog(this);
                        this.listView.ClearItems();
                        if (!Directory.Exists(CreatorMain.Export_ModFile_Directory))
                            Directory.CreateDirectory(CreatorMain.Export_ModFile_Directory);
                        foreach (string file in Directory.GetFiles(CreatorMain.Export_ModFile_Directory))
                        {
                            if (Path.GetExtension(file) == ".oMod" || Path.GetExtension(file) == ".wMod" || Path.GetExtension(file) == ".oMod2" || Path.GetExtension(file) == ".wMod2" || Path.GetExtension(file) == ".sMod")
                                this.listView.AddItem(Path.GetFileName(file));
                        }
                    }
                    catch (Exception e)
                    {
                        this.player.ComponentGui.DisplaySmallMessage($"发生了一个很严重的错误，\n 错误提示 :{e.Message}\n{dir}", true, false);
                        DialogsManager.HideDialog(this);
                        DialogsManager.HideDialog(this.dialog);
                    }
                    DialogsManager.HideDialog(this);
                }
            }
        }


        private ButtonWidget ImportButton;

        private ButtonWidget ExportOnekeyButton;

        private ButtonWidget ExportCopyButton;

        private ButtonWidget DeleteButton;

        private ButtonWidget OK;

        private CreatorAPI creatorAPI;

        private ComponentPlayer player;

        private ListPanelWidget ListView;

        private ButtonWidget ExportOldCopyButton;

        public ButtonWidget ExportOldOnekeyButton;
        private ButtonWidget DerivedSpecialButton;

        public ModDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Widget/模组界面");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.OK = Children.Find<ButtonWidget>("确定", true);
            this.ExportOnekeyButton = Children.Find<ButtonWidget>("导出一键", true);
            this.ImportButton = Children.Find<ButtonWidget>("导入配置", true);
            this.ExportCopyButton = Children.Find<ButtonWidget>("导出复制", true);
            this.ExportOldCopyButton = Children.Find<ButtonWidget>("导出旧复制", true);
            this.ExportOldOnekeyButton = Children.Find<ButtonWidget>("导出旧一键", true);
            this.DerivedSpecialButton = Children.Find<ButtonWidget>("导出特殊", true);
            this.DeleteButton = Children.Find<ButtonWidget>("删除", true);
            this.ListView = Children.Find<ListPanelWidget>("ListView", true);
            UpList();
        }

        private void UpList()
        {
            this.ListView.ClearItems();
            if (!Directory.Exists(CreatorMain.Export_ModFile_Directory))
                Directory.CreateDirectory(CreatorMain.Export_ModFile_Directory);
            foreach (string file in Directory.GetFiles(CreatorMain.Export_ModFile_Directory))
            {
                if (Path.GetExtension(file) == ".oMod" || Path.GetExtension(file) == ".wMod" || Path.GetExtension(file) == ".oMod2" || Path.GetExtension(file) == ".wMod2" || Path.GetExtension(file) == ".sMod")
                    this.ListView.AddItem(Path.GetFileName(file));
            }
        }

        public override void Update()
        {
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
            if (this.OK.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
            if (DeleteButton.IsClicked)
            {
                string sdFile = CreatorMain.Export_ModFile_Directory + "/" + (string)this.ListView.SelectedItem;
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
            this.ExportOnekeyButton.IsEnabled = File.Exists(CreatorMain.OneKeyFile);
            this.ExportCopyButton.IsEnabled = File.Exists(CreatorMain.CopyFile);
            this.ExportOldCopyButton.IsEnabled = File.Exists(CreatorMain.CopyFile);
            this.ExportOldOnekeyButton.IsEnabled = File.Exists(CreatorMain.OneKeyFile);
            this.DerivedSpecialButton.IsEnabled = File.Exists(CreatorMain.SpecialCopyFile);
            if (this.ExportOnekeyButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView,DataType.OneKey));
            if (this.ExportCopyButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView));
            if (this.ExportOldCopyButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView,DataType.OldCopy));
            if (this.ExportOldOnekeyButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView, DataType.OldOneKey));
            if (this.DerivedSpecialButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new DerivedDialog(player, this, this.ListView, DataType.SpecialCopy));
            if (this.ImportButton.IsClicked)
            {
                Task.Run(() =>
                {
                    if (!Directory.Exists(CreatorMain.CacheDirectory))
                        Directory.CreateDirectory(CreatorMain.CacheDirectory);
                    string fileName = CreatorMain.Export_ModFile_Directory + "/" + (string)this.ListView.SelectedItem;
                    if (FileOperation.IsFileInUse(fileName))
                    {
                        player.ComponentGui.DisplaySmallMessage($"操作失败...\n{fileName}", true, false);
                        DialogsManager.HideDialog(this);
                        return;
                    }
                    if (Path.GetExtension(fileName) == ".oMod2")
                    {
                        OnekeyGeneration.ImportOnekeyoMod2(CreatorMain.OneKeyFile, fileName);
                        player.ComponentGui.DisplaySmallMessage("导入一键生成MOD配置文件成功！", true, false);
                    }
                    else if (Path.GetExtension(fileName) == ".wMod2")
                    {
                        CopyAndPaste.ImportCopywMod2(CreatorMain.CopyFile, fileName);
                        player.ComponentGui.DisplaySmallMessage("导入复制MOD配置文件成功！", true, false);
                    }
                    else if (Path.GetExtension(fileName) == ".wMod")
                    {
                        CopyAndPaste.ImportCopywMod(CreatorMain.CopyFile, fileName);
                        player.ComponentGui.DisplaySmallMessage("导入复制MOD配置文件成功！", true, false);
                    }
                    else if (Path.GetExtension(fileName) == ".oMod")
                    {
                        player.ComponentGui.DisplaySmallMessage("抱歉，一键生成的旧文件数据无法导入！", true, false);
                    }
                    else if (Path.GetExtension(fileName) == ".sMod")
                    {
                        if (!Directory.Exists(CreatorMain.CacheDirectory))
                            Directory.CreateDirectory(CreatorMain.CacheDirectory);
                        string sdFile = CreatorMain.SpecialCopyFile;
                        if (!FileOperation.IsFileInUse(fileName) && (!File.Exists(sdFile) || !FileOperation.IsFileInUse(sdFile)))
                        {
                            FileStream fileStream = new FileStream(fileName, FileMode.Open);
                            FileStream file = new FileStream(sdFile, FileMode.Create);
                            fileStream.CopyTo(file);
                            file.Dispose();
                            fileStream.Dispose();
                            player.ComponentGui.DisplaySmallMessage("导入成功！", true, false);
                        }
                        else
                            player.ComponentGui.DisplaySmallMessage("操作失败！", true, false);
                    }
                    else
                    {
                        player.ComponentGui.DisplaySmallMessage($"操作失败...\n{fileName}", true, false);
                    }
                });
                DialogsManager.HideDialog(this);
            }
        }
    }
}
