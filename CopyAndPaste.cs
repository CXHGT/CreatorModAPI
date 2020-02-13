using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Engine;
using Engine.Serialization;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace CreatorModAPI
{
    public static class CopyAndPaste
    {
        /// <summary>
        /// 创建复制文件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public static void CreateCopy(CreatorAPI creatorAPI, string directory, string path, Point3 Start, Point3 End)
        {
            int count = 0;
            CreatorMain.Math.StartEnd(ref Start, ref End);
            FileStream fileStream = new FileStream(directory + "/" + path, FileMode.Create);
            EngineBinaryWriter binaryWriter = new EngineBinaryWriter(fileStream, true);
            binaryWriter.Write(Start.X - End.X);
            binaryWriter.Write(Start.Y - End.Y);
            binaryWriter.Write(Start.Z - End.Z);
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
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"复制成功，共{count}个方块", true, true);
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="creatorAPI"></param>
        /// <param name="path"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="type"></param>
        public static void PasetData(CreatorAPI creatorAPI, string path, Point3 Start, Point3 End)
        {
            ChunkData chunkData = new ChunkData(creatorAPI);
            creatorAPI.revokeData = new ChunkData(creatorAPI);
            ComponentPlayer player = creatorAPI.componentMiner.ComponentPlayer;
            Stream fileStream = new FileStream(path, FileMode.Open).CreateStream();
            EngineBinaryReader binaryReader = new EngineBinaryReader(fileStream, false);
            int count = 0;
            int MaxX = binaryReader.ReadInt32();
            int MaxY = binaryReader.ReadInt32();
            int MaxZ = binaryReader.ReadInt32();
            for (int PositionX = 0; PositionX <= MaxX; PositionX++)
            {
                for (int PositionY = 0; PositionY <= MaxY; PositionY++)
                {
                    for (int PositionZ = 0; PositionZ <= MaxZ; PositionZ++)
                    {
                        if (!creatorAPI.launch) return;
                        int x, y, z;
                        int id = binaryReader.ReadInt32();
                        if (!creatorAPI.AirIdentify && Terrain.ExtractContents(id) == 0) continue;
                        y = Start.Y + PositionY;
                        if (Start.X <= End.X && Start.Z <= End.Z)
                        {
                            x = Start.X + PositionX;
                            z = Start.Z + PositionZ;
                            if (creatorAPI.pasteLimit && (End.X < x || End.Z < z)) continue;
                            creatorAPI.CreateBlock(x, y, z, id,chunkData);
                        }
                        else if (Start.X >= End.X && Start.Z <= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X - MaxX + PositionX;
                                z = Start.Z + PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > x + Start.X - End.X || End.Z < z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X - PositionZ;
                                int _z = Start.Z + PositionX;
                                if (creatorAPI.pasteLimit && (Start.X > _x + Start.X - End.X || End.Z < _z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        else if (Start.X >= End.X && Start.Z >= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X - MaxX + PositionX;
                                z = Start.Z - MaxZ + PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > x + Start.X - End.X || Start.Z > z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X - PositionX;
                                int _z = Start.Z - PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > _x + Start.X - End.X || Start.Z > _z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        else if (Start.X <= End.X && Start.Z >= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X + PositionX;
                                z = Start.Z - MaxZ + PositionZ;
                                if (creatorAPI.pasteLimit && (End.X < x || Start.Z > z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X + PositionZ;
                                int _z = Start.Z - PositionX;
                                if (creatorAPI.pasteLimit && (End.X < _x || Start.Z > _z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        count++;
                    }
                }
            }
            binaryReader.Dispose();
            fileStream.Dispose();
            chunkData.Render();
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"粘贴成功，共{count}个方块", true, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="creatorAPI"></param>
        /// <param name="path"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public static void MirrorData(CreatorAPI creatorAPI, string path, Point3 Start, Point3 End)
        {
            ChunkData chunkData = new ChunkData(creatorAPI);
            creatorAPI.revokeData = new ChunkData(creatorAPI);
            ComponentPlayer player = creatorAPI.componentMiner.ComponentPlayer;
            Stream fileStream = new FileStream(path, FileMode.Open).CreateStream();
            EngineBinaryReader binaryReader = new EngineBinaryReader(fileStream, false);
            int count = 0;
            int MaxX = binaryReader.ReadInt32();
            int MaxY = binaryReader.ReadInt32();
            int MaxZ = binaryReader.ReadInt32();
            for (int PositionX = MaxX; PositionX >= 0; PositionX--)
            {
                for (int PositionY = 0; PositionY <= MaxY; PositionY++)
                {
                    for (int PositionZ = 0; PositionZ <= MaxZ; PositionZ++)
                    {
                        if (!creatorAPI.launch) return;
                        int x, y, z;
                        int id = binaryReader.ReadInt32();
                        if (!creatorAPI.AirIdentify && Terrain.ExtractContents(id) == 0) continue;
                        y = Start.Y + PositionY;
                        if (Start.X <= End.X && Start.Z <= End.Z)
                        {
                            x = Start.X + PositionX;
                            z = Start.Z + PositionZ;
                            if (creatorAPI.pasteLimit && (End.X < x || End.Z < z)) continue;
                            creatorAPI.CreateBlock(x, y, z, id,chunkData);
                        }
                        else if (Start.X >= End.X && Start.Z <= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X - MaxX + PositionX;
                                z = Start.Z + PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > x + Start.X - End.X || End.Z < z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X - PositionZ;
                                int _z = Start.Z + PositionX;
                                if (creatorAPI.pasteLimit && (Start.X > _x + Start.X - End.X || End.Z < _z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        else if (Start.X >= End.X && Start.Z >= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X - MaxX + PositionX;
                                z = Start.Z - MaxZ + PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > x + Start.X - End.X || Start.Z > z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X - PositionX;
                                int _z = Start.Z - PositionZ;
                                if (creatorAPI.pasteLimit && (Start.X > _x + Start.X - End.X || Start.Z > _z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        else if (Start.X <= End.X && Start.Z >= End.Z)
                        {
                            if (!creatorAPI.pasteRotate)
                            {
                                x = Start.X + PositionX;
                                z = Start.Z - MaxZ + PositionZ;
                                if (creatorAPI.pasteLimit && (End.X < x || Start.Z > z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(x, y, z, id,chunkData);
                            }
                            else
                            {
                                int _x = Start.X + PositionZ;
                                int _z = Start.Z - PositionX;
                                if (creatorAPI.pasteLimit && (End.X < _x || Start.Z > _z + Start.Z - End.Z)) continue;
                                creatorAPI.CreateBlock(_x, y, _z, id,chunkData);
                            }
                        }
                        count++;
                    }
                }
            }
            binaryReader.Dispose();
            fileStream.Dispose();
            chunkData.Render();
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"粘贴成功，共{count}个方块", true, true);
        }
        /// <summary>
        /// 导出成普通文本文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exportFile"></param>
        public static void ExportCopywMod2(string path, string exportFile)
        {
            if (path.IsFileInUse()) throw new FileLoadException("文件被占用");
            Stream exportFileStream = new FileStream(exportFile, FileMode.Create);
            Stream stream = File.Open(path, FileMode.Open);
            EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream, false);
            string data = "";
            int MaxX = engineBinaryReader.ReadInt32();
            int MaxY = engineBinaryReader.ReadInt32();
            int MaxZ = engineBinaryReader.ReadInt32();
            data += $"{MaxX}\n{MaxY}\n{MaxZ}";
            for (int x = 0; x <= MaxX; x++)
                for (int y = 0; y <= MaxY; y++)
                    for (int z = 0; z <= MaxZ; z++)
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
        public static void ImportCopywMod2(string path, string importFile)
        {
            List<string> list = new List<string>();
            Stream stream = File.OpenRead(importFile);
            StreamReader streamReader = new StreamReader(stream);
            string text = streamReader.ReadToEnd();
            streamReader.Dispose();
            stream.Dispose();
            FileStream fileStream = new FileStream(path, FileMode.Create);
            EngineBinaryWriter binaryWriter = new EngineBinaryWriter(fileStream, true);
            foreach (string data in text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                binaryWriter.Write(int.Parse(data));
            }
            binaryWriter.Dispose();
            fileStream.Dispose();
        }

        public static void ExportCopywMod(string path, string exportFile)
        {
            if (path.IsFileInUse()) throw new FileLoadException("文件被占用");
            Stream exportFileStream = new FileStream(exportFile,FileMode.Create);
            Stream stream = File.Open(path, FileMode.Open);
            EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream, false);
            string data = "";
            int MaxX = engineBinaryReader.ReadInt32();
            int MaxY = engineBinaryReader.ReadInt32();
            int MaxZ = engineBinaryReader.ReadInt32();
            data += $"{MaxX},{MaxY},{MaxZ}";
            for (int x = 0; x <= MaxX; x++)
            {
                for (int y = 0; y <= MaxY; y++)
                {
                    for (int z = 0; z <= MaxZ; z++)
                    {
                        int BlockID = engineBinaryReader.ReadInt32();
                        if (Terrain.ExtractContents(BlockID) != 0) data += $"\n{x},{y},{z},{BlockID}";
                    }
                }
            }
            exportFileStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            engineBinaryReader.Dispose();
            stream.Dispose();
            exportFileStream.Dispose();
        }

        public static void ImportCopywMod(string path, string importFile)
        {
            List<string> list = new List<string>();
            Stream stream = File.OpenRead(importFile);
            StreamReader streamReader = new StreamReader(stream);
            string text = streamReader.ReadToEnd();
            streamReader.Dispose();
            stream.Dispose();
            foreach (string data in text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                list.Add(data);
            }
            FileStream fileStream = new FileStream(path, FileMode.Create);
            EngineBinaryWriter binaryWriter = new EngineBinaryWriter(fileStream, true);
            string[] data2 = list[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int MaxX = int.Parse(data2[0]);
            int MaxY = int.Parse(data2[1]);
            int MaxZ = int.Parse(data2[2]);
            list.RemoveAt(0);
            binaryWriter.Write(MaxX);
            binaryWriter.Write(MaxY);
            binaryWriter.Write(MaxZ);
            for (int x = 0; x <= MaxX; x++)
            {
                for (int y = 0; y <= MaxY; y++)
                {
                    for (int z = 0; z <= MaxZ; z++)
                    {
                        int blockID = 0;
                        for (int i = 0; i < list.Count; i++)
                        {
                            string[] database = list[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (int.Parse(database[0]) == x && int.Parse(database[1]) == y && int.Parse(database[2]) == z)
                            {
                                blockID = int.Parse(database[3]);
                                list.RemoveAt(i);
                            }
                        }
                        binaryWriter.Write(blockID);
                    }
                }
            }
            binaryWriter.Dispose();
            fileStream.Dispose();
        }

        /// <summary>
        /// 创建特殊复制文件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public static void CreateSpecialCopy(CreatorAPI creatorAPI, string path, Point3 Start, Point3 End)
        {
            CreatorMain.Math.StartEnd(ref Start, ref End);
            FileStream fileStream = new FileStream(path, FileMode.Create);
            List<Entity> blockEntities = new List<Entity>();
            List<Entity> entities = new List<Entity>();
            string data = "";
            data += $"{Start.X - End.X},{Start.Y - End.Y},{Start.Z - End.Z}";
            for (int x = End.X; x <= Start.X; x++)
            {
                for (int y = End.Y; y <= Start.Y; y++)
                {
                    for (int z = End.Z; z <= Start.Z; z++)
                    {
                        if (GameManager.Project.FindSubsystem<SubsystemBlockEntities>().GetBlockEntity(x, y, z) != null)
                        {
                            blockEntities.Add(GameManager.Project.FindSubsystem<SubsystemBlockEntities>().GetBlockEntity(x, y, z).Entity);
                        }
                        data += "|" + GameManager.Project.FindSubsystem<SubsystemTerrain>(true).Terrain.GetCellValueFast(x, y, z);
                    }
                }
            }
            foreach (ComponentCreature current in GameManager.Project.FindSubsystem<SubsystemCreatureSpawn>(true).Creatures)
            {
                if (current.DisplayName != "Male Player" && current.DisplayName != "Female Player")
                {
                    Vector3 vector3 = current.ComponentBody.Position;
                    if (vector3.X <= Start.X && vector3.X >= End.X && vector3.Y <= Start.Y && vector3.Y >= End.Y && vector3.Z <= Start.Z && vector3.Z >= End.Z)
                    {
                        entities.Add(current.Entity);
                    }
                }
            }
            data += "\nBlockEntity";
            foreach (Entity entity in blockEntities)
            {
                ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
                if (componentBlockEntity != null)
                {
                    Point3 point3 = componentBlockEntity.Coordinates;
                    string typeName = "Chest";
                    if (entity.FindComponent<ComponentDispenser>() != null)
                    {
                        typeName = "Dispenser";
                    }
                    else if (entity.FindComponent<ComponentFurnace>() != null)
                    {
                        typeName = "Furnace";
                    }
                    else if (entity.FindComponent<ComponentCraftingTable>() != null)
                    {
                        typeName = "CraftingTable";
                    }
                    else
                    {
                        throw new Exception("检测到一个无法识别的方块实体，现在电路元件暂时还不能识别");
                    }
                    data += $"|{typeName}\t{point3.X - End.X},{ point3.Y - End.Y},{point3.Z - End.Z}";
                    ComponentInventoryBase blockEntityInventoryBase = entity.FindComponent<ComponentInventoryBase>();
                    if (blockEntityInventoryBase != null)
                    {
                        for (int i = 0; i < blockEntityInventoryBase.SlotsCount; i++)
                        {
                            int slotValue = blockEntityInventoryBase.GetSlotValue(i);
                            int slotCount = blockEntityInventoryBase.GetSlotCount(i);
                            if (slotValue != 0 && slotCount > 0)
                            {
                                data += $"\t{slotValue}:{slotCount}";
                            }
                        }
                    }
                }
            }
            data += "\nEntity";
            foreach (Entity entity in entities)
            {
                ComponentCreature creature = entity.FindComponent<ComponentCreature>();
                Vector3 vector3 = creature.ComponentBody.Position;
                data += $"|{creature.DisplayName}\t{vector3.X - End.X},{ vector3.Y - End.Y},{vector3.Z - End.Z}";
                ComponentInventoryBase EntityInventoryBase = entity.FindComponent<ComponentInventoryBase>();
                if (EntityInventoryBase != null)
                {
                    for (int i = 0; i < EntityInventoryBase.SlotsCount; i++)
                    {
                        int slotValue = EntityInventoryBase.GetSlotValue(i);
                        int slotCount = EntityInventoryBase.GetSlotCount(i);
                        if (slotValue != 0 && slotCount > 0)
                        {
                            data += $"\t{slotValue}:{slotCount}";
                        }
                    }
                }
            }
            fileStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            fileStream.Dispose();
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"复制成功", true, true);
        }

        public static void SpecialPasetData(CreatorAPI creatorAPI, string path, Point3 Start, Point3 End)
        {
            ChunkData chunkData = new ChunkData(creatorAPI);
            creatorAPI.revokeData = new ChunkData(creatorAPI);
            Stream stream = File.OpenRead(path);
            CreatorMain.Math.StartEnd(ref Start,ref End);
            StreamReader streamReader = new StreamReader(stream);
            string text = streamReader.ReadToEnd();
            streamReader.Dispose();
            stream.Dispose();
            string[] data = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] data2 = data[0].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] data3 = data2[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int MaxX = int.Parse(data3[0]);
            int MaxY = int.Parse(data3[1]);
            int MaxZ = int.Parse(data3[2]);
            int num = 0;
            for (int x = 0; x <= MaxX; x++)
            {
                for (int y = 0; y <= MaxY; y++)
                {
                    for (int z = 0; z <= MaxZ; z++)
                    {
                        creatorAPI.CreateBlock(End.X + x,End.Y+y,End.Z+z,int.Parse(data2[num+1]),chunkData);
                        num++;
                    }
                }
            }
            chunkData.Render();
            string[] data4 = data[1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 1; i < data4.Length; i++)
            {
                string[] data5 = data4[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                string[] data6 = data5[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                DatabaseObject databaseObject = GameManager.Project.GameDatabase.Database.FindDatabaseObject(data5[0], GameManager.Project.GameDatabase.EntityTemplateType, true);
                ValuesDictionary valuesDictionary = new ValuesDictionary();
                valuesDictionary.PopulateFromDatabaseObject(databaseObject);
                valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(int.Parse(data6[0])+ End.X, int.Parse(data6[1]) + End.Y, int.Parse(data6[2]) + End.Z));
                Entity entity = GameManager.Project.CreateEntity(valuesDictionary);
                ComponentInventoryBase inventoryBase = entity.FindComponent<ComponentInventoryBase>();
                if (inventoryBase != null)
                {
                    for (int j = 2, s = 0; j < data5.Length; j++, s++)
                    {

                        string[] data7 = data5[j].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        inventoryBase.AddSlotItems(s, int.Parse(data7[0]), int.Parse(data7[1]));
                    }
                }
                GameManager.Project.AddEntity(entity);
            }
            string[] data8 = data[2].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < data8.Length; i++)
            {
                string[] data9 = data8[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                string[] data10 = data9[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Entity entity = DatabaseManager.CreateEntity(GameManager.Project, data9[0], true);
                entity.FindComponent<ComponentBody>(true).Position = new Vector3(float.Parse(data10[0]) + End.X, float.Parse(data10[1]) + End.Y, float.Parse(data10[2]) + End.Z);
                entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1.2f);
                entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.25f;
                ComponentInventoryBase inventoryBase = entity.FindComponent<ComponentInventoryBase>();
                if (inventoryBase != null)
                {
                    for (int j = 2, s = 0; j < data9.Length; j++, s++)
                    {
                        string[] data11 = data9[j].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        inventoryBase.AddSlotItems(s, int.Parse(data11[0]), int.Parse(data11[1]));
                    }
                }
                GameManager.Project.AddEntity(entity);
            }
            creatorAPI.componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"粘贴成功，共{num}个方块", true, true);











        }



    }
}