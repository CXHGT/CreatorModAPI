using System;
using Game;
using TemplatesDatabase;
using Engine;
using System.IO;
using System.Collections.Generic;
using Engine.Graphics;
using System.Xml.Linq;
using Engine.Media;
using System.Net.Http;

namespace CreatorModAPI
{
    public class CreatorBlockBehavior : SubsystemBlockBehavior, IDrawable
    {

        public Dictionary<ComponentPlayer, CreatorAPI> dictionaryPlayers = new Dictionary<ComponentPlayer, CreatorAPI>();
        private List<ComponentPlayer> listPlayer = new List<ComponentPlayer>();
        public override int[] HandledBlocks => new int[] { 195 };
        public int[] DrawOrders => new int[] { 800 };
        public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
        {
            TerrainRaycastResult? terrainRaycastResult = componentMiner.PickTerrainForDigging(start, direction);
            ComponentPlayer Player = componentMiner.ComponentPlayer;
            if (terrainRaycastResult.HasValue)
            {
                CreatorAPI creatorAPI;
                if (!dictionaryPlayers.TryGetValue(Player, out creatorAPI))
                {
                    listPlayer.Add(Player);
                    creatorAPI = new CreatorAPI(componentMiner);
                    dictionaryPlayers.Add(Player, creatorAPI);
                }
                creatorAPI.OnUse((TerrainRaycastResult)terrainRaycastResult);
            }
            return base.OnUse(start,direction,componentMiner);
        }
        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer Player)
        {
/*            Run();*/
            if (Player.ComponentGui.ModalPanelWidget is CreatorWidget)
            {
                Player.ComponentGui.ModalPanelWidget = null;
            }
            else
            {
                CreatorAPI creatorAPI;
                if (!dictionaryPlayers.TryGetValue(Player, out creatorAPI))
                {
                    listPlayer.Add(Player);
                    creatorAPI = new CreatorAPI(Player.ComponentMiner);
                    dictionaryPlayers.Add(Player, creatorAPI);
                }
                if (CreatorMain.canUse)
                {
                    Player.ComponentGui.ModalPanelWidget = new CreatorWidget(creatorAPI);
                }
                else
                {
                    DialogsManager.ShowDialog(Player.View.GameWidget, new PasswordDialog(Player));
                }
            }
            return true;
        }
        public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
        {
            Projectile projectile = worldItem as Projectile;
            (((projectile != null) ? projectile.Owner : null) as ComponentPlayer).ComponentBody.Position = worldItem.Position;
            return true;
        }
        public void Draw(Camera camera, int drawOrder)
        {
            foreach (ComponentPlayer player in listPlayer)
            {
                if(player.ComponentHealth.Health <= 0f)
                {
                    listPlayer.Remove(player);
                    break;
                }
                CreatorAPI creatorAPI;
                Vector3 right = Vector3.TransformNormal(0.03f * Vector3.Normalize(Vector3.Cross(camera.ViewDirection, camera.ViewUp)), camera.ViewMatrix);
                Vector3 down = Vector3.TransformNormal(-0.03f * Vector3.UnitY, camera.ViewMatrix);
                BitmapFont bitmapFont = ContentManager.Get<BitmapFont>("Fonts/Pericles32");
                
                if (dictionaryPlayers.TryGetValue(player, out creatorAPI))
                {
                    if (creatorAPI.Position[0].Y != -1 && creatorAPI.Position[1].Y != -1)
                    {
                        Point3 Start = creatorAPI.Position[0];
                        Point3 End = creatorAPI.Position[1];
                        CreatorMain.Math.StartEnd(ref Start, ref End);
                        Vector3 vector = new Vector3(End.X, End.Y, End.Z);
                        Vector3 vector2 = new Vector3(Start.X + 1, Start.Y + 1, Start.Z + 1);
                        BoundingBox boundingBox = new BoundingBox(vector, vector2);
                        creatorAPI.primitivesRenderer3D = new PrimitivesRenderer3D();
                        creatorAPI.primitivesRenderer3D.FlatBatch(-1, DepthStencilState.None, null, null).QueueBoundingBox(boundingBox, Color.Blue);


                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("1", new Vector3(creatorAPI.Position[0]) + new Vector3(0,0.5f,0), right, down, Color.Blue);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("2", new Vector3(creatorAPI.Position[1]) + new Vector3(0,0.5f,0), right, down, Color.Blue);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("x", new Vector3(Start.X + 1,End.Y+1,End.Z + 1), right, down, Color.Blue);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("z", new Vector3(End.X,End.Y + 1,Start.Z + 2), right, down, Color.Blue);
                        creatorAPI.primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, 2147483647);
                    }
                    if (creatorAPI.Position[2].Y != -1 && creatorAPI.Position[3].Y != -1)
                    {
                        Point3 Start = creatorAPI.Position[2];
                        Point3 End = creatorAPI.Position[3];
                        CreatorMain.Math.StartEnd(ref Start, ref End);
                        Vector3 vector = new Vector3(End.X, End.Y, End.Z);
                        Vector3 vector2 = new Vector3(Start.X + 1, Start.Y + 1, Start.Z + 1);
                        BoundingBox boundingBox = new BoundingBox(vector, vector2);
                        creatorAPI.primitivesRenderer3D = new PrimitivesRenderer3D();
                        creatorAPI.primitivesRenderer3D.FlatBatch(-1, DepthStencilState.None, null, null).QueueBoundingBox(boundingBox, Color.Red);


                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("3", new Vector3(creatorAPI.Position[2]) + new Vector3(0,0.5f,0), right, down, Color.Red);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("4", new Vector3(creatorAPI.Position[3]) + new Vector3(0,0.5f,0), right, down, Color.Red);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("x", new Vector3(Start.X + 1, End.Y + 1, End.Z + 1), right, down, Color.Red);
                        creatorAPI.primitivesRenderer3D.FontBatch(bitmapFont, -1, DepthStencilState.None).QueueText("z", new Vector3(End.X, End.Y + 1, Start.Z + 2), right, down, Color.Red);
                        creatorAPI.primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, 2147483647);
                    }
                }
            }
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
                    if(this.TextBox.Text == CreatorMain.password)
                    {
                        CreatorMain.canUse = true;
                        this.player.ComponentGui.DisplaySmallMessage($"创世神{CreatorMain.version}功能开启",true,false);
                    }
                    else
                    {
                        this.player.ComponentGui.DisplaySmallMessage($"创世神{CreatorMain.version}功能开启失败", true, false);
                    }
                    DialogsManager.HideDialog(this);
                }
            }

        }
    }
}
