using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TemplatesDatabase;

namespace CreatorModAPI
{
    public class CreatorWidget : CanvasWidget
    {
        private ComponentPlayer player;
        private CreatorAPI creatorAPI;
        private WorldSettings worldSettings;
        private ButtonWidget SphereButton;
        private ButtonWidget PrismButton;
        private ButtonWidget PyramidButton;
        private ButtonWidget CylindricalButton;
        private ButtonWidget PillarsButton;
        private ButtonWidget PrismColumnButton;
        private ButtonWidget RectangularButton;
        private ButtonWidget CircleButton;
        private ButtonWidget MazeButton;
        private ButtonWidget SpiralButton;
        private ButtonWidget LevelSetButton;
        private ButtonWidget TransferButton;
        private ButtonWidget SetPositionButton;
        private ButtonWidget RevokeButton;
        private ButtonWidget SetSpawn;
        private ButtonWidget SetLinkButton;
        private ButtonWidget RemoveItemButton;
        private ButtonWidget RemoveAnimalButton;
        private ButtonWidget MountainButton;
        private ButtonWidget SetModeButton;
        private ButtonWidget SetButton;
        private ButtonWidget ClearCacheButton;
        private ButtonWidget CopyPasteButton;
        private ButtonWidget OnekeyButton;
        private ButtonWidget ReplaceButton;
        private ButtonWidget ModButton;
        private ButtonWidget EditRegionButton;
        private ButtonWidget EditWorldButton;
        private ButtonWidget PenetrateButton;
        private ButtonWidget TerrainTestButton;
        private ButtonWidget FillingButton;
        private ButtonWidget PavageButton;
        private ButtonWidget ClearBlockButton;
        private ButtonWidget SetPositionCarefulButton;
        private ButtonWidget AdjustPositionButton;
        private ButtonWidget SetDifficultyButton;
        private ButtonWidget ThreePointPlaneButton;
        private ButtonWidget FourPointSpaceButton;
        private ButtonWidget LightWorldButton;
        public CreatorWidget(CreatorAPI creatorAPI)
        {
            this.player = creatorAPI.componentMiner.ComponentPlayer; ;
            this.creatorAPI = creatorAPI;
            XElement node;
            if (creatorAPI.oldMainWidget)
                node = ContentManager.Get<XElement>("NewWidgets/CreatorAPIWidget");
            else
                node = ContentManager.Get<XElement>("NewWidgets/NewCreatorAPIWidget");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.worldSettings = player.Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings;

            this.SphereButton = Children.Find<ButtonWidget>("球体");
            this.PrismButton = Children.Find<ButtonWidget>("棱体");
            this.PyramidButton = Children.Find<ButtonWidget>("棱锥");
            this.CylindricalButton = Children.Find<ButtonWidget>("圆柱");
            this.PrismColumnButton = Children.Find<ButtonWidget>("棱柱");
            this.PillarsButton = Children.Find<ButtonWidget>("柱子");
            this.RectangularButton = Children.Find<ButtonWidget>("矩形");
            this.CircleButton = Children.Find<ButtonWidget>("圆环");
            this.MazeButton = Children.Find<ButtonWidget>("迷宫");
            this.FillingButton = Children.Find<ButtonWidget>("填充");
            this.PavageButton = Children.Find<ButtonWidget>("平铺");
            this.MountainButton = Children.Find<ButtonWidget>("山脉");
            this.TransferButton = Children.Find<ButtonWidget>("传送");
            this.LevelSetButton = Children.Find<ButtonWidget>("等级");
            this.SetButton = Children.Find<ButtonWidget>("设置");
            this.SpiralButton = Children.Find<ButtonWidget>("螺旋");
            this.PenetrateButton = Children.Find<ButtonWidget>("穿透");

            this.SetLinkButton = Children.Find<ButtonWidget>("连线");

            this.SetPositionButton = Children.Find<ButtonWidget>("设点");
            this.RevokeButton = Children.Find<ButtonWidget>("撤回");
            this.SetSpawn = Children.Find<ButtonWidget>("出生点");

            this.EditWorldButton = Children.Find<ButtonWidget>("编辑世界");
            this.EditRegionButton = Children.Find<ButtonWidget>("编辑区域");
            this.ModButton = Children.Find<ButtonWidget>("模组专用");
            this.ReplaceButton = Children.Find<ButtonWidget>("方块替换");
            this.OnekeyButton = Children.Find<ButtonWidget>("一键生成");
            this.CopyPasteButton = Children.Find<ButtonWidget>("复制粘贴");
            this.ClearCacheButton = Children.Find<ButtonWidget>("清除缓存");
            this.SetModeButton = Children.Find<ButtonWidget>("变更模式");
            
            this.RemoveItemButton = Children.Find<ButtonWidget>("清除掉落");
            this.RemoveAnimalButton = Children.Find<ButtonWidget>("清除动物");
            this.ClearBlockButton = Children.Find<ButtonWidget>("清理方块");
            this.TerrainTestButton = Children.Find<ButtonWidget>("测试地形");

            this.SetPositionCarefulButton = this.Children.Find<ButtonWidget>("精准设点");
            this.AdjustPositionButton = this.Children.Find<ButtonWidget>("点位调整");
            this.SetDifficultyButton = this.Children.Find<ButtonWidget>("变更难度");
            this.ThreePointPlaneButton = this.Children.Find<ButtonWidget>("3点成面");
            this.FourPointSpaceButton = this.Children.Find<ButtonWidget>("4点空间");
            this.LightWorldButton = this.Children.Find<ButtonWidget>("发光世界");

        }

