using System;
using Engine;
using System.Collections.Generic;
using Game;
using GameEntitySystem;
using Engine.Graphics;
using System.Threading.Tasks;
using System.Threading;

namespace CreatorModAPI
{
    public class CreatorAPI
    {
        public enum NumberPoint
        {
            One,
            Two,
            Three,
            Four
        }
        public enum OnekeyType
        {
            Tree,
            Build
        }
        public bool oldMainWidget = false;
        /// <summary>
        /// 空气识别
        /// </summary>
        public bool AirIdentify = false;
        /// <summary>
        /// 清理方块
        /// </summary>
        public bool ClearBlock = false;
        /// <summary>
        /// 超视距生成
        /// </summary>
        public bool UnLimitedOfCreate = false;
        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool RevokeSwitch = false;
        /// <summary>
        /// 撤回缓存
        /// </summary>
        public ChunkData revokeData = null;
        public List<Point3> Position { get; set; }
        /// <summary>
        /// 可设置多少个点
        /// </summary>
        public NumberPoint amountPoint = NumberPoint.Two;
        /// <summary>
        /// 设置到第几个点
        /// </summary>
        public NumberPoint numberPoint = NumberPoint.One;
        /// <summary>
        /// 选择生成方块处理方式
        /// </summary>
        public CreateBlockType CreateBlockType = CreateBlockType.Fast;
/*        /// <summary>
        /// 两点连线开关
        /// </summary>
        public bool twoPointsOfAttachment = false;*/
        /// <summary>
        /// 一键生成开关
        /// </summary>
        public bool oneKeyGeneration = false;
        /// <summary>
        /// 一键生成类型
        /// </summary>
        public OnekeyType onekeyType = OnekeyType.Build;
        /// <summary>
        /// 生成开关
        /// </summary>
        public bool launch = true;
        /// <summary>
        /// 粘贴旋转设置
        /// </summary>
        public bool pasteRotate = false;
        /// <summary>
        /// 粘贴限制设置
        /// </summary>
        public bool pasteLimit = false;
        /// <summary>
        /// 获取玩家
        /// </summary>
        public ComponentMiner componentMiner;
        public PrimitivesRenderer3D primitivesRenderer3D;
        public CreatorGenerationAlgorithm creatorGenerationAlgorithm;
        public CreatorAPI(ComponentMiner componentMiner)
        {
            creatorGenerationAlgorithm = new CreatorGenerationAlgorithm();
            this.componentMiner = componentMiner;
            this.Position = new List<Point3>(4)
            {
                new Point3(0,-1,0),
                new Point3(0,-1,0),
                new Point3(0,-1,0),
                new Point3(0,-1,0)
            };
        }
        public void OnUse(TerrainRaycastResult terrainRaycastResult)
        {
            Point3 position = terrainRaycastResult.CellFace.Point;
            ComponentPlayer Player = this.componentMiner.ComponentPlayer;
            if (!OnTouch.Touch(this, position))
            {
                return;
            }
            int cellValue = GameManager.Project.FindSubsystem<SubsystemTerrain>(true).Terrain.GetCellValue(position.X, position.Y, position.Z);
            int light = Terrain.ExtractLight(cellValue);
            int data = Terrain.ExtractData(cellValue);
            int contents = Terrain.ExtractContents(cellValue);
            if (BlocksManager.Blocks[contents] != null)
            {
                if (numberPoint == NumberPoint.One)
                {
                    this.Position[0] = position;
                    Player.ComponentGui.DisplaySmallMessage($"坐标1设置完成\n X:{position.X} ,Y:{position.Y} ,Z:{position.Z}\n方块ID : {contents}\n方块完整值 : {cellValue}\n特殊值一 : {light} , 特殊值二 : {data} \n", true, false);
                    if (amountPoint != numberPoint) numberPoint = NumberPoint.Two;
                }
                else if (numberPoint == NumberPoint.Two)
                {
                    this.Position[1] = position;
                    Player.ComponentGui.DisplaySmallMessage($"坐标2设置完成\n X:{position.X} ,Y:{position.Y} ,Z:{position.Z}\n方块ID : {contents}\n方块完整值 : {cellValue}\n特殊值一 : {light} , 特殊值二 : {data}\n", true, false);
                    if (amountPoint == numberPoint) numberPoint = NumberPoint.One; else numberPoint = NumberPoint.Three;
                }
                else if (numberPoint == NumberPoint.Three)
                {
                    this.Position[2] = position;
                    Player.ComponentGui.DisplaySmallMessage($"坐标3设置完成\n X:{position.X} ,Y:{position.Y} ,Z:{position.Z}\n方块ID : {contents}\n方块完整值 : {cellValue}\n特殊值一 : {light} , 特殊值二 : {data}\n", true, false);
                    if (amountPoint == numberPoint) numberPoint = NumberPoint.One; else numberPoint = NumberPoint.Four;
                }
                else if (numberPoint == NumberPoint.Four)
                {
                    this.Position[3] = position;
                    Player.ComponentGui.DisplaySmallMessage($"坐标4设置完成\n X:{position.X} ,Y:{position.Y} ,Z:{position.Z}\n方块ID : {contents}\n方块完整值 : {cellValue}\n特殊值一 : {light} , 特殊值二 : {data}\n", true, false);
                    numberPoint = NumberPoint.One;
                }
            }
        }
        public void CreateBlock(int x, int y, int z, int value,ChunkData chunkData = null)
        {
            if (this.RevokeSwitch && this.revokeData != null && this.revokeData.GetChunk(x, y) == null) this.revokeData.CreateChunk(x, y,true);
            switch (this.CreateBlockType)
            {
                case CreateBlockType.Fast:
                    SetBlock(x, y, z, value);
                    break;
                case CreateBlockType.Normal:
                    GameManager.Project.FindSubsystem<SubsystemTerrain>(true).ChangeCell(x, y, z, value);
                    break;
                case CreateBlockType.Catch:
                    chunkData.SetBlock(x,y,z,value);
                    break;
            }
        }
        public void CreateBlock(Point3 point3, int value,ChunkData chunkData = null)
        {
            if (this.RevokeSwitch && this.revokeData != null  && this.revokeData.GetChunk(point3.X, point3.Z) == null) this.revokeData.CreateChunk(point3.X,point3.Z,true);
            switch (this.CreateBlockType)
            {
                case CreateBlockType.Fast:
                    SetBlock(point3.X, point3.Y, point3.Z, value);
                    break;
                case CreateBlockType.Normal:
                    GameManager.Project.FindSubsystem<SubsystemTerrain>(true).ChangeCell(point3.X, point3.Y, point3.Z, value);
                    break;
                case CreateBlockType.Catch:
                    chunkData.SetBlock(point3,value);
                    break;
            }
        }
        public void SetBlock(int x, int y, int z, int value)
        {
            try
            {
                SubsystemTerrain subsystemTerrain = this.componentMiner.Project.FindSubsystem<SubsystemTerrain>(true);
                if (subsystemTerrain.Terrain.IsCellValid(x, y, z))
                {
                    TerrainChunk chunkAtCell = subsystemTerrain.Terrain.GetChunkAtCell(x, z);
                    if (chunkAtCell == null)
                    {
                        if (!this.UnLimitedOfCreate) return;
                        chunkAtCell = subsystemTerrain.Terrain.AllocateChunk(x >> 4, z >> 4);
                        while (chunkAtCell.ThreadState < TerrainChunkState.Valid)
                            subsystemTerrain.TerrainUpdater.UpdateChunkSingleStep(chunkAtCell, 15);
                    }
                    chunkAtCell.Cells[y + (x & 15) * 128 + (z & 15) * 128 * 16] = value;
                    chunkAtCell.ModificationCounter++;
                    if (this.UnLimitedOfCreate)
                    {
                        chunkAtCell.State = TerrainChunkState.Valid;
                    }
                    if (chunkAtCell.State > TerrainChunkState.InvalidLight)
                    {
                        chunkAtCell.State = TerrainChunkState.InvalidLight;
                    }
                    chunkAtCell.WasDowngraded = true;
                    //subsystemTerrain.m_modifiedCells[new Point3(x, y, z)] = true;
                }
            }
            catch(Exception e)
            {
                Log.Error("快速生成发生错误:"+e.Message);
            }
        }



    }
}