using System.IO;
using System.Text;

namespace CreatorModAPI
{
    public static class FileOperation
    {
        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static bool IsFileInUse(this string file)
        {
            bool isUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(file, FileMode.Open);
                fs.Dispose();
                isUse = false;
            }
            catch { }
            return isUse;
        }
        public static Stream CreateStream(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            stream.Dispose();
            return memoryStream;
        }
        public static bool Delete(this string directory)
        {
            if (File.Exists(directory))
            {
                if (IsFileInUse(directory)) return false;
                File.Delete(directory);
                return true;
            }
            foreach (string dire in Directory.GetDirectories(directory)) Delete(dire);
            foreach (string file in Directory.GetFiles(directory))
            {
                if (!IsFileInUse(file)) File.Delete(file); else return false;
            }
            if (Directory.Exists(directory))
                Directory.Delete(directory);
            return true;
        }
        /*public static bool IsCopyWorld(this string file)
        {
            bool b = true;
            FileStream stream = new FileStream(file, FileMode.Open);
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                byte[] array = new byte[7];
                if (binaryReader.Read(array, 0, array.Length) != array.Length && array[0] == (byte)'W' && array[1] == (byte)'o' && array[2] == (byte)'r' && array[3] == (byte)'l' && array[4] == (byte)'d' && array[5] == (byte)'\0')
                {
                    try
                    {
                        binaryReader.Dispose();
                        stream.Dispose();
                        CopyAndPaste.ConversionCopyFile(file);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                if (binaryReader.Read(array, 0, array.Length) != array.Length || array[0] != (byte)'S' || array[1] != (byte)'C' || array[2] != (byte)'C' || array[3] != (byte)'O' || array[4] != (byte)'P' || array[5] != (byte)'Y' || array[6] != (byte)'\0')
                {
                    b = false;
                }
                binaryReader.Dispose();
                stream.Dispose();
            }
            return b;
        }
        public static bool IsOneKeyWorld(this string file)
        {
            bool b = true;
            FileStream stream = new FileStream(file, FileMode.Open);
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                byte[] array = new byte[7];
                if (binaryReader.Read(array, 0, array.Length) != array.Length || array[0] != (byte)'O' || array[1] != (byte)'n' || array[2] != (byte)'e' || array[3] != (byte)'K' || array[4] != (byte)'e' || array[5] != (byte)'y' || array[6] != (byte)'\0')
                {
                    b = false;
                }
                binaryReader.Dispose();
                stream.Dispose();
            }
            return b;
        }*/
        public static FileStream CreateFile(this string file)
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(file, FileMode.Create);
            }
            catch (DirectoryNotFoundException)
            {
                string[] directorys = file.Split('/');
                string path = "";
                for (int i = 0; i < directorys.Length - 1; i++)
                {
                    if (i == directorys.Length - 1)
                    {
                        path += directorys[i];
                        break;
                    }
                    path += directorys[i] + "/";
                }
                Directory.CreateDirectory(path);
                fileStream = new FileStream(file, FileMode.Create);
            }
            return fileStream;
        }
    }
}