﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Project1.Constants;

namespace Project1
{
    internal class SwordSpritePlayer : ISpriteWeapon
    {

        private Texture2D[] texture = new Texture2D[4];
        private int userX;
        private int userY;
        private int weaponX;
        private int weaponY;

        public bool swordPlaced { get; private set; }
        private int direction;
        private int current_frame;
        private int total_frame;

        private int elapsedTime;
        private int fps;

        private int width;
        private int height;

        private int offsetX;
        private int offsetY;

        private Rectangle rec;


        public bool isAttacking;

        public SwordSpritePlayer(Texture2D[] spriteSheet)
        {
            texture = spriteSheet;
            swordPlaced = false;
            total_frame = W_DEFAULT_FRAMES;
            current_frame = 0;

            
            offsetX = 0;
            offsetY = 0;


            GetUserState();
            isAttacking = Player.getPlayerAttackingState();
        }


        /*
         * Arrow has been created
         */
        public void SetSword()
        {
            swordPlaced = true;
        }

        /*
         * To know when sword should be removed from the active objects list
         */
        private void RemoveSword()
        {
            swordPlaced = false;
            
        }


        //ignore
        private void UpdateFrames()
        {

        }

        /*
         * Attack
         */
        public void Attack()
        {
            DetermineWeaponState();
            SetSword();

        }

        /*
         * Get info for weapon direction
         */
        private void DetermineWeaponState()
        {
                GetUserPos();
                GetUserState();
                placeOffset();
                GetUserAttackingState();
        }

        /*
         * Update movement
         */
        public void Update()
        {
            Attack();
        }

        /*
         * inital distiance away from user
         */
        private void placeOffset()
        {
           (int,int) pos = WeaponDirectionMovement.SwordOffSetX(userX,userY, direction);

            weaponX = pos.Item1;
            weaponY = pos.Item2;


        }

        /*
         * Draw
         */
        public void Draw(SpriteBatch spriteBatch)
        {
            width = texture[current_frame].Width;
            height = texture[current_frame].Height;
            Rectangle SOURCE_REC = new Rectangle(1, y: 1, width, height);
            Rectangle DEST_REC = new Rectangle(weaponX,weaponY, width * SWORD_SCALE, height * SWORD_SCALE);
            rec = DEST_REC;
            spriteBatch.Draw(texture[current_frame], DEST_REC, SOURCE_REC, Color.White);
            
        }

        /*
         * Get user x and y at teh momemnt of attk
         */
        private void GetUserPos()
        {
            Vector2 posVec = Player.getUserPos();
            userX = (int)posVec.X;
            userY = (int)posVec.Y;
        }



        /*
         * Get user directions -> change name
         */
        private void GetUserState()
        {
            direction = Player.getUserDirection();
            current_frame = direction;

            //right = 0, left = 1, up = 2, down = 3
            switch (direction)
            {
                case 0:
                    current_frame = FRAME2_RIGHT;
                    break;
                case 1:
                    current_frame = FRAME2_LEFT;
                    break;
                case 2:
                    current_frame = FRAME2_UP;
                    break;
                case 3:
                    current_frame = FRAME2_DOWN;
                    break;
            }
        }

        private void GetUserAttackingState()
        {
            isAttacking = Player.getPlayerAttackingState();
        }
     

        public void GetUserPos(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void GetUserState(Constants.Direction direct)
        {
            throw new NotImplementedException();
        }

        public bool finished()
        {
            if (!isAttacking) return true;
            return false;
        }

        public Rectangle GetRectangle()
        {
            return rec;
        }

        public Rectangle getDetectionFieldRectangle()
        {
            throw new NotImplementedException();
        }

        public void MovementChange(bool detected)
        {
            throw new NotImplementedException();
        }

        public void setTarget(IEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}


