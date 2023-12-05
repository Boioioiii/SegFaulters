﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;
using static Project1.Constants;

namespace Project1
{
    public class MapSprite : IItemSprite
    {
        //Stores all frames for the item
        private Texture2D[] Texture { get; set; }

        //curremtFrame is used to keep track of which frame of the animation we are currently on
        private int current_frame { get; set; }

        //position specifed by XML
        private int pos_x { get; set; }
        private int pos_y { get; set; }

        //bounding box
        private Rectangle rect;

        //dimentions of sprite frame
        private int width;
        private int height;

        public MapSprite(Texture2D[] spriteSheet, (int, int) pos)
        {
            pos_x = pos.Item2;
            pos_y = pos.Item2;
            Texture = spriteSheet;
            current_frame = START_FRAME;

        }

        public void Update()
        {
            //non animated so no code goes here
        }


        private void setDimention()
        {

            width = Texture[(int)current_frame].Width;
            height = Texture[(int)current_frame].Height;
        }

        // draw inside Link's inventory
        public void Draw(SpriteBatch spriteBatch)
        {
            setDimention();
            Rectangle SOURCE_REC = new Rectangle(0, 0, width, height);
            Rectangle DEST_REC = new Rectangle(pos_x, pos_y, width + 10, height + 10);
            rect = DEST_REC;
            spriteBatch.Draw(Texture[(int)current_frame], DEST_REC, SOURCE_REC, Color.White);
        }

        // draw inside level loader
        public void Draw(SpriteBatch spriteBatch, Vector2 location, int scale)
        {
            setDimention();
            Rectangle SOURCE_REC = new Rectangle(0, 0, width, height);
            Rectangle DEST_REC = new Rectangle((int)location.X, (int)location.Y, width * scale, height * scale);
            rect = DEST_REC;
            spriteBatch.Draw(Texture[(int)current_frame], DEST_REC, SOURCE_REC, Color.White);
        }
        public Rectangle getRect()
        {
            return rect;
        }

    }
}





