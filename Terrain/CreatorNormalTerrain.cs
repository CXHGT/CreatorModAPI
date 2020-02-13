using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Game;

namespace CreatorModAPI
{
    public class CreatorNormalTerrain : ITerrainContentsGenerator
    {
        private WorldSettings worldSettings;
        private int worldSeed;
        private SubsystemTerrain subsystemTerrain;
        private SubsystemBottomSuckerBlockBehavior subsystemBottomSuckerBlockBehavior;
        public int OceanLevel => 64 + this.worldSettings.SeaLevelOffset;

        private Vector2 oceanCorner;
        private Vector2 temperatureOffset;
        public bool TGNewBiomeNoise;
        public float TGTurbulenceStrength;
        public float TGTurbulenceTopOffset;
        public float TGTurbulencePower;
        public float TGDensityBias;
        public float TGShoreFluctuations;
        public float TGShoreFluctuationsScaling;
        public float TGMountainsStrength;
        public float TGMountainsPeriod;
        public float TGMountainsPercentage;
        private Vector2 mountainsOffset;
        public float TGBiomeScaling;
        private Vector2 humidityOffset;
        public float TGOceanSlope;
        public float TGOceanSlopeVariation;
        public float TGIslandsFrequency;
        public float TGHillsStrength;
        public float TGRiversStrength;
        private Vector2 riversOffset;
        public float TGHeightBias;

        /*private Vector2? islandSize;

        private static float TGSurfaceMultiplier;
        public bool TGWater;
        public bool TGExtras;
        public bool TGCavesAndPockets;*/

