using Game;
using Engine;
using System.Xml.Linq;

namespace CreatorModAPI
{
    public class EditRegionDialog : Dialog
    {
        public SliderWidget TemperatureSlider;
        public SliderWidget HumiditySlider;
        public SliderWidget TopHeightSlider;
        public LabelWidget SliderData;
        public ButtonWidget OKButton, cancelButton;
        private SubsystemTerrain subsystemTerrain;
        private CreatorAPI creatorAPI;
        private ComponentPlayer player;
        public EditRegionDialog(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.player = creatorAPI.componentMiner.ComponentPlayer;
            this.subsystemTerrain = creatorAPI.componentMiner.Project.FindSubsystem<SubsystemTerrain>(true);
            XElement node = ContentManager.Get<XElement>("NewWidgets/Dialog/编辑区域");
            WidgetsManager.LoadWidgetContents(this, this, node);
            this.SliderData = this.Children.Find<LabelWidget>("滑条数据", true);
            this.TemperatureSlider = this.Children.Find<SliderWidget>("滑条1", true);
            this.HumiditySlider = this.Children.Find<SliderWidget>("滑条2", true);
            this.TopHeightSlider = this.Children.Find<SliderWidget>("滑条3", true);
            this.OKButton = this.Children.Find<ButtonWidget>("确定", true);
            this.cancelButton = this.Children.Find<ButtonWidget>("取消");
        }
        public override void Update()
        {
            if (this.OKButton.IsClicked)
            {
                Point3 Start, End;
                Start = creatorAPI.Position[0];
                End = creatorAPI.Position[1];
                CreatorMain.Math.StartEnd(ref Start, ref End);
                for (int x = End.X; x <= Start.X; x++)
                {
                    for (int z = End.Z; z <= Start.Z; z++)
                    {
                        subsystemTerrain.Terrain.SetTemperature(x, z, (int)TemperatureSlider.Value);
                        subsystemTerrain.Terrain.SetHumidity(x, z, (int)HumiditySlider.Value);
                        subsystemTerrain.Terrain.SetTopHeight(x, z, (int)TopHeightSlider.Value);
                        subsystemTerrain.Terrain.GetChunkAtCoords(x>>4, z>>4).State = TerrainChunkState.Valid;
                        subsystemTerrain.Terrain.GetChunkAtCoords(x >> 4, z >> 4).State = TerrainChunkState.InvalidLight;
                    }
                }
                this.player.ComponentGui.DisplaySmallMessage("修改成功", true, true);
                DialogsManager.HideDialog(this);
            }
            if (this.cancelButton.IsClicked) DialogsManager.HideDialog(this);
            SliderData.Text = $"温度 :{(int)TemperatureSlider.Value} 湿度 :{(int)HumiditySlider.Value} 高度 :{(int)TopHeightSlider.Value}";
        }


    }
}
