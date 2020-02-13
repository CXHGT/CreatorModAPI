using Game;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorModAPI
{

    /// <summary>
    /// 迷宫类
    /// </summary>
    public class Maze
    {
        /// <summary>
        /// 迷宫的小房间的数组
        /// </summary>
        private Room[,] roomMatrix;

        /// <summary>
        /// 保存通路的链表
        /// </summary>
        private List<List<Room>> roads;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private Random random;

        /// <summary>
        /// 迷宫的构造函数， 一旦构造完成， 迷宫的格数不能改变
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Maze(int width, int height)
        {
            // 实例化随机数生成器
            random = new Random();
            // 实例化房间
            InstRooms(width, height);
            // 组织房间
            OrganizeRooms();
            // 设定固定的门
            SetFixedDoor();
            // 使所有房间连通
            Interlink();
        }

        /// <summary>
        /// 实例化所有房间并添加进通路， 然后将通路添加到通路链表中
        /// </summary>
        private void InstRooms(int width, int height)
        {
            // 实例化房间数组
            roomMatrix = new Room[width, height];
            // 实例化通路链表
            roads = new List<List<Room>>();
            // 实例化每个房间， 并添加到对应的通路， 然后将通路添加到通路链表中
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // 实例化通路
                    List<Room> road = new List<Room>();
                    // 实例化一个房间
                    roomMatrix[i, j] = new Room();
                    // 将该单元格添加进通路
                    road.Add(roomMatrix[i, j]);
                    // 将通路添加进通路链表
                    roads.Add(road);
                }
            }
        }

        /// <summary>
        /// 将相邻的房间公共的门设为同一个对象的引用
        /// </summary>
        private void OrganizeRooms()
        {
            // 从上往下， 除了最后一行， 将每个房间下方向的门设为下方的房间上方向的门
            for (int i = 0; i < roomMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < roomMatrix.GetLength(1) - 1; j++)
                {
                    roomMatrix[i, j].BottonDoor = roomMatrix[i, j + 1].TopDoor;
                }
            }
            // 从左往右， 除了最后一列， 将每个房间的右边的门设为右边的房间左边的门
            for (int j = 0; j < roomMatrix.GetLength(1); j++)
            {
                for (int i = 0; i < roomMatrix.GetLength(0) - 1; i++)
                {
                    roomMatrix[i, j].RightDoor = roomMatrix[i + 1, j].LeftDoor;
                }
            }
        }

        /// <summary>
        /// 设定固定的门
        /// </summary>
        private void SetFixedDoor()
        {
            // 第一行的所有房间的上门设置为固定
            for (int i = 0; i < roomMatrix.GetLength(0); i++)
            {
                roomMatrix[i, 0].TopDoor.IsFixed = true;
            }
            // 最后一行的所有房间的下门设置为固定
            for (int i = 0; i < roomMatrix.GetLength(0); i++)
            {
                roomMatrix[i, roomMatrix.GetLength(1) - 1].BottonDoor.IsFixed = true;
            }
            // 第一列的所有房间的左门设置为固定
            for (int i = 0; i < roomMatrix.GetLength(1); i++)
            {
                roomMatrix[0, i].LeftDoor.IsFixed = true;
            }
            // 最后一列所有房间的右门设置为固定
            for (int i = 0; i < roomMatrix.GetLength(1); i++)
            {
                roomMatrix[roomMatrix.GetLength(0) - 1, i].RightDoor.IsFixed = true;
            }
        }

        /// <summary>
        /// 使迷宫的所有房间连通
        /// </summary>
        private void Interlink()
        {
            while (!AllRoomLinked())
            {
                // 随机从通路链表中选取一条通路
                List<Room> road = roads[random.UniformInt(0, 0xfffff) % roads.Count];
                // 获取该通路的轮廓上的门的链表
                List<Door> roadsOutlineDoors = GetOutlineDoors(road);
                // 从轮廓上的门的链表中随机选取一扇门作为准备打开的门
                Door doorReadyToOpen = roadsOutlineDoors[random.UniformInt(0, 0xfffff) % roadsOutlineDoors.Count];
                // 找出包含将要打开的门的两条旧的通路
                List<List<Room>> oldRoads = GetOldRoads(doorReadyToOpen);
                // 两条旧的路合成的一条新的通路
                List<Room> newRoad = GetNewRoad(oldRoads);
                // 从通路链表移除两条旧的通路
                RemoveOldRoads(oldRoads);
                // 添加新的通路到通路链表中
                roads.Add(newRoad);
                // 将准备打开的门打开
                doorReadyToOpen.OpenTheDoor();
            }
        }

        private void RemoveOldRoads(List<List<Room>> oldRoads)
        {
            // 移除两条旧的通路
            foreach (List<Room> oldRoad in oldRoads)
            {
                roads.Remove(oldRoad);
            }
        }

        /// <summary>
        /// 用两条旧的路合成一条新的通路
        /// </summary>
        /// <param name="oldRoad"></param>
        /// <returns></returns>
        private List<Room> GetNewRoad(List<List<Room>> oldRoad)
        {
            // 新的通路
            List<Room> newRoad = new List<Room>();
            // 遍历旧的两条通路里面的通路
            foreach (List<Room> road in oldRoad)
            {
                // 遍历每条旧的通路中的房间
                foreach (Room room in road)
                {
                    // 将旧的两条通路的所有房间添加到新的通路中
                    newRoad.Add(room);
                }
            }
            // 返回新的合并后的通路
            return newRoad;
        }

        /// <summary>
        /// 获取包含该门的两条旧的通路
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        private List<List<Room>> GetOldRoads(Door door)
        {
            // 用于保存旧的两条路的链表
            List<List<Room>> oldRoads = new List<List<Room>>();
            // 遍历所有通路， 找出包含指定的门的两条路
            foreach (List<Room> road in roads)
            {
                // 遍历每条通路的所有房间， 找出包含用于比较的门的房间
                foreach (Room room in road)
                {
                    // 只遍历处于非固定状态并且处于锁闭状态的门
                    if (TwoDoorAreEqual(room.TopDoor, door))
                    {
                        oldRoads.Add(road);
                        break;
                    }
                    // 只遍历处于非固定状态并且处于锁闭状态的门
                    if (TwoDoorAreEqual(room.BottonDoor, door))
                    {
                        oldRoads.Add(road);
                        break;
                    }
                    // 只遍历处于非固定状态并且处于锁闭状态的门
                    if (TwoDoorAreEqual(room.LeftDoor, door))
                    {
                        oldRoads.Add(road);
                        break;
                    }
                    // 只遍历处于非固定状态并且处于锁闭状态的门
                    if (TwoDoorAreEqual(room.RightDoor, door))
                    {
                        oldRoads.Add(road);
                        break;
                    }
                }
            }
            // 返回包含该门的两条旧通路
            return oldRoads;
        }

        /// <summary>
        /// 检查两扇合法的门是否为同一对象
        /// </summary>
        /// <param name="doorSource"></param>
        /// <param name="doorTarget"></param>
        /// <returns></returns>
        private bool TwoDoorAreEqual(Door doorSource, Door doorTarget)
        {
            // 如果检查的门处于锁闭状态并且不是固定的门， 则表示两扇门相通
            if (doorSource.GetLockState() && !doorSource.IsFixed)
            {
                if (Equals(doorSource, doorTarget))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否所有的房间已经连通
        /// </summary>
        /// <returns></returns>
        private bool AllRoomLinked()
        {
            //  当通路链表中只有一条通路时， 说明所有房间已连通， 返回真
            if (roads.Count == 1)
            {
                return true;
            }
            // 否则返回假
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取通路外部轮廓上的处于锁闭状态的门
        /// </summary>
        /// <returns></returns>
        private List<Door> GetOutlineDoors(List<Room> road)
        {
            // 用来保存轮廓上的门的链表
            List<Door> outlineDoors = new List<Door>();
            // 获取该通路所有锁闭状态的门
            foreach (Room room in road)
            {
                // 逐一判断该门是否属于轮廓上的门， 如果属于则加入， 否则则移除
                AddOutlineDoor(room.TopDoor, outlineDoors);
                AddOutlineDoor(room.BottonDoor, outlineDoors);
                AddOutlineDoor(room.LeftDoor, outlineDoors);
                AddOutlineDoor(room.RightDoor, outlineDoors);
            }
            return outlineDoors;
        }

        /// <summary>
        /// 将属于轮廓上的门加入轮廓上的门链表
        /// </summary>
        /// <param name="door"></param>
        /// <param name="outlineDoors"></param>
        private void AddOutlineDoor(Door door, List<Door> outlineDoors)
        {
            // 只处理处于非固定状态并且处于锁闭状态的门
            if (door.GetLockState() && !door.IsFixed)
            {
                // 如果该门未在链表中出现过， 则有可能是轮廓上的门， 先加入链表
                if (!outlineDoors.Contains(door))
                {
                    outlineDoors.Add(door);
                }
                // 如果该门在链表中出现了两次， 则不是轮廓上的门， 排除表中已存储的该门的实例（因为轮廓上的门只会于一个房间相交， 与两个房间相交的门就不是轮廓上的门）
                else
                {
                    outlineDoors.Remove(door);
                }
            }
        }

        /// <summary>
        /// 将迷宫房间转换为二维数组
        /// </summary>
        private bool[,] RoomToData()
        {
            // 新建一个布尔型的二维数组用于保存完成后的迷宫
            bool[,] dataMatrix = new bool[roomMatrix.GetLength(0) * 2 + 1, roomMatrix.GetLength(1) * 2 + 1];
            // 将房间不影响的门先全部预先填充为关闭， 节省时间复杂度
            PreFill(dataMatrix);
            // 遍历每个房间， 并将该房间的四个方向的门的开关状态映射到保存迷宫的数组中
            for (int xPos = 0; xPos < roomMatrix.GetLength(0); xPos++)
            {
                for (int yPos = 0; yPos < roomMatrix.GetLength(1); yPos++)
                {
                    SetData(dataMatrix, xPos, yPos, -1, 0, roomMatrix[xPos, yPos].LeftDoor.GetLockState());
                    SetData(dataMatrix, xPos, yPos, 1, 0, roomMatrix[xPos, yPos].RightDoor.GetLockState());
                    SetData(dataMatrix, xPos, yPos, 0, -1, roomMatrix[xPos, yPos].TopDoor.GetLockState());
                    SetData(dataMatrix, xPos, yPos, 0, 1, roomMatrix[xPos, yPos].BottonDoor.GetLockState());
                }
            }
            // 返回保存迷宫的数组
            return dataMatrix;
        }

        /// <summary>
        /// 根据迷宫的房间的门的开关设置数组的数据
        /// </summary>
        /// <param name="dataMatrix">数据的数组</param>
        /// <param name="xPos">房间的x坐标</param>
        /// <param name="yPos">房间的y坐标</param>
        /// <param name="xOffset">门在迷宫的x方向的偏移</param>
        /// <param name="yOffset">门在迷宫的y方向的偏移</param>
        private void SetData(bool[,] dataMatrix, int xPos, int yPos, int xOffset, int yOffset, bool isClose)
        {
            dataMatrix[xPos * 2 + 1 + xOffset, yPos * 2 + 1 + yOffset] = isClose;
        }

        /// <summary>
        /// 将x与y方向的第奇数格预先填充为真
        /// </summary>
        /// <param name="dataMatrix"></param>
        private void PreFill(bool[,] dataMatrix)
        {
            for (int i = 0; i < dataMatrix.GetLength(0); i += 2)
            {
                for (int j = 0; j < dataMatrix.GetLength(1); j += 2)
                {
                    dataMatrix[i, j] = true;
                }
            }
        }

        /// <summary>
        /// 获取迷宫的二维数组 
        /// </summary>
        /// <returns></returns>
        public bool[,] GetBoolArray()
        {
            return RoomToData();
        }

        /// <summary>
        /// 组成迷宫的基本小房间
        /// </summary>
        private class Room
        {
            // 上下左右的门
            private Door topDoor;
            public Door TopDoor
            {
                get { return topDoor; }
                set { topDoor = value; }
            }

            private Door bottonDoor;
            public Door BottonDoor
            {
                get { return bottonDoor; }
                set { bottonDoor = value; }
            }

            private Door leftDoor;
            public Door LeftDoor
            {
                get { return leftDoor; }
                set { leftDoor = value; }
            }

            private Door rightDoor;
            public Door RightDoor
            {
                get { return rightDoor; }
                set { rightDoor = value; }
            }

            // 左上 右上 左下 右下的门
            private Door topLeftDoor;
            public Door TopLeftDoor
            {
                get { return topLeftDoor; }
                set { topLeftDoor = value; }
            }

            private Door topRightDoor;
            public Door TopRightDoor
            {
                get { return topRightDoor; }
                set { topRightDoor = value; }
            }

            private Door bottonLeftDoor;
            public Door BottonLeftDoor
            {
                get { return bottonLeftDoor; }
                set { bottonLeftDoor = value; }
            }

            private Door bottonRightDoor;
            public Door BottonRightDoor
            {
                get { return bottonRightDoor; }
                set { bottonRightDoor = value; }
            }

            /// <summary>
            /// 实例化一个迷宫小房间， 实例化8个方向的门
            /// </summary>
            public Room()
            {
                topDoor = new Door();
                bottonDoor = new Door();
                leftDoor = new Door();
                rightDoor = new Door();

                topLeftDoor = new Door();
                topRightDoor = new Door();
                bottonLeftDoor = new Door();
                bottonRightDoor = new Door();
            }
        }

        /// <summary>
        /// 迷宫的基本小房间的门
        /// </summary>
        private class Door
        {
            /// <summary>
            /// 门是否是关闭的
            /// </summary>
            private bool isLocked;
            /// <summary>
            /// 是否为固定的门(固定的门无法开启和关闭)
            /// </summary>
            private bool isFixed;
            public bool IsFixed
            {
                get { return isFixed; }
                set { isFixed = value; }
            }
            /// <summary>
            /// 构造函数， 默认为门是锁着的
            /// </summary>
            public Door()
            {
                isLocked = true;
            }
            /// <summary>
            /// 开门
            /// </summary>
            public void OpenTheDoor()
            {
                isLocked = false;
            }
            /// <summary>
            /// 关门
            /// </summary>
            public void CloseTheDoor()
            {
                isLocked = true;
            }
            /// <summary>
            /// 获取门是否是锁着的
            /// </summary>
            public bool GetLockState()
            {
                return isLocked;
            }
        }
    }
}