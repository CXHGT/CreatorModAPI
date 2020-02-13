using Engine;
using Engine.Graphics;
using Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TemplatesDatabase;

namespace CreatorModAPI
{
    public class EditWorldDialog : Dialog
    {
        private ComponentPlayer player;

        private ButtonWidget paletteButton;

        private RectangleWidget blocksTextureIcon;

        private LabelWidget blocksTextureLabel;

        private LabelWidget blocksTextureDetails;

        private ButtonWidget blocksTextureButton;

        private ButtonWidget supernaturalCreaturesButton;
        private ButtonWidget environmentBehaviorButton;
        private ButtonWidget timeOfDayButton;
        private ButtonWidget weatherEffectsButton;
        private ButtonWidget adventureRespawnButton;
        private ButtonWidget adventureSurvivalMechanicsButton;
        private LabelWidget terrainGenerationLabel;
        private ButtonWidget terrainGenerationButton;
        private SliderWidget seaLevelOffsetSlider;
        private SliderWidget temperatureOffsetSlider;
        private SliderWidget humidityOffsetSlider;
        private SliderWidget biomeSizeSlider;
        private Widget islandTerrainPanel;
        private SliderWidget islandSizeNS;
        private SliderWidget islandSizeEW;
        private Widget flatTerrainPanel;
        private Widget continentTerrainPanel;
        private SliderWidget flatTerrainLevelSlider;
        private BlockIconWidget flatTerrainBlock;
        private LabelWidget flatTerrainBlockLabel;
        private ButtonWidget flatTerrainBlockButton;
        private CheckboxWidget flatTerrainMagmaOceanCheckbox;
        private ButtonWidget OKButton;
        private ButtonWidget UpdataWorldButton;
        private ButtonWidget UpdataButton;
        private WorldSettings worldSettings;
        private CreatorAPI creatorAPI;
        private float[] islandSizes = new float[]
        {
            30f,
            40f,
            50f,
            60f,
            80f,
            100f,
            120f,
            150f,
            200f,
            250f,
            300f,
            400f,
            500f,
            600f,
            800f,
            1000f,
            1200f,
            1500f,
            2000f,
            2500f
        };
        private float[] biomeSizes = new float[]
        {
            0.33f,
            0.5f,
            0.75f,
            1f,
            1.5f,
            2f,
            3f
        };

        private BlocksTexturesCache blockTexturesCache = new BlocksTexturesCache();

        public EditWorldDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/编辑世界");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.worldSettings = player.Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings;
            this.OKButton = Children.Find<ButtonWidget>("确定");
            this.UpdataWorldButton = Children.Find<ButtonWidget>("重载");
            this.UpdataButton = Children.Find<ButtonWidget>("刷新");
            this.paletteButton = this.Children.Find<ButtonWidget>("Palette", true);
            this.blocksTextureIcon = this.Children.Find<RectangleWidget>("BlocksTextureIcon", true);
            this.blocksTextureLabel = this.Children.Find<LabelWidget>("BlocksTextureLabel", true);
            this.blocksTextureDetails = this.Children.Find<LabelWidget>("BlocksTextureDetails", true);
            this.blocksTextureButton = this.Children.Find<ButtonWidget>("BlocksTextureButton", true);
            this.supernaturalCreaturesButton = this.Children.Find<ButtonWidget>("SupernaturalCreatures", true);
            this.environmentBehaviorButton = this.Children.Find<ButtonWidget>("EnvironmentBehavior", true);
            this.timeOfDayButton = this.Children.Find<ButtonWidget>("TimeOfDay", true);
            this.weatherEffectsButton = this.Children.Find<ButtonWidget>("WeatherEffects", true);
            this.adventureRespawnButton = this.Children.Find<ButtonWidget>("AdventureRespawn", true);
            this.adventureSurvivalMechanicsButton = this.Children.Find<ButtonWidget>("AdventureSurvivalMechanics", true);
            this.terrainGenerationLabel = this.Children.Find<LabelWidget>("TerrainGenerationLabel", true);
            this.terrainGenerationButton = this.Children.Find<ButtonWidget>("TerrainGeneration", true);
            this.seaLevelOffsetSlider = this.Children.Find<SliderWidget>("SeaLevelOffset", true);
            this.temperatureOffsetSlider = this.Children.Find<SliderWidget>("TemperatureOffset", true);
            this.humidityOffsetSlider = this.Children.Find<SliderWidget>("HumidityOffset", true);
            this.biomeSizeSlider = this.Children.Find<SliderWidget>("BiomeSize", true);
            this.islandTerrainPanel = this.Children.Find<Widget>("IslandTerrainPanel", true);
            this.islandSizeNS = this.Children.Find<SliderWidget>("IslandSizeNS", true);
            this.islandSizeEW = this.Children.Find<SliderWidget>("IslandSizeEW", true);
            this.flatTerrainPanel = this.Children.Find<Widget>("FlatTerrainPanel", true);
            this.continentTerrainPanel = this.Children.Find<Widget>("ContinentTerrainPanel", true);
            this.flatTerrainLevelSlider = this.Children.Find<SliderWidget>("FlatTerrainLevel", true);
            this.flatTerrainBlock = this.Children.Find<BlockIconWidget>("FlatTerrainBlock", true);
            this.flatTerrainBlockLabel = this.Children.Find<LabelWidget>("FlatTerrainBlockLabel", true);
            this.flatTerrainBlockButton = this.Children.Find<ButtonWidget>("FlatTerrainBlockButton", true);
            this.flatTerrainMagmaOceanCheckbox = this.Children.Find<CheckboxWidget>("MagmaOcean", true);

