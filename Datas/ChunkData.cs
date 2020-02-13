using System;
using System.Collections.Generic;
using Game;
using Engine;

namespace CreatorModAPI
{
    public class ChunkData
    {
        public class Chunk
        {
            public int chunkX;
            public int chunkY;
            public int[] Cells;
            public Chunk(int chunkX,int chunkY)
            {
                this.chunkX = chunkX;
                this.chunkY = chunkY;
            }
        }
        public List<Chunk> chunksData = new List<Chunk>();
        private SubsystemTerrain subsystemTerrain;
        private CreatorAPI creatorAPI;
        public ChunkData(CreatorAPI creatorAPI)
        {
            this.creatorAPI = creatorAPI;
            this.subsystemTerrain = creatorAPI.componentMiner.Project.FindSubsystem<SubsystemTerrain>(true);
        }
        public Chunk CreateChunk(int x,int z,bool unLimited = false)
        {
            int chunkX = x >> 4;
            int chunkY = z >> 4;
            TerrainChunk chunkAtCell = subsystemTerrain.Terrain.GetChunkAtCoords(chunkX, chunkY);
            if (chunkAtCell == null)
            {
                if (!unLimited) return null;
                chunkAtCell = subsystemTerrain.Terrain.AllocateChunk(chunkX, chunkY);
                while (chunkAtCell.ThreadState < TerrainChunkState.Valid)
                    subsystemTerrain.TerrainUpdater.UpdateChunkSingleStep(chunkAtCell, 15);
            }
            Chunk chunk1 = new Chunk(chunkX, chunkY);
            chunk1.Cells = new int[chunkAtCell.Cells.Length];
            for(int i = 0;i < chunkAtCell.Cells.Length; i++)
            {
                chunk1.Cells[i] = chunkAtCell.Cells[i];
            }
            //Array.Copy(chunkAtCell.Cells,chunk1.Cells,chunkAtCell.Cells.Length);
            //chunk1.Cells = chunkAtCell.Cells;
            this.chunksData.Add(chunk1);
            return chunk1;
        }

        public int GetCellValueFast(int x, int y, int z)
        {
            int chunkX = x >> 4;
            int chunkY = z >> 4;
            Chunk chunk1 = null;
            foreach (Chunk chunk in chunksData)
            {
                if (chunk.chunkX == chunkX && chunk.chunkY == chunkY)
                {
                    chunk1 = chunk;
                    break;
                }
            }
            if (chunk1 != null)
                return chunk1.Cells[y + (x & 15) * 128 + (z & 15) * 128 * 16];
            else
                return 0;
        }
        public Chunk GetChunk(int x,int z)
        {
            int chunkX = x >> 4;
            int chunkY = z >> 4;
            foreach(Chunk chunk in chunksData)
            {
                if(chunk.chunkX == chunkX && chunk.chunkY == chunkY)
                {
                    return chunk;
                }
            }
            return null;
        }
        public virtual void SetBlock(int x, int y, int z, int value)
        {
            try
            {
                int num = y + (x & 15) * 128 + (z & 15) * 128 * 16;
                Chunk chunk = GetChunk(x, z);
                if (chunk == null)
                {
                    chunk = CreateChunk(x, z,this.creatorAPI.UnLimitedOfCreate);
                    if (chunk == null) return;
                }
                if (num >= chunk.Cells.Length) return;
                chunk.Cells[num] = value;
            }
            catch(Exception e)
            {
                Log.Error("缓存生成发生错误:"+e.Message);
            }
        }
        public virtual void SetBlock(Point3 point3,int value)
        {
            SetBlock(point3.X,point3.Y,point3.Z,value);
        }
        public virtual void Render()
        {
            foreach (Chunk chunk in chunksData)
            {
                TerrainChunk chunkAtCoords = subsystemTerrain.Terrain.GetChunkAtCoords(chunk.chunkX, chunk.chunkY);
                if (chunkAtCoords == null)
                {
                    if (!this.creatorAPI.UnLimitedOfCreate) continue;
                    chunkAtCoords = subsystemTerrain.Terrain.AllocateChunk(chunk.chunkX, chunk.chunkY);
                }
                for (int i = 0; i < chunkAtCoords.Cells.Length; i++)
                {
                    chunkAtCoords.Cells[i] = chunk.Cells[i];
                }
                chunkAtCoords.ModificationCounter++;
                if (this.creatorAPI.UnLimitedOfCreate)
                {
                    chunkAtCoords.State = TerrainChunkState.Valid;
                }
                if (chunkAtCoords.State > TerrainChunkState.InvalidLight)
                {
                    chunkAtCoords.State = TerrainChunkState.InvalidLight;
                }
                chunkAtCoords.WasDowngraded = true;
            }
            if(this != this.creatorAPI.revokeData) this.chunksData.Clear();
        }
        public void Clear()
        {
            this.chunksData.Clear();
        }
    }
}