﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Project1.SmartAI;

namespace Project1
{
    public interface IActiveObjects
    {

        public List<IItem> getItemList();
        public List<IEntity> getEntityList();
        public List<IWeapon> getWeaponList();
        public List<IEnvironment> getEnvironmentList();
        public List<Door> getDoorList();
        public List<Rectangle> getBoundarys();
        public List<IWeapon> getPlayerWeaponList();
        public List<IWeapon> getDetectionWeaponsList();
        public List<IEntity> getDetectionEntityList();


        public void addNewItem(IItem item);
        public void addNewEntity(IEntity entity);
        public void addNewWeapon(IWeapon weapon);
        public void addNewPlayerWeapon(IWeapon weapon);
        public void addNewWall(Rectangle wall);
        public void addNewEnvironment(IEnvironment block);
        public void addDoors(Door door);
        public void addNewDetectionWeapon(IWeapon weapon);
        public void addNewDetectionEntity(IEntity weapon);


        public void removeItem(IItem item);
        public void removeDetectionWeapon(IWeapon item);
        public void removeEntity(IEntity entity);
        public void removeWeapon(IWeapon weapon);
        public void removePlayerWeapon(IWeapon weapon);
        public void removeDetectionEntity(IEntity entity);



        public void clearAll();
        public void RemoveDead();
        public void ClearRemovingLists();
        public void ClearAddingLists();
        public void setAllObjects();

        

    }
}
