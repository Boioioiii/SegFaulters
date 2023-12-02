﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using static Project1.Constants;



namespace Project1
{
    //reads the XML file and sends info to Room Loader
    public static class LevelLoader
    {
        public static int roomCount;
        private static Room[] roomList;
        private static (string, ((int, int), (string, int)[]))[] enemyArray; //(string enemyName, ((int posX, int posY), (string itemName, int quantity)[]))
        private static (((string, bool), (int, bool))[],(string,(int, int))[]) environmentInfo; //(doorArray, blockArray)
        private static (string, (int, int))[] itemArray;//(string itemName,(int posX, posY))
        public static void Load()
        { 

            XmlDocument xmlDoc = new XmlDocument();

           

            string relativePath = XMLPATH;

            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            xmlDoc.Load(fullPath);

            getRoomCountFromXmlDoc(xmlDoc);
            roomList = new Room[roomCount];

            parseXML(xmlDoc);

            RoomManager.Load();
            
        }
        private static void parseXML(XmlDocument xmlDoc)
        {

            //loop through xml document and sends info of room to the Room instance
            XmlNodeList rooms = xmlDoc.GetElementsByTagName(ROOM);
            int i = 0;
            foreach (XmlNode room in rooms)
            {
                System.Diagnostics.Debug.WriteLine(ROOM + i);

                //all rooms have these three encompassing nodes
                XmlNode enemies = room.SelectSingleNode(ENEMIES);
                XmlNode environment = room.SelectSingleNode(ENVIRONMENT);
                XmlNode items = room.SelectSingleNode(ITEMS_STRING);

                parseEnemies(enemies);
                parseEnvironment(environment);
                parseItems(items);

                roomList[i] = new Room(enemyArray, environmentInfo, itemArray);// enemyArray, environmentInfo, itemArray);
                //roomList[i].print();
                i++;
            }
        }

        //parses enemy info from xml doc, and make enemyArray out of it.
        private static void parseEnemies(XmlNode enemies)
        {
            //get all the enemies needed to be loaded in
 
            XmlNodeList enemyList = enemies.ChildNodes;
            int enemyCount = enemyList.Count;

            enemyArray = new (string, ((int, int), (string, int)[]))[enemyCount];
            int i = 0;
            //if there is enemies, go through each one of them
            foreach (XmlNode enemy in enemyList)
            {
                //get all necessary information for the enemyArray
                string name = enemy.Name;
                int posX = int.Parse(enemy.Attributes[XLOC].Value);
                int posY = int.Parse(enemy.Attributes[YLOC].Value);

                //cycle through dropped items make it into an array
                XmlNodeList droppedItemsList = enemy.SelectSingleNode(DROPS).ChildNodes;
                (string, int)[] droppedItemArray = new (string, int)[droppedItemsList.Count];
                int j = 0;
                foreach(XmlNode droppedItem in droppedItemsList)
                {
                    string itemName = droppedItem.Attributes[NAME].Value;
                    int quantity = int.Parse(droppedItem.Attributes[QUANITY].Value);
                    droppedItemArray[j] = (itemName, quantity);

                    j++;
                }

                //add to the enemy array
                enemyArray[i] = new(name, ((posX, posY), droppedItemArray));
                i++;

               
            }
        }
        private static void parseEnvironment(XmlNode environment)
        {
            XmlNodeList wallList = environment.SelectSingleNode(WALLS).ChildNodes;
            XmlNodeList doorList = environment.SelectSingleNode(DOORS).ChildNodes;
            XmlNodeList blockList = environment.SelectSingleNode(BLOCKS).ChildNodes;

            //so far no implementation for walls needed

            //get door information
            ((string, bool), (int, bool))[] doorArray = new ((string, bool), (int, bool))[doorList.Count];
            int i = 0;
            foreach(XmlNode door in doorList)
            {
                string direction = door.Attributes[ID].Value;
                int destinationRoom = int.Parse(door.SelectSingleNode(DESTINATION_ROOM).InnerText) - 1;
                bool isLocked = bool.Parse(door.SelectSingleNode(LOCKED).InnerText);

                bool isTunnel = false;

                if (door.SelectNodes(IS_TUNNEL).Count > 0)
                {
                    isTunnel = true;
                }

                doorArray[i] = ((direction, isTunnel), (destinationRoom, isLocked));
                i++;
            }

            //get block information
            (string, (int, int))[] blockArray = new (string, (int, int))[blockList.Count];
            int j = 0;
            foreach(XmlNode block in blockList)
            {
                string blockType = block.Name;
                int posX = int.Parse(block.Attributes[XLOC].Value);
                int posY = int.Parse(block.Attributes[YLOC].Value);

                blockArray[j] = (blockType, (posX, posY));
                j++;
            }
            //load info into environmentInfo
            environmentInfo = (doorArray,  blockArray);
        }
        private static void parseItems(XmlNode items)
        {
            XmlNodeList itemList = items.ChildNodes;
            itemArray = new (string, (int, int))[itemList.Count];
            int i = 0;
            foreach (XmlNode item in itemList) {
                string itemName = item.Name;
                int posX = int.Parse(item.Attributes[XLOC].Value);
                int posY = int.Parse(item.Attributes[YLOC].Value);

                itemArray[i] = (itemName, (posX, (posY)));
                i++;
            }
        }

        public static void UnlockDoorFromRoom(int roomNumber, DIRECTION direction)
        {
            roomList[roomNumber].UnlockDoor(direction);
        }
        
        public static void RemoveItem(int roomNumber, IItem item)
        {
            roomList[roomNumber].RemoveItem(item);
        }

        public static void RemoveEnemy(int roomNumber, (int, int) position)
        {
            roomList[roomNumber].RemoveEnemy(position);
        }

        public static void drawActiveRoom(int currentRoomNum, int nextRoomNum, DIRECTION doorDirection)
        {
            Room currentRoom = roomList[currentRoomNum];
            Room nextRoom = roomList[nextRoomNum];

            if (doorDirection != DIRECTION.none) //if not the first load in
            {
                RoomTransition.StartScrolling(nextRoom, nextRoomNum, doorDirection);
            }
            else
            {
                nextRoom.Load();
            }
        }

        private static void getRoomCountFromXmlDoc(XmlDocument doc)
        {
            roomCount = doc.GetElementsByTagName(ROOM).Count;
        }
    }
}