            this.islandSizeEW.MinValue = 0f;
            this.islandSizeEW.MaxValue = (float)(this.islandSizes.Length - 1);
            this.islandSizeEW.Granularity = 1f;
            this.islandSizeNS.MinValue = 0f;
            this.islandSizeNS.MaxValue = (float)(this.islandSizes.Length - 1);
            this.islandSizeNS.Granularity = 1f;
            this.biomeSizeSlider.MinValue = 0f;
            this.biomeSizeSlider.MaxValue = (float)(this.biomeSizes.Length - 1);
            this.biomeSizeSlider.Granularity = 1f;
        }

        public override void Update()
        {
            if (this.UpdataButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
                SubsystemTerrain subsystemTerrain = GameManager.Project.FindSubsystem<SubsystemTerrain>();
                foreach (TerrainChunk terrainChunk in subsystemTerrain.Terrain.AllocatedChunks)
                {
                    foreach (SubsystemBlockBehavior subsystemBlockBehavior in GameManager.Project.FindSubsystem<SubsystemBlockBehaviors>().BlockBehaviors)
                    {
                        subsystemBlockBehavior.OnChunkDiscarding(terrainChunk);
                    }
                    int chunkX = terrainChunk.Coords.X;
                    int chunkY = terrainChunk.Coords.Y;
                    subsystemTerrain.Dispose();
                    subsystemTerrain.Load(new ValuesDictionary());
                    player.ComponentGui.DisplaySmallMessage("刷新完成！", true, false);
                }
            }
            if (this.OKButton.IsClicked) DialogsManager.HideDialog(this);
            if (this.UpdataWorldButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
                WorldInfo world = GameManager.WorldInfo;
                GameManager.SaveProject(true, true);
                GameManager.DisposeProject();
                object[] expr_E9 = new object[2];
                expr_E9[0] = world;
                ScreensManager.SwitchScreen("GameLoading", expr_E9);
            }
            if (this.paletteButton.IsClicked)
            {
                    DialogsManager.ShowDialog(null, new CreatorModAPIEditPaletteDialog(worldSettings.Palette));
            }
            
            Texture2D texture = this.blockTexturesCache.GetTexture(this.worldSettings.BlocksTextureName);
            this.blocksTextureIcon.Subtexture = new Subtexture(texture, Vector2.Zero, Vector2.One);
            this.blocksTextureLabel.Text = BlocksTexturesManager.GetDisplayName(this.worldSettings.BlocksTextureName);
            this.blocksTextureDetails.Text = string.Format("{0}x{1}", texture.Width, texture.Height);
            if (this.blocksTextureButton.IsClicked)
            {
                BlocksTexturesManager.UpdateBlocksTexturesList();
                ListSelectionDialog dialog = new ListSelectionDialog("Select Blocks Texture", BlocksTexturesManager.BlockTexturesNames, 64f, delegate (object item)
                {
                    XElement node = ContentManager.Get<XElement>("Widgets/BlocksTextureItem");
                    ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget(this, node, null);
                    Texture2D texture2 = this.blockTexturesCache.GetTexture((string)item);
                    containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Text", true).Text = BlocksTexturesManager.GetDisplayName((string)item);
                    containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Details", true).Text = string.Format("{0}x{1}", texture2.Width, texture2.Height);
                    containerWidget.Children.Find<RectangleWidget>("BlocksTextureItem.Icon", true).Subtexture = new Subtexture(texture2, Vector2.Zero, Vector2.One);
                    return containerWidget;
                }, delegate (object item)
                {
                    this.worldSettings.BlocksTextureName = (string)item;
                    SubsystemBlocksTexture subsystemBlocksTexture = GameManager.Project.FindSubsystem<SubsystemBlocksTexture>();
                    subsystemBlocksTexture.Dispose();
                    subsystemBlocksTexture.Load(new ValuesDictionary());

                });
                DialogsManager.ShowDialog(null, dialog);
            }
            if (this.supernaturalCreaturesButton.IsClicked)
            {
                this.worldSettings.AreSupernaturalCreaturesEnabled = !this.worldSettings.AreSupernaturalCreaturesEnabled;
            }
            if (this.environmentBehaviorButton.IsClicked)
            {
                IList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(EnvironmentBehaviorMode));
                this.worldSettings.EnvironmentBehaviorMode = (EnvironmentBehaviorMode)((enumValues2.IndexOf((int)this.worldSettings.EnvironmentBehaviorMode) + 1) % enumValues2.Count);
            }
            if (this.timeOfDayButton.IsClicked)
            {
                IList<int> enumValues3 = EnumUtils.GetEnumValues(typeof(TimeOfDayMode));
                this.worldSettings.TimeOfDayMode = (TimeOfDayMode)((enumValues3.IndexOf((int)this.worldSettings.TimeOfDayMode) + 1) % enumValues3.Count);
            }
            if (this.weatherEffectsButton.IsClicked)
            {
                this.worldSettings.AreWeatherEffectsEnabled = !this.worldSettings.AreWeatherEffectsEnabled;
            }
            if (this.adventureRespawnButton.IsClicked)
            {
                this.worldSettings.IsAdventureRespawnAllowed = !this.worldSettings.IsAdventureRespawnAllowed;
            }
            if (this.adventureSurvivalMechanicsButton.IsClicked)
            {
                this.worldSettings.AreAdventureSurvivalMechanicsEnabled = !this.worldSettings.AreAdventureSurvivalMechanicsEnabled;
            }
            if (this.terrainGenerationButton.IsClicked)
            {
                IList<int> enumValues = EnumUtils.GetEnumValues(typeof(TerrainGenerationMode));
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select World Type", enumValues, 56f, (object e) => ((TerrainGenerationMode)e).ToString(), delegate (object e)
                 {
                     this.worldSettings.TerrainGenerationMode = (TerrainGenerationMode)e;
                     SubsystemTerrain subsystemTerrain = GameManager.Project.FindSubsystem<SubsystemTerrain>();
                     if ((TerrainGenerationMode)e == TerrainGenerationMode.Flat)
                     {
                         subsystemTerrain.TerrainContentsGenerator = new TerrainContentsGeneratorFlat(subsystemTerrain);
                     }
                     else
                     {
                         subsystemTerrain.TerrainContentsGenerator = new TerrainContentsGenerator(subsystemTerrain);
                     }
                 }));
            }
            if (this.seaLevelOffsetSlider.IsSliding)
            {
                this.worldSettings.SeaLevelOffset = (int)this.seaLevelOffsetSlider.Value;
            }
            if (this.temperatureOffsetSlider.IsSliding)
            {
                this.worldSettings.TemperatureOffset = this.temperatureOffsetSlider.Value;
            }
            if (this.humidityOffsetSlider.IsSliding)
            {
                this.worldSettings.HumidityOffset = this.humidityOffsetSlider.Value;
            }
            if (this.biomeSizeSlider.IsSliding)
            {
                this.worldSettings.BiomeSize = this.biomeSizes[MathUtils.Clamp((int)this.biomeSizeSlider.Value, 0, this.biomeSizes.Length - 1)];
            }
            if (this.islandSizeEW.IsSliding)
            {
                this.worldSettings.IslandSize.X = this.islandSizes[MathUtils.Clamp((int)this.islandSizeEW.Value, 0, this.islandSizes.Length - 1)];
            }
            if (this.islandSizeNS.IsSliding)
            {
                this.worldSettings.IslandSize.Y = this.islandSizes[MathUtils.Clamp((int)this.islandSizeNS.Value, 0, this.islandSizes.Length - 1)];
            }
            if (this.flatTerrainLevelSlider.IsSliding)
            {
                this.worldSettings.TerrainLevel = (int)this.flatTerrainLevelSlider.Value;
            }
            if (this.flatTerrainBlockButton.IsClicked)
            {
                int[] items = new int[]
                {
                    8,
                    2,
                    7,
                    3,
                    67,
                    66,
                    4,
                    5,
                    26,
                    73,
                    21,
                    46,
                    47,
                    15,
                    62,
                    68,
                    126,
                    71,
                    1
                };
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", items, 72f, delegate (object index)
                {
                    XElement node = ContentManager.Get<XElement>("Widgets/SelectBlockItem");
                    ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget(null, node, null);
                    containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
                    containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));
                    return containerWidget;
                }, delegate (object index)
                {
                    this.worldSettings.TerrainBlockIndex = (int)index;
                }));
            }
            if (this.flatTerrainMagmaOceanCheckbox.IsClicked)
            {
                this.worldSettings.TerrainOceanBlockIndex = ((this.worldSettings.TerrainOceanBlockIndex == 18) ? 92 : 18);
            }
            this.islandTerrainPanel.IsVisible = (this.worldSettings.TerrainGenerationMode == TerrainGenerationMode.Island);
            this.flatTerrainPanel.IsVisible = (this.worldSettings.TerrainGenerationMode == TerrainGenerationMode.Flat);
            this.continentTerrainPanel.IsVisible = (this.worldSettings.TerrainGenerationMode == TerrainGenerationMode.Continent);
            this.flatTerrainLevelSlider.Value = (float)this.worldSettings.TerrainLevel;
            this.flatTerrainLevelSlider.Text = this.worldSettings.TerrainLevel.ToString();
            this.flatTerrainBlock.Contents = this.worldSettings.TerrainBlockIndex;
            this.flatTerrainMagmaOceanCheckbox.IsChecked = (this.worldSettings.TerrainOceanBlockIndex == 92);
            this.seaLevelOffsetSlider.Value = (float)this.worldSettings.SeaLevelOffset;
            this.seaLevelOffsetSlider.Text = WorldOptionsScreen.FormatOffset((float)this.worldSettings.SeaLevelOffset);
            this.temperatureOffsetSlider.Value = this.worldSettings.TemperatureOffset;
            this.temperatureOffsetSlider.Text = WorldOptionsScreen.FormatOffset(this.worldSettings.TemperatureOffset);
            this.humidityOffsetSlider.Value = this.worldSettings.HumidityOffset;
            this.humidityOffsetSlider.Text = WorldOptionsScreen.FormatOffset(this.worldSettings.HumidityOffset);
            this.biomeSizeSlider.Value = (float)FindNearestIndex(this.biomeSizes, this.worldSettings.BiomeSize);
            this.biomeSizeSlider.Text = this.worldSettings.BiomeSize.ToString() + "x";
            this.islandSizeEW.Value = (float)FindNearestIndex(this.islandSizes, this.worldSettings.IslandSize.X);
            this.islandSizeEW.Text = this.worldSettings.IslandSize.X.ToString();
            this.islandSizeNS.Value = (float)FindNearestIndex(this.islandSizes, this.worldSettings.IslandSize.Y);
            this.islandSizeNS.Text = this.worldSettings.IslandSize.Y.ToString();
            this.supernaturalCreaturesButton.Text = (this.worldSettings.AreSupernaturalCreaturesEnabled ? "Enabled" : "Disabled");
            this.environmentBehaviorButton.Text = this.worldSettings.EnvironmentBehaviorMode.ToString();
            this.timeOfDayButton.Text = this.worldSettings.TimeOfDayMode.ToString();
            this.weatherEffectsButton.Text = (this.worldSettings.AreWeatherEffectsEnabled ? "Enabled" : "Disabled");
            this.adventureRespawnButton.Text = (this.worldSettings.IsAdventureRespawnAllowed ? "Allowed" : "Not Allowed");
            this.adventureSurvivalMechanicsButton.Text = (this.worldSettings.AreAdventureSurvivalMechanicsEnabled ? "Enabled" : "Disabled");
            this.terrainGenerationLabel.Text = this.worldSettings.TerrainGenerationMode.ToString();
        }

        private int FindNearestIndex(IList<float> list, float v)
        {
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (MathUtils.Abs(list[i] - v) < MathUtils.Abs(list[num] - v))
                {
                    num = i;
                }
            }
            return num;
        }

    }
}
