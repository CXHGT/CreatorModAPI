using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Engine;
using Engine.Serialization;
using Game;

namespace CreatorModAPI
{
    public class CreatorGenerationAlgorithm
    {
        /// <summary>
        /// 两点连线
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public IEnumerable<Point3> TwoPointLineGeneration(Point3 startPoint, Point3 endPoint)
        {
            int lengin = Math.Max(MathUtils.Max(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)), Math.Abs(startPoint.Z - endPoint.Z));
            int num = 0;
            while (num <= lengin)
            {
                yield return new Point3(startPoint.X + (int)(Math.Round((float)num / lengin * (endPoint.X - startPoint.X))), startPoint.Y + (int)(Math.Round((float)num / lengin * (endPoint.Y - startPoint.Y))), startPoint.Z + (int)(Math.Round((float)num / lengin * (endPoint.Z - startPoint.Z))));
                num++;
            }
        }
        /// <summary>
        /// 两点连线（栏栅式）
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public IEnumerable<Point3> TwoPointLineGeneration2(Point3 startPoint, Point3 endPoint)
        {
            int lengin = Math.Max(MathUtils.Max(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)), Math.Abs(startPoint.Z - endPoint.Z));
            float num = 0;
            bool sc = false;
            while (num <= lengin)
            {
                double mx = Math.Round(num / lengin * (endPoint.X - startPoint.X) + ((endPoint.X - startPoint.X) > 0 ? 0.49f : -0.49f));
                if ((int)mx != (int)Math.Round(num / lengin * (endPoint.X - startPoint.X))) sc = true;
                double mz = Math.Round(num / lengin * (endPoint.Z - startPoint.Z) + (sc ? ((endPoint.Z - startPoint.Z) < 0 ? 0.01f : -0.01f) : ((endPoint.Z - startPoint.Z) > 0 ? 0.01f : -0.01f)));
                if (!sc && (int)mz == (int)Math.Round(num / lengin * (endPoint.X - startPoint.X))) 
                    mx += mx > 0 ? 1 : -1;
                yield return new Point3(startPoint.X + (int)mx, startPoint.Y + (int)Math.Round(num / lengin * (endPoint.Y - startPoint.Y) + ((endPoint.Y - startPoint.Y) > 0 ? 0.01f : -0.01f)), startPoint.Z + (int)mz);
                Log.Error($"{startPoint.X + num / lengin * (endPoint.X - startPoint.X)},{startPoint.Y + num / lengin * (endPoint.Y - startPoint.Y)},{startPoint.Z + num / lengin * (endPoint.Z - startPoint.Z)}");
                Log.Error($"Round : {mx},{Math.Round(num / lengin * (endPoint.Y - startPoint.Y) + ((endPoint.Y - startPoint.Y) > 0 ? 0.01f : -0.01f))},{mz}");
                num += 0.5f;
            }
        }

        /// <summary>
        /// 球体
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="XRadius"></param>
        /// <param name="YRadius"></param>
        /// <param name="ZRadius"></param>
        /// <param name="Hollow"></param>
        /// <param name="XYZType">切半</param>
        /// <param name="typeBool">true位正，false为负</param>
        /// <returns></returns>
        public IEnumerable<Point3> Sphere(Vector3 Position, int XRadius, int YRadius, int ZRadius, bool Hollow = false, CreatorMain.CreatorType? XYZType = null, bool typeBool = false)
        {
            int MaxRadius = Math.Max(Math.Max(XRadius,YRadius),ZRadius);
            for (int x = -XRadius; x <= XRadius; x++)
            {
                for (int y = -YRadius; y <= YRadius; y++)
                {
                    for (int z = -ZRadius; z <= ZRadius; z++)
                    {
                        if (((int)Math.Sqrt((float)x * MaxRadius / XRadius * x * MaxRadius / XRadius + (float)y * MaxRadius / YRadius * y * MaxRadius / YRadius + (float)z * MaxRadius / ZRadius * z * MaxRadius / ZRadius)) <= MaxRadius)
                        {
                            if (XYZType.HasValue)
                            {
                                if (XYZType == CreatorMain.CreatorType.X)
                                    if (typeBool)
                                    {
                                        if (Position.X + x < Position.X) continue;
                                    }
                                    else
                                    {
                                        if (Position.X + x > Position.X) continue;
                                    }
                                else if (XYZType == CreatorMain.CreatorType.Y)
                                    if (typeBool)
                                    {
                                        if (Position.Y + y < Position.Y) continue;
                                    }
                                    else
                                    {
                                        if (Position.Y + y > Position.Y) continue;
                                    }
                                else if (XYZType == CreatorMain.CreatorType.Z)
                                    if (typeBool)
                                    {
                                        if (Position.Z + z < Position.Z) continue;
                                    }
                                    else
                                    {
                                        if (Position.Z + z > Position.Z) continue;
                                    }
                            }
                            if (Hollow && (int)Math.Sqrt((float)(Math.Abs(x) + 1) * MaxRadius / XRadius * (Math.Abs(x) + 1) * MaxRadius / XRadius + (float)y * MaxRadius / YRadius * y * MaxRadius /YRadius + (float)z * MaxRadius / ZRadius * z * MaxRadius / ZRadius) <= MaxRadius && (int)Math.Sqrt((float)x * MaxRadius / XRadius * x * MaxRadius / XRadius + (float)(Math.Abs(y) + 1) * MaxRadius / YRadius * (Math.Abs(y) + 1) * MaxRadius / YRadius + (float)z * MaxRadius / ZRadius * z * MaxRadius / ZRadius) <= MaxRadius && (int)Math.Sqrt((float)x * MaxRadius / XRadius * x * MaxRadius / XRadius + (float)y * MaxRadius / YRadius * y * MaxRadius / YRadius + (float)(Math.Abs(z) + 1) * MaxRadius / ZRadius * (Math.Abs(z) + 1) * MaxRadius / ZRadius) <= MaxRadius) continue;
                            yield return new Point3((int)(Position.X + x), (int)(Position.Y + y), (int)(Position.Z + z));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 球体
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Hollow"></param>
        /// <param name="XYZType"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Sphere(Point3 Position,int Radius,bool Hollow = false, CreatorMain.CreatorType? XYZType = null, bool typeBool = false)
        {
            int MRadius = Radius * Radius;
            for (int x = -Radius; x <= Radius; x++)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        if (x  * x + y  * y + z  * z  <= MRadius)
                        {
                            if (XYZType.HasValue)
                            {
                                if (XYZType == CreatorMain.CreatorType.X)
                                    if (typeBool)
                                    {
                                        if (Position.X + x < Position.X) continue;
                                    }
                                    else
                                    {
                                        if (Position.X + x > Position.X) continue;
                                    }
                                else if (XYZType == CreatorMain.CreatorType.Y)
                                    if (typeBool)
                                    {
                                        if (Position.Y + y < Position.Y) continue;
                                    }
                                    else
                                    {
                                        if (Position.Y + y > Position.Y) continue;
                                    }
                                else if (XYZType == CreatorMain.CreatorType.Z)
                                    if (typeBool)
                                    {
                                        if (Position.Z + z < Position.Z) continue;
                                    }
                                    else
                                    {
                                        if (Position.Z + z > Position.Z) continue;
                                    }
                            }
                            if (Hollow && (Math.Abs(x) + 1) * (Math.Abs(x)+1) + y * y + z * z <= MRadius && x * x + (Math.Abs(y) + 1) * (Math.Abs(y) + 1) +z * z <= MRadius && x * x +y * y + (Math.Abs(z) + 1) * (Math.Abs(z) + 1)  <= MRadius) continue;
                            yield return new Point3(Position.X + x, Position.Y + y, Position.Z + z);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 球体
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="Hollow"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Sphere(Point3 Start,Point3 End,bool Hollow = false)
        {
            throw new Exception("no way");
        }



        /// <summary>
        /// 棱体
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Hollow"></param>
        /// <param name="creatorType"></param>
        /// <param name="XYZtype"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Prism(Point3 Position, int Radius, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool Hollow = false, CreatorMain.CreatorType? XYZtype = null, bool typeBool = false)
        {
            for (int x = -Radius; x <= Radius; x++)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        if (XYZtype.HasValue)
                        {
                            if (XYZtype == CreatorMain.CreatorType.X)
                                if (typeBool)
                                {
                                    if (Position.X + x < Position.X) continue;
                                }
                                else
                                {
                                    if (Position.X + x > Position.X) continue;
                                }
                            if (XYZtype == CreatorMain.CreatorType.Y)
                                if (typeBool)
                                {
                                    if (Position.Y + y < Position.Y) continue;
                                }
                                else
                                {
                                    if (Position.Y + y > Position.Y) continue;
                                }
                            if (XYZtype == CreatorMain.CreatorType.Z)
                                if (typeBool)
                                {
                                    if (Position.Z + z < Position.Z) continue;
                                }
                                else
                                {
                                    if (Position.Z + z > Position.Z) continue;
                                }
                        }
                        if (creatorType == CreatorMain.CreatorType.Y)
                        {
                            if ((Math.Abs(x) + Math.Abs(y) > Radius || Math.Abs(z) + Math.Abs(y) > Radius) || (Hollow && Math.Abs(x) + Math.Abs(y) < Radius && Math.Abs(z) + Math.Abs(y) < Radius))
                            {
                                continue;
                            }
                        }
                        if (creatorType == CreatorMain.CreatorType.X)
                        {
                            if ((Math.Abs(x) + Math.Abs(y) > Radius || Math.Abs(z) + Math.Abs(x) > Radius) || (Hollow && Math.Abs(x) + Math.Abs(y) < Radius && Math.Abs(x) + Math.Abs(z) < Radius))
                            {
                                continue;
                            }
                        }
                        if (creatorType == CreatorMain.CreatorType.Z)
                        {
                            if ((Math.Abs(z) + Math.Abs(y) > Radius || Math.Abs(z) + Math.Abs(x) > Radius) || (Hollow && Math.Abs(z) + Math.Abs(y) < Radius && Math.Abs(z) + Math.Abs(x) < Radius))
                            {
                                continue;
                            }
                        }
                        yield return new Point3(Position.X + x, Position.Y + y, Position.Z + z);
                    }
                }
            }
        }
        /// <summary>
        /// 棱锥
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Hollow"></param>
        /// <param name="XYZType"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Pyramid(Point3 Position, int Radius, bool Hollow = false, CreatorMain.CreatorType? XYZType = null, bool typeBool = false)
        {
            for (int x = -Radius; x <= Radius; x++)
            {
                for (int y = -Radius; y <= Radius; y++)
                {
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        if (XYZType != null)
                        {
                            if (XYZType == CreatorMain.CreatorType.X)
                                if (typeBool)
                                {
                                    if (Position.X + x < Position.X) continue;
                                }
                                else
                                {
                                    if (Position.X + x > Position.X) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Y)
                                if (typeBool)
                                {
                                    if (Position.Y + y < Position.Y) continue;
                                }
                                else
                                {
                                    if (Position.Y + y > Position.Y) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Z)
                                if (typeBool)
                                {
                                    if (Position.Z + z < Position.Z) continue;
                                }
                                else
                                {
                                    if (Position.Z + z > Position.Z) continue;
                                }
                        }
                        if (!(Math.Abs(x) + Math.Abs(y) + Math.Abs(z) > Radius))
                        {
                            if (!(Hollow && Math.Abs(x) + Math.Abs(y) + Math.Abs(z) < Radius))
                            {
                                yield return new Point3(Position.X + x, Position.Y + y, Position.Z + z);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 圆柱
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Height"></param>
        /// <param name="Hollow"></param>
        /// <param name="creatorType"></param>
        /// <param name="YType"></param>
        /// <param name="XYZType"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Cylindrical(Vector3 Position, int XRadius, int Height, int ZRadius, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool YType = true, bool Hollow = false)
        {
            int MaxRadius = Math.Max(XRadius, ZRadius);
            for (int x = -XRadius; x <= XRadius; x++)
            {
                for (int z = -ZRadius; z <= ZRadius; z++)
                {
                    if (((int)Math.Sqrt((float)x * MaxRadius / XRadius * x * MaxRadius / XRadius + (float)z * MaxRadius / ZRadius * z * MaxRadius / ZRadius)) <= MaxRadius)
                    {
                        if (Hollow && (int)Math.Sqrt(((float)Math.Abs(x) + 1) * MaxRadius / XRadius * (Math.Abs(x) + 1) * MaxRadius / XRadius + (float)z * MaxRadius / ZRadius * z * MaxRadius / ZRadius) <= MaxRadius && (int)Math.Sqrt((float)x * x * MaxRadius / XRadius * MaxRadius / XRadius + (float)(Math.Abs(z) + 1) * MaxRadius / ZRadius * (Math.Abs(z) + 1) * MaxRadius / ZRadius) <= MaxRadius) continue;
                        for (int y = 0; y < Height; y++)
                        {
                            int Y;
                            if (YType) Y = y; else Y = -y;
                            if (creatorType == CreatorMain.CreatorType.X)
                            {
                                yield return new Point3((int)Position.X + Y, (int)Position.Y + x, (int)Position.Z + z);
                            }
                            else if (creatorType == CreatorMain.CreatorType.Y)
                            {
                                yield return new Point3((int)Position.X + x, (int)Position.Y + Y, (int)Position.Z + z);
                            }
                            else
                            {
                                yield return new Point3((int)Position.X + x, (int)Position.Y + z, (int)Position.Z + Y);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 圆柱
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Height"></param>
        /// <param name="creatorType"></param>
        /// <param name="YType"></param>
        /// <param name="Hollow"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Cylindrical(Point3 Position, int Radius, int Height, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool YType = true, bool Hollow = false)
        {
            for (int x = -Radius; x <= Radius; x++)
            {
                for (int z = -Radius; z <= Radius; z++)
                {
                    if (Math.Sqrt(x  * x + z *z) <= Radius)
                    {
                        if (Hollow && Math.Sqrt((Math.Abs(x) + 1) *  (Math.Abs(x) + 1) + z * z) <= Radius && Math.Sqrt(x * x + (Math.Abs(z) + 1) * (Math.Abs(z) + 1)) <= Radius) continue;
                        for (int y = 0; y < Height; y++)
                        {
                            int Y;
                            if (YType) Y = y; else Y = -y;
                            if (creatorType == CreatorMain.CreatorType.X)
                            {
                                yield return new Point3(Position.X + Y, Position.Y + x, Position.Z + z);
                            }
                            else if (creatorType == CreatorMain.CreatorType.Y)
                            {
                                yield return new Point3(Position.X + x, Position.Y + Y, Position.Z + z);
                            }
                            else
                            {
                                yield return new Point3(Position.X + x, Position.Y + z, Position.Z + Y);
                            }
                        }
                    }
                }
            }
        }







        /// <summary>
        /// 棱柱
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Height"></param>
        /// <param name="Hollow"></param>
        /// <param name="creatorType"></param>
        /// <param name="YType"></param>
        /// <param name="XYZType"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> PrismColumn(Point3 Position, int Radius, int Height, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool YType = true, bool Hollow = false, CreatorMain.CreatorType? XYZType = null, bool typeBool = false)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        if (XYZType != null)
                        {
                            if (XYZType == CreatorMain.CreatorType.X)
                                if (typeBool)
                                {
                                    if (Position.X + x < Position.X) continue;
                                }
                                else
                                {
                                    if (Position.X + x > Position.X) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Y)
                                if (typeBool)
                                {
                                    if (Position.Y + y < Position.Y) continue;
                                }
                                else
                                {
                                    if (Position.Y + y > Position.Y) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Z)
                                if (typeBool)
                                {
                                    if (Position.Z + z < Position.Z) continue;
                                }
                                else
                                {
                                    if (Position.Z + z > Position.Z) continue;
                                }
                        }
                        if (!(Math.Abs(x) + Math.Abs(z) > Radius))
                        {
                            if (!(Hollow && Math.Abs(x) + Math.Abs(z) < Radius))
                            {
                                int Y;
                                if (YType) Y = y; else Y = -y;
                                if (creatorType == CreatorMain.CreatorType.X)
                                {
                                    yield return new Point3(Position.X + Y, Position.Y + x, Position.Z + z);
                                }
                                else if (creatorType == CreatorMain.CreatorType.Y)
                                {
                                    yield return new Point3(Position.X + x, Position.Y + Y, Position.Z + z);
                                }
                                else
                                {
                                    yield return new Point3(Position.X + x, Position.Y + z, Position.Z + Y);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 柱子
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <param name="Height"></param>
        /// <param name="Hollow"></param>
        /// <param name="creatorType"></param>
        /// <param name="YType"></param>
        /// <param name="XYZType"></param>
        /// <param name="typeBool"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Pillars(Point3 Position, int Radius, int Height, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool YType = true, bool Hollow = false, CreatorMain.CreatorType? XYZType = null, bool typeBool = false)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        if (XYZType != null)
                        {
                            if (XYZType == CreatorMain.CreatorType.X)
                                if (typeBool)
                                {
                                    if (Position.X + x < Position.X) continue;
                                }
                                else
                                {
                                    if (Position.X + x > Position.X) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Y)
                                if (typeBool)
                                {
                                    if (Position.Y + y < Position.Y) continue;
                                }
                                else
                                {
                                    if (Position.Y + y > Position.Y) continue;
                                }
                            if (XYZType == CreatorMain.CreatorType.Z)
                                if (typeBool)
                                {
                                    if (Position.Z + z < Position.Z) continue;
                                }
                                else
                                {
                                    if (Position.Z + z > Position.Z) continue;
                                }
                        }
                        if (!(Math.Abs(x) > Radius && Math.Abs(z) > Radius))
                        {
                            if (!(Hollow && (Math.Abs(x) < Radius && Math.Abs(z) < Radius)))
                            {
                                int Y;
                                if (YType) Y = y; else Y = -y;
                                if (creatorType == CreatorMain.CreatorType.X)
                                {
                                    yield return new Point3(Position.X + Y, Position.Y + x, Position.Z + z);
                                }
                                else if (creatorType == CreatorMain.CreatorType.Y)
                                {
                                    yield return new Point3(Position.X + x, Position.Y + Y, Position.Z + z);
                                }
                                else
                                {
                                    yield return new Point3(Position.X + x, Position.Y + z, Position.Z + Y);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 矩形
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="type">null为实心，true空心，false框架</param>
        /// <returns></returns>
        public IEnumerable<Point3> Rectangular(Point3 Start, Point3 End, bool? type = null)
        {
            CreatorMain.Math.StartEnd(ref Start, ref End);
                for (int x = 0; x <= Start.X - End.X; x++)
                {
                    for (int y = 0; y <= Start.Y - End.Y; y++)
                    {
                        for (int z = 0; z <= Start.Z - End.Z; z++)
                        {
                            if (!(type == true && x > 0 && x < Start.X - End.X && y > 0 && y < Start.Y - End.Y && z > 0 && z < Start.Z - End.Z))
                            {
                                if (!(type == false && ((x >= 0 && x <= Start.X - End.X && y > 0 && y < Start.Y - End.Y && z > 0 && z < Start.Z - End.Z) || (y >= 0 && y <= Start.Y - End.Y && x > 0 && x < Start.X - End.X && z > 0 && z < Start.Z - End.Z) || (z >= 0 && z <= Start.Z - End.Z && y > 0 && y < Start.Y - End.Y && x > 0 && x < Start.X - End.X))))
                                {
                                    yield return new Point3(End.X + x, End.Y + y, End.Z + z);
                                }
                            }
                        }
                    }
                }
        }
        /// <summary>
        /// 圆环
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Height"></param>
        /// <param name="Radius"></param>
        /// <param name="Hollow"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Circle(Point3 Position, int Height, int Radius, CreatorMain.CreatorType type = CreatorMain.CreatorType.Y, bool Hollow = false)
        {
            int _Radius = Height - Radius;
            for (int x = -Radius; x <= Radius; x++)
            {
                int _radius = _Radius + Radius - x;
                for (int z = -Radius; z <= Radius; z++)
                {
                    if (((int)Math.Sqrt(x * x + z * z)) <= Radius)
                    {
                        if (Hollow && (int)Math.Sqrt((Math.Abs(x) + 1) * (Math.Abs(x) + 1) + z * z) <= Radius && (int)Math.Sqrt(x * x + (Math.Abs(z) + 1) * (Math.Abs(z) + 1)) <= Radius)
                        {
                            continue;
                        }
                        for (int x_2 = -_radius; x_2 <= _radius; x_2++)
                        {
                            for (int z_2 = -_radius; z_2 <= _radius; z_2++)
                            {
                                if (((int)Math.Sqrt(x_2 * x_2 + z_2 * z_2)) <= _radius)
                                {
                                    if ((int)Math.Sqrt((float)(Math.Abs(x_2) + 0.5f) * ((float)Math.Abs(x_2) + 0.5f) + (Math.Abs(z_2) + 1) * (Math.Abs(z_2) + 1)) <= _radius && (int)Math.Sqrt((Math.Abs(x_2) + 1) * (Math.Abs(x_2) + 1) + ((float)Math.Abs(z_2) + 0.5f) * ((float)Math.Abs(z_2) + 0.5f)) <= _radius)
                                    {
                                        continue;
                                    }
                                    if (type == CreatorMain.CreatorType.Y)
                                    {
                                        yield return new Point3(Position.X + x_2, Position.Y + z, Position.Z + z_2);
                                    }
                                    else if (type == CreatorMain.CreatorType.X)
                                    {
                                        yield return new Point3(Position.X + z, Position.Y + x_2, Position.Z + z_2);
                                    }
                                    else
                                    {
                                        yield return new Point3(Position.X + z_2, Position.Y + x_2, Position.Z + z);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 迷宫
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Maze(Point3 Start, Point3 End)
        {
            CreatorMain.Math.StartEnd(ref Start, ref End);
            int StartX = Start.X - End.X;
            int StartZ = Start.Z - End.Z;
            Maze maze = new Maze(StartX / 2, StartZ / 2);
            bool[,] mazeArray = maze.GetBoolArray();
            for (int x = 0; x <= (StartX % 2 != 0 ? StartX - 1 : StartX); x++)
            {
                for (int z = 0; z <= (StartZ % 2 != 0 ? StartZ - 1 : StartZ); z++)
                {
                    if (x == 1 && z == 0) continue;
                    if (x == (StartX % 2 != 0 ? StartX - 1 : StartX) && z == (StartZ % 2 != 0 ? StartZ - 1 : StartZ) - 1) continue;
                    if (mazeArray[x, z])
                    {
                        for (int y = 0; y <= Start.Y - End.Y; y++)
                        {
                            yield return new Point3(End.X + x, End.Y + y, End.Z + z);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 螺旋
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Height"></param>
        /// <param name="Radius"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        public IEnumerable<Point3> Spiral(Point3 Position,int Height,int Radius,int Number, CreatorMain.CreatorType creatorType = CreatorMain.CreatorType.Y, bool YType = true)
        {
            for(int angle = 0;angle <= 360 * Number; angle++)
            {
                float x = (float)Radius * angle / 360 * MathUtils.Cos(angle * MathUtils.PI / 180);
                float z = (float)Radius * angle / 360 * MathUtils.Sin(angle * MathUtils.PI / 180);
                for(int y = 0; y<= Height - 1; y++)
                {
                    int Y;
                    if (YType) Y = y; else Y = -y;
                    if (creatorType == CreatorMain.CreatorType.X)
                    {
                        yield return new Point3(Position.X + Y, Position.Y + (int)x, Position.Z + (int)z);
                    }
                    else if (creatorType == CreatorMain.CreatorType.Y)
                    {
                        yield return new Point3(Position.X + (int)x, Position.Y + Y, Position.Z + (int)z);
                    }
                    else
                    {
                        yield return new Point3(Position.X + (int)x, Position.Y + (int)z, Position.Z + Y);
                    }
                }
            }
        }
        /// <summary>
        /// 3点成面
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public IEnumerable<Point3> ThreePointPlane(Point3 p1,Point3 p2,Point3 p3)
        {
            List<Point3> listPoint3 = new List<Point3>();
            foreach (Point3 point3 in TwoPointLineGeneration(p1,p2))
            {
                listPoint3.Add(point3);
            }
            foreach(Point3 p in listPoint3)
            {
                foreach (Point3 point3 in TwoPointLineGeneration(p, p3))
                {
                    yield return point3;
                }
            }
            listPoint3.Clear();
            foreach (Point3 point3 in TwoPointLineGeneration(p1, p3))
            {
                listPoint3.Add(point3);
            }
            foreach (Point3 p in listPoint3)
            {
                foreach (Point3 point3 in TwoPointLineGeneration(p, p2))
                {
                    yield return point3;
                }
            }
            listPoint3.Clear();
            foreach (Point3 point3 in TwoPointLineGeneration(p3, p2))
            {
                listPoint3.Add(point3);
            }
            foreach (Point3 p in listPoint3)
            {
                foreach (Point3 point3 in TwoPointLineGeneration(p, p1))
                {
                    yield return point3;
                }
            }
        }
        
        public IEnumerable<Point3> ThreePointPlane2(Point3 p1, Point3 p2, Point3 p3)
        {
            Point3 p;
            if (p2.Y > p1.Y)
            {
                p = p1;
                p1 = p2;
                p2 = p;
            }
            if (p3.Y > p1.Y)
            {
                p = p1;
                p1 = p3;
                p3 = p;
            }
            if(p3.X > p2.X)
            {
                p = p2;
                p2 = p3;
                p3 = p;
            }
            List<Point3> listPoint3 = new List<Point3>();
            foreach(Point3 point3 in TwoPointLineGeneration(p1,p2))
            {
                listPoint3.Add(point3);
            }
            int lengin = Math.Max(MathUtils.Max(Math.Abs(p3.X - p1.X), Math.Abs(p3.Y - p1.Y)), Math.Abs(p3.Z - p1.Z));
            int num;
            foreach (Point3 point3 in TwoPointLineGeneration(p3, p2))
            {
                num = 0;
                while (num <= lengin)
                {
                    p = new Point3(point3.X + (int)(Math.Round((float)num / lengin * (p1.X - p3.X))), point3.Y + (int)(Math.Round((float)num / lengin * (p1.Y - p3.Y))), point3.Z + (int)(Math.Round((float)num / lengin * (p1.Z - p3.Z))));
                    if (listPoint3.Contains(p)) break;
                    yield return p;
                    num++;
                }
            }



        }




        /// <summary>
        /// 4点空间
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public IEnumerable<Point3> FourPointSpace(Point3 p1,Point3 p2,Point3 p3,Point3 p4)
        {
            foreach(Point3 point3 in ThreePointPlane(p1, p2, p3))
            {
                yield return point3;
            }
            foreach (Point3 point3 in ThreePointPlane(p1, p2, p4))
            {
                yield return point3;
            }
            foreach (Point3 point3 in ThreePointPlane(p1, p4, p3))
            {
                yield return point3;
            }
            foreach (Point3 point3 in ThreePointPlane(p4, p2, p3))
            {
                yield return point3;
            }
        }



    }
}