        public override void Update()
        {
            if(this.SphereButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new SphereDialog(creatorAPI));
            if (this.PrismButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new PrismDialog(creatorAPI));
            if (this.PyramidButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new PyramidDialog(creatorAPI));
            if (this.CylindricalButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new CylindricalDialog(creatorAPI));
            if (this.PrismColumnButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new PrismColumnDialog(creatorAPI));
            if (this.PillarsButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new PillarsDialog(creatorAPI));
            if (this.RectangularButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new RectangularDialog(creatorAPI));
            if (this.CircleButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new CircleDialog(creatorAPI));
            if (this.MazeButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new MazeDialog(creatorAPI));
            if (this.PavageButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new PavageDialog(creatorAPI));
            if(this.FillingButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new FillingDialog(creatorAPI));
            if (this.MountainButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new MountainDialog(creatorAPI));
            if (this.TransferButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new TransferDialog(creatorAPI));
            if (this.LevelSetButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new LevelSetDialog(creatorAPI));
            if (this.SetButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new SetDialog(creatorAPI));
            if (this.SpiralButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget,new SpiralDialog(creatorAPI));
            if (this.PenetrateButton.IsClicked)
            {
                if (CreatorMain.Penetrate)
                {
                    foreach (int blockID in CreatorMain.PenetrateBlocksID)
                    {
                        BlocksManager.Blocks[blockID].IsCollidable = true;
                    }
                    CreatorMain.PenetrateBlocksID.Clear();
                }
                else
                {
                    this.worldSettings.EnvironmentBehaviorMode = EnvironmentBehaviorMode.Static;
                    foreach (Block block in BlocksManager.Blocks)
                    {
                        if (block.IsCollidable)
                        {
                            block.IsCollidable = false;
                            CreatorMain.PenetrateBlocksID.Add(block.BlockIndex);
                        }
                    }
                }
                CreatorMain.Penetrate = !CreatorMain.Penetrate;
            }
            if (this.LightWorldButton.IsClicked)
            {
                if (CreatorMain.LightWorld)
                {
                    foreach (int blockID in CreatorMain.LightWorldBlockID)
                    {
                        BlocksManager.Blocks[blockID].EmittedLightAmount = 0;
                    }
                    CreatorMain.LightWorldBlockID.Clear();
                }
                else
                {
                    foreach (Block block in BlocksManager.Blocks)
                    {
                        if (block.EmittedLightAmount == 0)
                        {
                            block.EmittedLightAmount = 15;
                            CreatorMain.LightWorldBlockID.Add(block.BlockIndex);
                        }
                    }
                }
                CreatorMain.LightWorld = !CreatorMain.LightWorld;
            }
            if (CreatorMain.LightWorld) this.LightWorldButton.Color = Color.Yellow; else this.LightWorldButton.Color = Color.White;

            if (CreatorMain.Penetrate) this.PenetrateButton.Color = Color.Yellow; else this.PenetrateButton.Color = Color.White;
            if(this.EditRegionButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new EditRegionDialog(creatorAPI));
            if(this.EditWorldButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new EditWorldDialog(creatorAPI));
            if (this.ClearBlockButton.IsClicked)
            {
                this.player.ComponentGui.DisplaySmallMessage("可在设置中关闭生成在来停止正在清理的进程，在超距模式下谨慎使用清理方块", true, false);
                this.creatorAPI.ClearBlock = !this.creatorAPI.ClearBlock;
            }
            if (this.creatorAPI.ClearBlock) this.ClearBlockButton.Color = Color.Yellow; else this.ClearBlockButton.Color = Color.White;
            if(this.SetLinkButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new TwoPointLineDialog(creatorAPI));
            /*            if (this.SetLinkButton.IsClicked)
                        {
                            if (this.creatorAPI.twoPointsOfAttachment)
                                this.creatorAPI.twoPointsOfAttachment = false;
                            else
                                this.creatorAPI.twoPointsOfAttachment = true;
                        }
                        if (this.creatorAPI.twoPointsOfAttachment) this.SetLinkButton.Color = Color.Yellow; else this.SetLinkButton.Color = Color.White;*/


            if (this.SetPositionButton.IsClicked)
            {
                Vector3 position = player.ComponentBody.Position;
                Point3 point3 = new Point3((int)position.X,(int)position.Y,(int)position.Z);
                int[] p = new int[4] { 1, 2, 3, 4 };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("选择设置的点", p, 56f, (object e) => $"设置点{(int)e}", delegate (object e)
                {
                    creatorAPI.Position[(int)e-1] = point3;
                    this.player.ComponentGui.DisplaySmallMessage($"成功设置点{(int)e}：\nX:{point3.X} , Y:{point3.Y} , Z:{point3.Z}", true, true);
                }));
            }
            if (this.creatorAPI.RevokeSwitch)
                if (creatorAPI.revokeData == null) this.RevokeButton.IsEnabled = false; else this.RevokeButton.IsEnabled = true;
            else 
                this.RevokeButton.IsEnabled = false;
            if (this.RevokeButton.IsClicked)
            {
                this.creatorAPI.revokeData.Render();
                this.player.ComponentGui.DisplaySmallMessage($"撤回成功", true, true);
            }
            if (this.SetSpawn.IsClicked)
            {
                Vector3 position = this.player.ComponentBody.Position;
                this.player.PlayerData.SpawnPosition = position + new Vector3(0f, 0.1f, 0f);
                this.player.ComponentGui.DisplaySmallMessage(string.Format("玩家重生点位置设置\n X: {0} Y : {1} Z : {2}", (int)position.X, (int)position.Y, (int)position.Z), true, true);
            }
            if (this.ModButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new ModDialog(creatorAPI));
            if (this.ReplaceButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new ReplaceDialog(creatorAPI));
            if (this.SetModeButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new SetModeDialog(creatorAPI));
            if (this.OnekeyButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new OnekeyGenerationDialog(creatorAPI));
            if (this.CopyPasteButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new CopyPasteDialog(creatorAPI));
            this.ClearCacheButton.IsEnabled = Directory.Exists(CreatorMain.CacheDirectory);
            if (this.ClearCacheButton.IsClicked) if (FileOperation.Delete(CreatorMain.CacheDirectory)) this.player.ComponentGui.DisplaySmallMessage("清除成功", true, true); else this.player.ComponentGui.DisplaySmallMessage("清除失败", true, true);
            if (this.RemoveAnimalButton.IsClicked)
            {
                int num = 0;
                foreach (ComponentCreature current in player.Project.FindSubsystem<SubsystemCreatureSpawn>(true).Creatures)
                {
                    if (!(current is ComponentPlayer))
                    {
                        current.ComponentSpawn.Despawn();
                        num++;
                    }
                }
                this.player.ComponentGui.DisplaySmallMessage(string.Format("清除成功，共清除{0}只动物", num), true, false);
            }
            if (this.RemoveItemButton.IsClicked)
            {
                int num = 0;
                foreach (Pickable item in GameManager.Project.FindSubsystem<SubsystemPickables>(true).Pickables)
                {
                    item.Count = 0;
                    item.ToRemove = true;
                    num++;
                }
                this.player.ComponentGui.DisplaySmallMessage(string.Format("清除成功，共清除{0}个掉落物", num), true, false);
            }
            if (this.TerrainTestButton.IsClicked)
            {
                IList<int> enumValues = EnumUtils.GetEnumValues(typeof(CreatorTerrainType));
                SubsystemTerrain subsystemTerrain = GameManager.Project.FindSubsystem<SubsystemTerrain>();
                DialogsManager.ShowDialog(null, new ListSelectionDialog("选择地形类型", enumValues, 56f, (object e) => ((TerrainGenerationMode)e).ToString(), delegate (object e)
                {
                    ITerrainContentsGenerator myTerrain;
                    if ((CreatorTerrainType)e == CreatorTerrainType.Flat)
                    {
                        myTerrain = new CreatorNormalTerrain(subsystemTerrain);
                    }
                    else
                    {
                        myTerrain = new CreatorNormalTerrain(subsystemTerrain);
                    }
                    subsystemTerrain.Dispose();
                    subsystemTerrain.Load(new ValuesDictionary());
                    subsystemTerrain.TerrainContentsGenerator = myTerrain;
                }));
            }
            if (this.SetPositionCarefulButton.IsClicked) 
            {
            }
            if (this.AdjustPositionButton.IsClicked)
            {

            }
            if (this.ThreePointPlaneButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new ThreePointToPlaneDialog(creatorAPI));
            if (this.FourPointSpaceButton.IsClicked) DialogsManager.ShowDialog(player.View.GameWidget, new FourPointSpaceDialog(creatorAPI));
            //.........




            if (this.SetDifficultyButton.IsClicked)
            {
                Vector3 position = player.ComponentBody.Position;
                Point3 point3 = new Point3((int)position.X, (int)position.Y, (int)position.Z);
                int[] p = new int[4] { 0, 1, 2, 3 };
                string[] difference = new string[4] { "容易", "一般", "困难", "地狱" };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("选择难度", p, 56f, (object e) => difference[(int)e], delegate (object e)
                {
                    this.player.ComponentGui.DisplaySmallMessage($"成功设置难度位: {difference[(int)e]}", true, true);
                }));
            }
        }
    }
}