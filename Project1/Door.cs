﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Project1.Constants;

namespace Project1
{
    public class Door : IEnvironment
    {
        public DIRECTION direction { get; private set; }
        public int destinationRoom { get; private set; }
        private bool isLocked;
        private Texture2D texture;
        private int xPos;
        private int yPos;
        private int width;
        private int height;

        public Rectangle BoundingBox { get; private set; }
        public Door(Texture2D[]textures, DIRECTION direction, int destinationRoom, bool isLocked)
        {           
            this.destinationRoom = destinationRoom;
            this.isLocked = isLocked;
             
            switch (direction)
            {
                //set respective scaling, and texture for the door depending on the direction, and if its locked or not.
                case DIRECTION.up:
                    width = 100; height = 60;
                    if (isLocked)
                        texture = textures[4];
                    else
                        texture = textures[3];
                    xPos = 350;
                    yPos = 14 + FRAME_BUFFER;
                    this.direction = DIRECTION.up;
                    break;
                case DIRECTION.down:
                    width = 100; height = 60;
                    if (isLocked)
                        texture = textures[5];
                    else
                        texture = textures[2];
                    xPos = 352;
                    yPos = 409 + FRAME_BUFFER;
                    this.direction = DIRECTION.down;
                    break;
                case DIRECTION.left:
                    width = 60; height = 100;
                    if (isLocked)
                        texture = textures[6];
                    else
                        texture = textures[0];
                    xPos = 53;
                    yPos = 190 + FRAME_BUFFER;
                    this.direction = DIRECTION.left;
                    break;
                case DIRECTION.right:
                    width = 63; height = 107;
                    if (isLocked)
                        texture = textures[7];
                    else
                        texture = textures[1];
                    xPos = 688;
                    yPos = 190 + FRAME_BUFFER;
                    this.direction = DIRECTION.right;
                    break;
                default:
                    break;
            }
        }
        

        public void Update() { }
        public void Draw(SpriteBatch spriteBatch) {
            double ratio = 1.3;
            BoundingBox = new Rectangle(xPos, yPos, width, height);
            //System.Diagnostics.Debug.WriteLine("Direction: " + this.direction + " Width: " + (int)(texture.Width * ratio) + "  Height: " + (int)(texture.Height * ratio));
            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, width, height), Color.White);

        }

        public bool isDoorLocked()
        {
            return isLocked;
        }
    }
}
