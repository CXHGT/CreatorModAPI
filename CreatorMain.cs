using System;
using Game;
using System.Collections.Generic;
using Engine;
using System.IO;

namespace CreatorModAPI
{
    public static class CreatorMain
    {
        public enum CreatorType
        {
            X,
            Y,
            Z
        }
        public static string version = "pro2.0.3";
        public static string password = "456321";
        public static bool canUse = true;
        public static bool professional = false;
        public static readonly string CacheDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Cache";
        /// <summary>
        /// SD卡目录
        /// </summary>
        private static string sdcard;
        public static string Sdcard
        {
            get
            {
                if (sdcard == null)
                {
                    string txt = "/sdcard/Android/data/Survivalcraft/CreatorMod";
                    try
                    {
                        string[] data = Environment.OSVersion.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string d in data)
                        {
                            if (d == "Windows" || d == "windows")
                            {
                                txt = Directory.GetCurrentDirectory() + "/CreatorMod";
                                break;
                            }
                        }
                    }
                    catch
                    {
                        txt = Directory.GetCurrentDirectory() + "/CreatorMod";
                    }
                    sdcard = txt;
                }
                return sdcard;
            }
        }
        /// <summary>
        /// 导出wMod文件目录
        /// </summary>
        public static readonly string Export_wMod_Directory = Sdcard+"/wMod";
        /// <summary>
        /// 导出oMod文件目录
        /// </summary>
        public static readonly string Export_oMod_Directory = Sdcard + "/oMod";
        /// <summary>
        /// 导出复制文件目录
        /// </summary>
        public static readonly string Export_CopyFile_Directory = Sdcard + "/Copy";
        /// <summary>
        /// 导出一键生成文件目录
        /// </summary>
        public static readonly string Export_OnekeyFile_Directory = Sdcard + "/OneKey";
        /// <summary>
        /// 缓存一键生成所在目录
        /// </summary>
        public static readonly string OneKeyFile = $"{CacheDirectory}/CacheFile.od";
        /// <summary>
        /// 缓存复制所在目录
        /// </summary>
        public static readonly string CopyFile = $"{CacheDirectory}/CacheFile.cd";
        /// <summary>
        /// 缓存特殊复制所在目录
        /// </summary>
        public static readonly string SpecialCopyFile = $"{CacheDirectory}/CacheFile.sd";
        /// <summary>
        /// 导出mod配置文件目录
        /// </summary>
        public static readonly string Export_ModFile_Directory = Sdcard + "/Mod";
        /// <summary>
        /// 穿透模式
        /// </summary>
        public static bool Penetrate = false;
        /// <summary>
        /// 开启穿透模式时，保存方块原有数据
        /// </summary>
        public static List<int> PenetrateBlocksID = new List<int>();

        public static bool LightWorld = false;

        public static List<int> LightWorldBlockID = new List<int>();


        public static class Math
        {
            public static void StartEnd(ref Point3 Start, ref Point3 End)
            {
                if (Start.X < End.X)
                {
                    int num = Start.X;
                    Start.X = End.X;
                    End.X = num;
                }
                if (Start.Y < End.Y)
                {
                    int num = Start.Y;
                    Start.Y = End.Y;
                    End.Y = num;
                }
                if (Start.Z < End.Z)
                {
                    int num = Start.Z;
                    Start.Z = End.Z;
                    End.Z = num;
                }
            }
        }
    }
}