        public CreatorNormalTerrain(SubsystemTerrain subsystemTerrain)
        {
            this.subsystemTerrain = subsystemTerrain;
            this.subsystemBottomSuckerBlockBehavior = subsystemTerrain.Project.FindSubsystem<SubsystemBottomSuckerBlockBehavior>(true);
            SubsystemGameInfo subsystemGameInfo = subsystemTerrain.Project.FindSubsystem<SubsystemGameInfo>(true);
            this.worldSettings = subsystemGameInfo.WorldSettings;
            this.worldSeed = subsystemGameInfo.WorldSeed;
            OldRandom oldRandom = new OldRandom(100 + this.worldSeed);
            Game.Random random = new Game.Random(this.worldSeed);
            float num = float.MaxValue;
            this.oceanCorner = new Vector2(random.UniformFloat(-100f, -100f), random.UniformFloat(-100f, -100f));
            this.temperatureOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
            this.humidityOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
            this.mountainsOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
            this.riversOffset = new Vector2(random.UniformFloat(-2000f, 2000f), random.UniformFloat(-2000f, 2000f));
            this.TGNewBiomeNoise = true;
            this.TGBiomeScaling = 2.5f * this.worldSettings.BiomeSize;
            this.TGShoreFluctuations = MathUtils.Clamp(2f * num, 0f, 150f);
            this.TGShoreFluctuationsScaling = MathUtils.Clamp(0.04f * num, 0.5f, 3f);
            this.TGOceanSlope = 0.006f;
            this.TGOceanSlopeVariation = 0.004f;
            this.TGIslandsFrequency = 0.01f;
            this.TGDensityBias = 55f;
            this.TGHeightBias = 1f;
            this.TGRiversStrength = 3f;
            this.TGMountainsStrength = 125f;
            this.TGMountainsPeriod = 0.0015f;
            this.TGMountainsPercentage = 0.15f;
            this.TGHillsStrength = 8f;
            this.TGTurbulenceStrength = 35f;
            this.TGTurbulenceTopOffset = 0f;
            this.TGTurbulencePower = 0.3f;
            TerrainContentsGenerator.TGSurfaceMultiplier = 2f;
           /*this.TGWater = true;
            this.TGExtras = true;
            this.TGCavesAndPockets = true;*/
        }
        /// <summary>
        /// 高度计算
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float CalculateHeight(float x, float z)
        {
            float num = this.TGOceanSlope + this.TGOceanSlopeVariation * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise(x + this.mountainsOffset.X, z + this.mountainsOffset.Y, 0.01f, 1, 2f, 0.5f) - 1f, 0.5f);
            float num2 = this.CalculateOceanShoreDistance(x, z);
            float num3 = MathUtils.Saturate(1f - 0.05f * MathUtils.Abs(num2));
            float num4 = MathUtils.Saturate(MathUtils.Sin(this.TGIslandsFrequency * num2));
            float num5 = MathUtils.Saturate(MathUtils.Saturate(-num * num2) - 0.85f * num4);
            float num6 = MathUtils.Saturate(MathUtils.Saturate(0.05f * (-num2 - 10f)) - num4);
            float num7 = this.CalculateMountainRangeFactor(x, z);
            float f = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.001f / this.TGBiomeScaling, 2, 1.97f, 0.8f);
            float f2 = (1f - num3) * SimplexNoise.OctavedNoise(x, z, 0.0017f / this.TGBiomeScaling, 2, 1.93f, 0.7f);
            float num8 = (1f - num6) * (1f - num3) * MathUtils.Saturate((num7 - 0.6f) / 0.4f);
            float num9 = (1f - num6) * MathUtils.Saturate((num7 - (1f - this.TGMountainsPercentage)) / this.TGMountainsPercentage);
            float num10 = 2f * SimplexNoise.OctavedNoise(x, z, 0.02f, 3, 1.93f, 0.8f) - 1f;
            float num11 = 1.5f * SimplexNoise.OctavedNoise(x, z, 0.004f, 4, 1.98f, 0.9f) - 0.5f;
            float num12 = MathUtils.Lerp(60f, 30f, MathUtils.Saturate(1f * num9 + 0.5f * num8 + MathUtils.Saturate(1f - num2 / 30f)));
            float x2 = MathUtils.Lerp(-2f, -4f, MathUtils.Saturate(num9 + 0.5f * num8));
            float num13 = MathUtils.Saturate(2.0f - num12 * MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.riversOffset.X, z + this.riversOffset.Y, 0.001f, 4, 2f, 0.5f) - 1f));
            float num14 = -50f * num5 + this.TGHeightBias;
            float num15 = MathUtils.Lerp(0f, 8f, f);
            float num16 = MathUtils.Lerp(0f, -6f, f2);
            float num17 = this.TGHillsStrength * num8 * num10;
            float num18 = this.TGMountainsStrength * num9 * num11;
            float f3 = this.TGRiversStrength * num13;
            float num19 = num14 + num15 + num16 + num18 + num17;
            float num20 = MathUtils.Min(MathUtils.Lerp(num19, x2, f3), num19);
            return MathUtils.Clamp(64f + num20, 10f, 123f);
        }
        /// <summary>
        /// 计算湿度
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int CalculateHumidity(float x, float z)
        {
            if (this.TGNewBiomeNoise)
            {
                return MathUtils.Clamp((int)(MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.humidityOffset.X, z + this.humidityOffset.Y, 0.0012f / this.TGBiomeScaling, 5, 2f, 0.7f) - 1.2f + this.worldSettings.HumidityOffset / 16f) * 16f), 0, 15);
            }
            return MathUtils.Clamp((int)((MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.humidityOffset.X, z + this.humidityOffset.Y, 0.0008f / this.TGBiomeScaling, 5, 1.97f, 1f) - 1.5f) + this.worldSettings.HumidityOffset / 16f) * 16f), 0, 15);
        }
        /// <summary>
        /// 计算山脉系数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float CalculateMountainRangeFactor(float x, float z)
        {
            return 1f - MathUtils.Abs(2f * SimplexNoise.OctavedNoise(x + this.mountainsOffset.X, z + this.mountainsOffset.Y, this.TGMountainsPeriod / this.TGBiomeScaling, 3, 1.91f, 0.75f) - 1f);
        }
        /// <summary>
        /// 计算海岸距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float CalculateOceanShoreDistance(float x, float z)
        {
            /*if (this.m_islandSize != null)
            {
                float num = this.CalculateOceanShoreX(z);
                float num2 = this.CalculateOceanShoreZ(x);
                float num3 = this.CalculateOceanShoreX(z + 1000f) + this.m_islandSize.Value.X;
                float num4 = this.CalculateOceanShoreZ(x + 1000f) + this.m_islandSize.Value.Y;
                return MathUtils.Min(x - num, z - num2, num3 - x, num4 - z);
            }*/
            float num5 = this.CalculateOceanShoreX(z);
            float num6 = this.CalculateOceanShoreZ(x);
            return MathUtils.Min(x - num5, z - num6);
        }
        private float CalculateOceanShoreX(float z)
        {
            return this.oceanCorner.X + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(z, 0f, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f);
        }
        private float CalculateOceanShoreZ(float x)
        {
            return this.oceanCorner.Y + this.TGShoreFluctuations * SimplexNoise.OctavedNoise(0f, x, 0.005f / this.TGShoreFluctuationsScaling, 4, 1.95f, 1f);
        }
        /// <summary>
        /// 计算温度
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int CalculateTemperature(float x, float z)
        {
            if (this.TGNewBiomeNoise)
            {
                return MathUtils.Clamp((int)(MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.temperatureOffset.X, z + this.temperatureOffset.Y, 0.0015f / this.TGBiomeScaling, 5, 2f, 0.7f) - 1.6f + this.worldSettings.TemperatureOffset / 16f) * 16f), 0, 15);
            }
            return MathUtils.Clamp((int)((MathUtils.Saturate(4f * SimplexNoise.OctavedNoise(x + this.temperatureOffset.X, z + this.temperatureOffset.Y, 0.0006f / this.TGBiomeScaling, 4, 1.93f, 1f) - 1.6f) + this.worldSettings.TemperatureOffset / 16f) * 16f), 0, 15);
        }
        public Vector3 FindCoarseSpawnPosition()
        {
            return new Vector3(this.oceanCorner.X, this.CalculateHeight(this.oceanCorner.X, this.oceanCorner.Y), this.oceanCorner.Y);
        }
        public void GenerateChunkContentsPass1(TerrainChunk chunk)
        {
            this.GenerateSurfaceParameters(chunk, 0, 0, 16, 8);
            this.GenerateTerrain(chunk, 0, 0, 16, 8);
        }
        public void GenerateChunkContentsPass2(TerrainChunk chunk)
        {
            this.GenerateSurfaceParameters(chunk, 0, 8, 16, 16);
            this.GenerateTerrain(chunk, 0, 8, 16, 16);
        }
        public void GenerateChunkContentsPass3(TerrainChunk chunk)
        {
            /*this.GenerateCaves(chunk);
            this.GeneratePockets(chunk);
            this.GenerateMinerals(chunk);
            this.GenerateSurface(chunk);
            this.PropagateFluidsDownwards(chunk);*/
        }
        public void GenerateChunkContentsPass4(TerrainChunk chunk)
        {
            //this.GenerateGrassAndPlants(chunk);
          //  this.GenerateTreesAndLogs(chunk);
            //this.GenerateSugarcane(chunk);
           // this.GenerateCacti(chunk);
         //   this.GeneratePumpkins(chunk);
           // this.GenerateCassavas(chunk);
          //  this.GenerateKelp(chunk);
          //  this.GenerateSeagrass(chunk);
          //  this.GenerateBottomSuckers(chunk);
          //  this.GenerateTraps(chunk);
           // this.GenerateIvy(chunk);
           // this.GenerateGraves(chunk);
            //this.GenerateSnowAndIce(chunk);
            //this.GenerateBedrockAndAir(chunk);
            //this.UpdateFluidIsTop(chunk);
        }
        private void GenerateSurfaceParameters(TerrainChunk chunk, int x1, int z1, int x2, int z2)
        {
            for (int i = x1; i < x2; i++)
            {
                for (int j = z1; j < z2; j++)
                {
                    int num = i + chunk.Origin.X;
                    int num2 = j + chunk.Origin.Y;
                    int temperature = this.CalculateTemperature((float)num, (float)num2);
                    int humidity = this.CalculateHumidity((float)num, (float)num2);
                    chunk.SetTemperatureFast(i, j, temperature);
                    chunk.SetHumidityFast(i, j, humidity);
                }
            }
        }
        private void GenerateTerrain(TerrainChunk chunk, int x1, int z1, int x2, int z2)
        {
            int num = x2 - x1;
            int num2 = z2 - z1;
            Terrain terrain = this.subsystemTerrain.Terrain;
            int num3 = chunk.Origin.X + x1;
            int num4 = chunk.Origin.Y + z1;
            TerrainContentsGenerator.Grid2d grid2d = new TerrainContentsGenerator.Grid2d(num, num2);
            TerrainContentsGenerator.Grid2d grid2d2 = new TerrainContentsGenerator.Grid2d(num, num2);
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    grid2d.Set(j, i, this.CalculateOceanShoreDistance((float)(j + num3), (float)(i + num4)));
                    grid2d2.Set(j, i, this.CalculateMountainRangeFactor((float)(j + num3), (float)(i + num4)));
                }
            }
            TerrainContentsGenerator.Grid3d grid3d = new TerrainContentsGenerator.Grid3d(num / 4 + 1, 17, num2 / 4 + 1);
            for (int k = 0; k < grid3d.SizeX; k++)
            {
                for (int l = 0; l < grid3d.SizeZ; l++)
                {
                    int num5 = k * 4 + num3;
                    int num6 = l * 4 + num4;
                    float num7 = this.CalculateHeight((float)num5, (float)num6);
                    float num8 = this.CalculateMountainRangeFactor((float)num5, (float)num6);
                    float num9 = MathUtils.Saturate(0.9f * (num8 - 0.8f) / 0.2f + 0.1f);
                    for (int m = 0; m < grid3d.SizeY; m++)
                    {
                        int num10 = m * 8;
                        float num11 = num7 - this.TGTurbulenceTopOffset;
                        float num12 = MathUtils.Lerp(0f, this.TGTurbulenceStrength * num9, MathUtils.Saturate((num11 - (float)num10) * 0.2f)) * MathUtils.PowSign(2f * SimplexNoise.OctavedNoise((float)num5, (float)(num10 + 1000), (float)num6, 0.008f, 3, 2f, 0.75f) - 1f, this.TGTurbulencePower);
                        float num13 = (float)num10 + num12;
                        float num14 = num7 - num13;
                        num14 += MathUtils.Max(4f * (this.TGDensityBias - (float)num10), 0f);
                        grid3d.Set(k, m, l, num14);
                    }
                }
            }
            int oceanLevel = this.OceanLevel;
            for (int n = 0; n < grid3d.SizeX - 1; n++)
            {
                for (int num15 = 0; num15 < grid3d.SizeZ - 1; num15++)
                {
                    for (int num16 = 0; num16 < grid3d.SizeY - 1; num16++)
                    {
                        float num17;
                        float num18;
                        float num19;
                        float num20;
                        float num21;
                        float num22;
                        float num23;
                        float num24;
                        grid3d.Get8(n, num16, num15, out num17, out num18, out num19, out num20, out num21, out num22, out num23, out num24);
                        float num25 = (num18 - num17) / 4f;
                        float num26 = (num20 - num19) / 4f;
                        float num27 = (num22 - num21) / 4f;
                        float num28 = (num24 - num23) / 4f;
                        float num29 = num17;
                        float num30 = num19;
                        float num31 = num21;
                        float num32 = num23;
                        for (int num33 = 0; num33 < 4; num33++)
                        {
                            float num34 = (num31 - num29) / 4f;
                            float num35 = (num32 - num30) / 4f;
                            float num36 = num29;
                            float num37 = num30;
                            for (int num38 = 0; num38 < 4; num38++)
                            {
                                float num39 = (num37 - num36) / 8f;
                                float num40 = num36;
                                int num41 = num33 + n * 4;
                                int num42 = num38 + num15 * 4;
                                int x3 = x1 + num41;
                                int z3 = z1 + num42;
                                float x4 = grid2d.Get(num41, num42);
                                float num43 = grid2d2.Get(num41, num42);
                                int temperatureFast = chunk.GetTemperatureFast(x3, z3);
                                int humidityFast = chunk.GetHumidityFast(x3, z3);
                                float f = num43 - 0.01f * (float)humidityFast;
                                float num44 = MathUtils.Lerp(100f, 0f, f);
                                float num45 = MathUtils.Lerp(300f, 30f, f);
                                bool flag = (temperatureFast > 8 && humidityFast < 8 && num43 < 0.95f) || (MathUtils.Abs(x4) < 12f && num43 < 0.9f);
                                int num46 = TerrainChunk.CalculateCellIndex(x3, 0, z3);
                                for (int num47 = 0; num47 < 8; num47++)
                                {
                                    int num48 = num47 + num16 * 8;
                                    int value = 0;
                                    if (num40 < 0f)
                                    {
                                        if (num48 <= oceanLevel)
                                        {
                                            value = 18;
                                        }
                                    }
                                    else if (flag)
                                    {
                                        if (num40 < num44)
                                        {
                                            value = 4;
                                        }
                                        else if (num40 < num45)
                                        {
                                            value = 3;
                                        }
                                        else
                                        {
                                            value = 67;
                                        }
                                    }
                                    else if (num40 < num45)
                                    {
                                        value = 3;
                                    }
                                    else
                                    {
                                        value = 67;
                                    }
                                    chunk.SetCellValueFast(num46 + num48, value);
                                    num40 += num39;
                                }
                                num36 += num34;
                                num37 += num35;
                            }
                            num29 += num25;
                            num30 += num26;
                            num31 += num27;
                            num32 += num28;
                        }
                    }
                }
            }
        }
        /*
        private void GenerateCaves(TerrainChunk chunk)
        {
            List<TerrainContentsGenerator.CavePoint> list = new List<TerrainContentsGenerator.CavePoint>();
            int x = chunk.Coords.X;
            int y = chunk.Coords.Y;
            for (int i = x - 2; i <= x + 2; i++)
            {
                for (int j = y - 2; j <= y + 2; j++)
                {
                    list.Clear();
                    Game.Random random = new Game.Random(this.worldSeed + i + 9973 * j);
                    int num = i * 16 + random.UniformInt(0, 15);
                    int num2 = j * 16 + random.UniformInt(0, 15);
                    float probability = 0.5f;
                    if (random.Bool(probability))
                    {
                        int num3 = (int)this.CalculateHeight((float)num, (float)num2);
                        int num4 = (int)this.CalculateHeight((float)(num + 3), (float)num2);
                        int num5 = (int)this.CalculateHeight((float)num, (float)(num2 + 3));
                        Vector3 position = new Vector3((float)num, (float)(num3 - 1), (float)num2);
                        Vector3 v = new Vector3(3f, (float)(num4 - num3), 0f);
                        Vector3 v2 = new Vector3(0f, (float)(num5 - num3), 3f);
                        Vector3 vector = Vector3.Normalize(Vector3.Cross(v, v2));
                        if (vector.Y > -0.6f)
                        {
                            list.Add(new TerrainContentsGenerator.CavePoint
                            {
                                Position = position,
                                Direction = vector,
                                BrushType = 0,
                                Length = random.UniformInt(20, 40)
                            });
                        }
                        int num6 = i * 16 + 8;
                        int num7 = j * 16 + 8;
                        int k = 0;
                        while (k < list.Count)
                        {
                            TerrainContentsGenerator.CavePoint cavePoint = list[k];
                            List<TerrainBrush> list2 = TerrainContentsGenerator.m_caveBrushesByType[cavePoint.BrushType];
                            list2[random.UniformInt(0, list2.Count - 1)].PaintFastAvoidWater(chunk, Terrain.ToCell(cavePoint.Position.X), Terrain.ToCell(cavePoint.Position.Y), Terrain.ToCell(cavePoint.Position.Z));
                            cavePoint.Position += 2f * cavePoint.Direction;
                            cavePoint.StepsTaken += 2;
                            float num8 = cavePoint.Position.X - (float)num6;
                            float num9 = cavePoint.Position.Z - (float)num7;
                            if (random.Bool(0.5f))
                            {
                                Vector3 vector2 = Vector3.Normalize(random.Vector3(1f, true));
                                if ((num8 < -25.5f && vector2.X < 0f) || (num8 > 25.5f && vector2.X > 0f))
                                {
                                    vector2.X = -vector2.X;
                                }
                                if ((num9 < -25.5f && vector2.Z < 0f) || (num9 > 25.5f && vector2.Z > 0f))
                                {
                                    vector2.Z = -vector2.Z;
                                }
                                if ((cavePoint.Direction.Y < -0.5f && vector2.Y < -10f) || (cavePoint.Direction.Y > 0.1f && vector2.Y > 0f))
                                {
                                    vector2.Y = -vector2.Y;
                                }
                                cavePoint.Direction = Vector3.Normalize(cavePoint.Direction + 0.5f * vector2);
                            }
                            if (cavePoint.StepsTaken > 20 && random.Bool(0.06f))
                            {
                                cavePoint.Direction = Vector3.Normalize(random.Vector3(1f, true) * new Vector3(1f, 0.33f, 1f));
                            }
                            if (cavePoint.StepsTaken > 20 && random.Bool(0.05f))
                            {
                                cavePoint.Direction.Y = 0f;
                                cavePoint.BrushType = MathUtils.Min(cavePoint.BrushType + 2, TerrainContentsGenerator.m_caveBrushesByType.Count - 1);
                            }
                            if (cavePoint.StepsTaken > 30 && random.Bool(0.03f))
                            {
                                cavePoint.Direction.X = 0f;
                                cavePoint.Direction.Y = -1f;
                                cavePoint.Direction.Z = 0f;
                            }
                            if (cavePoint.StepsTaken > 30 && cavePoint.Position.Y < 30f && random.Bool(0.02f))
                            {
                                cavePoint.Direction.X = 0f;
                                cavePoint.Direction.Y = 1f;
                                cavePoint.Direction.Z = 0f;
                            }
                            if (random.Bool(0.33f))
                            {
                                cavePoint.BrushType = (int)(MathUtils.Pow(random.UniformFloat(0f, 0.999f), 7f) * (float)TerrainContentsGenerator.m_caveBrushesByType.Count);
                            }
                            if (random.Bool(0.06f) && list.Count < 12 && cavePoint.StepsTaken > 20 && cavePoint.Position.Y < 58f)
                            {
                                list.Add(new TerrainContentsGenerator.CavePoint
                                {
                                    Position = cavePoint.Position,
                                    Direction = Vector3.Normalize(random.UniformVector3(1f, 1f, false) * new Vector3(1f, 0.33f, 1f)),
                                    BrushType = (int)(MathUtils.Pow(random.UniformFloat(0f, 0.999f), 7f) * (float)TerrainContentsGenerator.m_caveBrushesByType.Count),
                                    Length = random.UniformInt(20, 40)
                                });
                            }
                            if (cavePoint.StepsTaken >= cavePoint.Length || MathUtils.Abs(num8) > 34f || MathUtils.Abs(num9) > 34f || cavePoint.Position.Y < 5f || cavePoint.Position.Y > 118f)
                            {
                                k++;
                            }
                            else if (cavePoint.StepsTaken % 20 == 0)
                            {
                                float num10 = this.CalculateHeight(cavePoint.Position.X, cavePoint.Position.Z);
                                if (cavePoint.Position.Y > num10 + 1f)
                                {
                                    k++;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void GeneratePockets(TerrainChunk chunk)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int num = i + chunk.Coords.X;
                    int num2 = j + chunk.Coords.Y;
                    Game.Random random = new Game.Random(this.worldSeed + num + 71 * num2);
                    int num3 = random.UniformInt(0, 10);
                    for (int k = 0; k < num3; k++)
                    {
                        random.UniformInt(0, 1);
                    }
                    float num4 = this.CalculateMountainRangeFactor((float)(num * 16), (float)(num2 * 16));
                    for (int l = 0; l < 3; l++)
                    {
                        int x = num * 16 + random.UniformInt(0, 15);
                        int y = random.UniformInt(50, 100);
                        int z = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_dirtPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_dirtPocketBrushes.Count - 1)].PaintFastSelective(chunk, x, y, z, 3);
                    }
                    for (int m = 0; m < 10; m++)
                    {
                        int x2 = num * 16 + random.UniformInt(0, 15);
                        int y2 = random.UniformInt(20, 80);
                        int z2 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_gravelPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_gravelPocketBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z2, 3);
                    }
                    for (int n = 0; n < 2; n++)
                    {
                        int x3 = num * 16 + random.UniformInt(0, 15);
                        int y3 = random.UniformInt(20, 120);
                        int z3 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_limestonePocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_limestonePocketBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z3, 3);
                    }
                    for (int num5 = 0; num5 < 1; num5++)
                    {
                        int x4 = num * 16 + random.UniformInt(0, 15);
                        int y4 = random.UniformInt(50, 70);
                        int z4 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_clayPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_clayPocketBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z4, 3);
                    }
                    for (int num6 = 0; num6 < 6; num6++)
                    {
                        int x5 = num * 16 + random.UniformInt(0, 15);
                        int y5 = random.UniformInt(40, 80);
                        int z5 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_sandPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_sandPocketBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z5, 4);
                    }
                    for (int num7 = 0; num7 < 4; num7++)
                    {
                        int x6 = num * 16 + random.UniformInt(0, 15);
                        int y6 = random.UniformInt(40, 60);
                        int z6 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_basaltPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z6, 4);
                    }
                    for (int num8 = 0; num8 < 3; num8++)
                    {
                        int x7 = num * 16 + random.UniformInt(0, 15);
                        int y7 = random.UniformInt(20, 40);
                        int z7 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_basaltPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_basaltPocketBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z7, 3);
                    }
                    for (int num9 = 0; num9 < 6; num9++)
                    {
                        int x8 = num * 16 + random.UniformInt(0, 15);
                        int y8 = random.UniformInt(4, 50);
                        int z8 = num2 * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_granitePocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_granitePocketBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z8, 67);
                    }
                    if (random.Bool(0.02f + 0.01f * num4))
                    {
                        int num10 = num * 16;
                        int num11 = random.UniformInt(40, 60);
                        int num12 = num2 * 16;
                        int num13 = random.UniformInt(1, 3);
                        for (int num14 = 0; num14 < num13; num14++)
                        {
                            Vector2 vector = random.Vector2(7f, false);
                            int num15 = 8 + (int)MathUtils.Round(vector.X);
                            int num16 = 0;
                            int num17 = 8 + (int)MathUtils.Round(vector.Y);
                            TerrainContentsGenerator.m_waterPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_waterPocketBrushes.Count - 1)].PaintFast(chunk, num10 + num15, num11 + num16, num12 + num17);
                        }
                    }
                    if (random.Bool(0.06f + 0.05f * num4))
                    {
                        int num18 = num * 16;
                        int num19 = random.UniformInt(15, 42);
                        int num20 = num2 * 16;
                        int num21 = random.UniformInt(1, 2);
                        for (int num22 = 0; num22 < num21; num22++)
                        {
                            Vector2 vector2 = random.Vector2(7f, false);
                            int num23 = 8 + (int)MathUtils.Round(vector2.X);
                            int num24 = random.UniformInt(0, 1);
                            int num25 = 8 + (int)MathUtils.Round(vector2.Y);
                            TerrainContentsGenerator.m_magmaPocketBrushes[random.UniformInt(0, TerrainContentsGenerator.m_magmaPocketBrushes.Count - 1)].PaintFast(chunk, num18 + num23, num19 + num24, num20 + num25);
                        }
                    }
                }
            }
        }
        private void GenerateMinerals(TerrainChunk chunk)
        {
            int x = chunk.Coords.X;
            int y = chunk.Coords.Y;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    Game.Random random = new Game.Random(this.worldSeed + i + 119 * j);
                    int num = random.UniformInt(0, 10);
                    for (int k = 0; k < num; k++)
                    {
                        random.UniformInt(0, 1);
                    }
                    float num2 = this.CalculateMountainRangeFactor((float)(i * 16), (float)(j * 16));
                    int num3 = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)i, (float)j, 0.33f, 1, 1f, 1f));
                    for (int l = 0; l < num3; l++)
                    {
                        int x2 = i * 16 + random.UniformInt(0, 15);
                        int y2 = random.UniformInt(5, 80);
                        int z = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_coalBrushes[random.UniformInt(0, TerrainContentsGenerator.m_coalBrushes.Count - 1)].PaintFastSelective(chunk, x2, y2, z, 3);
                    }
                    int num4 = (int)(6f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 1211), (float)(j + 396), 0.33f, 1, 1f, 1f));
                    for (int m = 0; m < num4; m++)
                    {
                        int x3 = i * 16 + random.UniformInt(0, 15);
                        int y3 = random.UniformInt(20, 65);
                        int z2 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_copperBrushes[random.UniformInt(0, TerrainContentsGenerator.m_copperBrushes.Count - 1)].PaintFastSelective(chunk, x3, y3, z2, 3);
                    }
                    int num5 = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 713), (float)(j + 211), 0.33f, 1, 1f, 1f));
                    for (int n = 0; n < num5; n++)
                    {
                        int x4 = i * 16 + random.UniformInt(0, 15);
                        int y4 = random.UniformInt(2, 40);
                        int z3 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_ironBrushes[random.UniformInt(0, TerrainContentsGenerator.m_ironBrushes.Count - 1)].PaintFastSelective(chunk, x4, y4, z3, 67);
                    }
                    int num6 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 915), (float)(j + 272), 0.33f, 1, 1f, 1f));
                    for (int num7 = 0; num7 < num6; num7++)
                    {
                        int x5 = i * 16 + random.UniformInt(0, 15);
                        int y5 = random.UniformInt(50, 70);
                        int z4 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_saltpeterBrushes[random.UniformInt(0, TerrainContentsGenerator.m_saltpeterBrushes.Count - 1)].PaintFastSelective(chunk, x5, y5, z4, 4);
                    }
                    int num8 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 711), (float)(j + 1194), 0.33f, 1, 1f, 1f));
                    for (int num9 = 0; num9 < num8; num9++)
                    {
                        int x6 = i * 16 + random.UniformInt(0, 15);
                        int y6 = random.UniformInt(2, 40);
                        int z5 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_sulphurBrushes[random.UniformInt(0, TerrainContentsGenerator.m_sulphurBrushes.Count - 1)].PaintFastSelective(chunk, x6, y6, z5, 67);
                    }
                    int num10 = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 432), (float)(j + 907), 0.33f, 1, 1f, 1f));
                    for (int num11 = 0; num11 < num10; num11++)
                    {
                        int x7 = i * 16 + random.UniformInt(0, 15);
                        int y7 = random.UniformInt(2, 15);
                        int z6 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_diamondBrushes[random.UniformInt(0, TerrainContentsGenerator.m_diamondBrushes.Count - 1)].PaintFastSelective(chunk, x7, y7, z6, 67);
                    }
                    int num12 = (int)(3f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 799), (float)(j + 131), 0.33f, 1, 1f, 1f));
                    for (int num13 = 0; num13 < num12; num13++)
                    {
                        int x8 = i * 16 + random.UniformInt(0, 15);
                        int y8 = random.UniformInt(2, 50);
                        int z7 = j * 16 + random.UniformInt(0, 15);
                        TerrainContentsGenerator.m_germaniumBrushes[random.UniformInt(0, TerrainContentsGenerator.m_germaniumBrushes.Count - 1)].PaintFastSelective(chunk, x8, y8, z7, 67);
                    }
                }
            }
        }
        private void GenerateSurface(TerrainChunk chunk)
        {
            Terrain terrain = this.subsystemTerrain.Terrain;
            Game.Random random = new Game.Random(this.worldSeed + chunk.Coords.X + 101 * chunk.Coords.Y);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    int num = i + chunk.Origin.X;
                    int num2 = j + chunk.Origin.Y;
                    int num3 = TerrainChunk.CalculateCellIndex(i, 126, j);
                    int k = 126;
                    while (k >= 0)
                    {
                        int num4 = Terrain.ExtractContents(chunk.GetCellValueFast(num3));
                        if (!BlocksManager.Blocks[num4].IsTransparent)
                        {
                            float num5 = this.CalculateMountainRangeFactor((float)num, (float)num2);
                            int temperature = terrain.GetTemperature(num, num2);
                            int humidity = terrain.GetHumidity(num, num2);
                            float f = MathUtils.Saturate(MathUtils.Saturate((num5 - 0.9f) / 0.1f) - MathUtils.Saturate(((float)humidity - 3f) / 12f) + TerrainContentsGenerator.TGSurfaceMultiplier * MathUtils.Saturate(((float)k - 85f) * 0.05f));
                            int min = (int)MathUtils.Lerp(4f, 0f, f);
                            int max = (int)MathUtils.Lerp(7f, 0f, f);
                            int num6 = MathUtils.Min(random.UniformInt(min, max), k);
                            int contents;
                            if (num4 == 4)
                            {
                                contents = ((temperature > 4 && temperature < 7) ? 6 : 7);
                            }
                            else
                            {
                                int num7 = temperature / 4;
                                int num8 = (k + 1 < 255) ? chunk.GetCellContentsFast(i, k + 1, j) : 0;
                                if ((k < 66 || k == 84 + num7 || k == 103 + num7) && humidity == 9 && temperature % 6 == 1)
                                {
                                    contents = 66;
                                }
                                else if (num8 == 18 && humidity > 8 && humidity % 2 == 0 && temperature % 3 == 0)
                                {
                                    contents = 72;
                                }
                                else
                                {
                                    contents = 2;
                                }
                            }
                            int num9 = TerrainChunk.CalculateCellIndex(i, k + 1, j);
                            for (int l = num9 - num6; l < num9; l++)
                            {
                                if (Terrain.ExtractContents(chunk.GetCellValueFast(l)) != 0)
                                {
                                    int value = Terrain.ReplaceContents(0, contents);
                                    chunk.SetCellValueFast(l, value);
                                }
                            }
                            break;
                        }
                        k--;
                        num3--;
                    }
                }
            }
        }
        private void PropagateFluidsDownwards(TerrainChunk chunk)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    int num = TerrainChunk.CalculateCellIndex(i, 255, j);
                    int num2 = 0;
                    int k = 255;
                    while (k >= 0)
                    {
                        int num3 = Terrain.ExtractContents(chunk.GetCellValueFast(num));
                        if (num3 == 0 && num2 != 0 && BlocksManager.FluidBlocks[num2] != null)
                        {
                            chunk.SetCellValueFast(num, num2);
                            num3 = num2;
                        }
                        num2 = num3;
                        k--;
                        num--;
                    }
                }
            }
        }
        */

    }
}
