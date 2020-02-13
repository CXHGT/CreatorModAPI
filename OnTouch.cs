using Engine;
using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreatorModAPI
{
    public static class OnTouch
    {
        public static object SleepTime { get; private set; }

        public static bool Touch(CreatorAPI creatorAPI,Point3 position)
        {
            ComponentPlayer player = creatorAPI.componentMiner.ComponentPlayer;
            int cellValue = GameManager.Project.FindSubsystem<SubsystemTerrain>(true).Terrain.GetCellValue(position.X, position.Y, position.Z);



/*            if (creatorAPI.twoPointsOfAttachment && creatorAPI.numberPoint == CreatorAPI.NumberPoint.Two)
            {
                Task.Run(delegate
                {
                    ChunkData chunkData = new ChunkData(creatorAPI);
                    creatorAPI.revokeData = new ChunkData(creatorAPI);
                    int num = 0;
                    foreach (Point3 point3 in creatorAPI.creatorGenerationAlgorithm.TwoPointLineGeneration2(creatorAPI.Position[0], creatorAPI.Position[1]))
                    {
                        creatorAPI.CreateBlock(point3, cellValue,chunkData);
                        num++;
                    }
                    chunkData.Render();
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共生成{num}个方块", true, true);
                });
            }
*/

            if (creatorAPI.oneKeyGeneration)
            {
                if (creatorAPI.onekeyType == CreatorAPI.OnekeyType.Build)
                {
                    if (File.Exists(CreatorMain.OneKeyFile))
                        Task.Run(delegate
                        {
                            OnekeyGeneration.GenerationData(creatorAPI, CreatorMain.OneKeyFile, position);
                        });
                    else
                        player.ComponentGui.DisplaySmallMessage($"未发现一键生成缓存文件，目录:{CreatorMain.OneKeyFile}\n请变更一键生成类型或关闭该功能", true, true);
                }
                else
                {

                }
                return false;
            }

            if (creatorAPI.ClearBlock)
            {
                Task.Run(delegate
                {
                    creatorAPI.revokeData = new ChunkData(creatorAPI);
                    int num = 0;
                    List<Point3> clearBlockList = new List<Point3>();
                    List<Point3> addList = new List<Point3>();
                    clearBlockList.Add(position);
                    while (true)
                    {
                        if (clearBlockList.Count <= 0) break;
                        foreach (Point3 point3 in clearBlockList)
                        {
                            if (!creatorAPI.launch) return;
                            if (creatorAPI.revokeData != null && creatorAPI.revokeData.GetChunk(point3.X, point3.Z) == null) creatorAPI.revokeData.CreateChunk(point3.X, point3.Z,true);
                            creatorAPI.SetBlock(point3.X, point3.Y, point3.Z, 0);
                            num++;
                            for (int x = -1; x <= 1; x++)
                            {
                                for (int y = -1; y <= 1; y++)
                                {
                                    for (int z = -1; z <= 1; z++)
                                    {
                                        if (point3.Y + y > 127) continue;
                                        int blockID = GameManager.Project.FindSubsystem<SubsystemTerrain>(true).Terrain.GetCellContentsFast(point3.X + x, point3.Y + y, point3.Z + z);
                                        if (blockID == 0 || blockID == 1) continue;
                                        if (MathUtils.Abs(x) + MathUtils.Abs(y) + MathUtils.Abs(z) > 1) continue;
                                        Point3 p = new Point3(point3.X + x, point3.Y + y, point3.Z + z);
                                        if (!clearBlockList.Contains(p) && !addList.Contains(p))
                                        {
                                            addList.Add(p);
                                        }
                                    }
                                }
                            }
                        }
                        clearBlockList = addList;
                        addList = new List<Point3>();
                    }
                    player.ComponentGui.DisplaySmallMessage($"操作成功，共清除{num}个方块", true, true);
                });
                return false;
            }
            return true;
        }
    }
}
