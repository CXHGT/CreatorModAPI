using Engine;
using Engine.Serialization;
using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CreatorModAPI
{
    public static class OnekeyGeneration
    {
        /// <summary>
        /// 创建一键生成文件
        /// </summary>
        /// <param name="creatorAPI"></param>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="position"></param>
        public static void CreateOnekey(CreatorAPI creatorAPI, string directory, string path, Point3 Start, Point3 End,Point3 position)
        {
            int count = 0;
            CreatorMain.Math.StartEnd(ref Start, ref End);
            FileStream fileStream = new FileStream(directory + "/" + path, FileMode.Create);
            EngineBinaryWriter binaryWriter = new EngineBinaryWriter(fileStream, true);
            binaryWriter.Write(End.X - position.X);
            binaryWriter.Write(End.Y - position.Y);
            binaryWriter.Write(End.Z - position.Z);
            binaryWriter.Write(Start.X - position.X);
            binaryWriter.Write(Start.Y - position.Y);
            binaryWriter.Write(Start.Z - position.Z);
            for (int x = End.X; x <= Start.X; x++)
            {
                for (int y = End.Y; y <= Start.Y; y++)
                {
                    for (int z = End.Z; z <= Start.Z; z++)
                    {
                        binaryWriter.Write(GameManager.Project.FindSubsystem<SubsystemTerrain>(true).Terrain.GetCellValueFast(x, y, z));
                        count++;
                    }
                }
            }
            binaryWriter.Dispose();
            fileStream.Dispose();
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"创建一键生成成功，共{count}个方块", true, true);
        }
        /// <summary>
        /// 一键生成
        /// </summary>
        /// <param name="creatorAPI"></param>
        /// <param name="path"></param>
        /// <param name="position"></param>
        public static void GenerationData(CreatorAPI creatorAPI, string path, Point3 position)
        {
            ChunkData chunkData = new ChunkData(creatorAPI);
            creatorAPI.revokeData = new ChunkData(creatorAPI);
            ComponentPlayer player = creatorAPI.componentMiner.ComponentPlayer;
            Stream fileStream = new FileStream(path, FileMode.Open).CreateStream();
            EngineBinaryReader binaryReader = new EngineBinaryReader(fileStream, false);
            int count = 0;
            int MinX = binaryReader.ReadInt32();
            int MinY = binaryReader.ReadInt32();
            int MInZ = binaryReader.ReadInt32();
            int MaxX = binaryReader.ReadInt32();
            int MaxY = binaryReader.ReadInt32();
            int MaxZ = binaryReader.ReadInt32();
            for (int PositionX = MinX; PositionX <= MaxX; PositionX++)
            {
                for (int PositionY = MinY; PositionY <= MaxY; PositionY++)
                {
                    for (int PositionZ = MInZ; PositionZ <= MaxZ; PositionZ++)
                    {
                        if (!creatorAPI.launch) return;
                        int id = binaryReader.ReadInt32();
                        if (!creatorAPI.AirIdentify && Terrain.ExtractContents(id) == 0) continue;
                        creatorAPI.CreateBlock(position.X + PositionX, position.Y + PositionY, position.Z + PositionZ, id,chunkData);
                        count++;
                    }
                }
            }
            binaryReader.Dispose();
            fileStream.Dispose();
            chunkData.Render();
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"操作成功，共{count}个方块", true, true);
        }
        /// <summary>
        /// 导出成普通文本文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exportFile"></param>
        public static void ExportOnekeyoMod2(string path, string exportFile)
        {
            if (path.IsFileInUse()) throw new FileLoadException("文件被占用");
            Stream exportFileStream = new FileStream(exportFile, FileMode.Create);
            Stream stream = File.Open(path, FileMode.Open);
            EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream, false);
            string data = "";
            int MinX = engineBinaryReader.ReadInt32();
            int MinY = engineBinaryReader.ReadInt32();
            int MinZ = engineBinaryReader.ReadInt32();
            int MaxX = engineBinaryReader.ReadInt32();
            int MaxY = engineBinaryReader.ReadInt32();
            int MaxZ = engineBinaryReader.ReadInt32();
            data += $"{MinX}\n{MinY}\n{MinZ}\n{MaxX}\n{MaxY}\n{MaxZ}";
            for (int x = MinX; x <= MaxX; x++)
                for (int y = MinY; y <= MaxY; y++)
                    for (int z = MinZ; z <= MaxZ; z++)
                        data += $"\n{engineBinaryReader.ReadInt32()}";
            exportFileStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            engineBinaryReader.Dispose();
            stream.Dispose();
            exportFileStream.Dispose();
        }
        /// <summary>
        /// 普通文本文件导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="importFile"></param>
        public static void ImportOnekeyoMod2(string path, string importFile)
        {
            List<string> list = new List<string>();
            Stream stream = File.OpenRead(importFile);
            StreamReader streamReader = new StreamReader(stream);
            string text = streamReader.ReadToEnd();
            streamReader.Dispose();
            stream.Dispose();
            FileStream fileStream = new FileStream(path, FileMode.Create);
            EngineBinaryWriter binaryWriter = new EngineBinaryWriter(fileStream, true);
            foreach (string data in text.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                binaryWriter.Write(int.Parse(data));
            }
            binaryWriter.Dispose();
            fileStream.Dispose();
        }

        /// <summary>
        /// 导出成普通文本文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exportFile"></param>
        public static void ExportOnekeyoMod(string path, string exportFile)
        {
            if (path.IsFileInUse()) throw new FileLoadException("文件被占用");
            Stream exportFileStream = new FileStream(exportFile, FileMode.Create);
            Stream stream = File.Open(path, FileMode.Open);
            EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream, false);
            string data = "";
            int MinX = engineBinaryReader.ReadInt32();
            int MinY = engineBinaryReader.ReadInt32();
            int MinZ = engineBinaryReader.ReadInt32();
            int MaxX = engineBinaryReader.ReadInt32();
            int MaxY = engineBinaryReader.ReadInt32();
            int MaxZ = engineBinaryReader.ReadInt32();
            //data += $"{MinX}\n{MinY}\n{MinZ}\n{MaxX}\n{MaxY}\n{MaxZ}";
            for (int x = MinX; x <= MaxX; x++)
            {
                for (int y = MinY; y <= MaxY; y++)
                {
                    for (int z = MinZ; z <= MaxZ; z++)
                    {
                        int blockID = engineBinaryReader.ReadInt32();
                        if (Terrain.ExtractContents(blockID) == 0) continue;
                        if (x == MaxX && y == MaxY && z == MaxZ)
                        {
                            data += $"{x},{y},{z},{blockID}\n";
                            continue;
                        }
                        data += $"{x},{y},{z},{blockID}\n";
                    }
                }
            }
            exportFileStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            engineBinaryReader.Dispose();
            stream.Dispose();
            exportFileStream.Dispose();
        }


    }
}
