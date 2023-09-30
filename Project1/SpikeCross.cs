﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Collections;
using static Project1.Constants;
namespace Project1
{
	public class SpikeCross : IEnemy
	{
        //Texture stores the texture alias for our animation
        private Texture2D Texture { get; set; }

        //rows is the number of rows i the texture alias
        private int Rows { get; set; }

        //Columns is the number of columns in the alias
        private int Columns { get; set; }

        //curremtFrame is used to keep track of which frame of the animation we are currently on
        private double CURRENT_FRAME;

        //totalFrames keeps track of how many frames there are in total
        private int TOTAL_FRAME;

        //Remove later-------
        private Game1 GameObject;
        private ContentManager ContentLoad;

        //Remove later above--------------



        //Later create a animation tracker class:
        private int WIDTH;
        private int HEIGHT;
        private int ROW;
        private int COL;

        //Movement:
        private int POS_X;
        private int POS_Y;

        private ISprite sprite;
        public SpikeCross()
		{

            //remove later:
            /*
            GameObject = Constants.GameObj;
            ContentLoad = GameObject.Content;
            Texture = Load();
            POS_X = SPRITE_X;
            POS_Y = SPRITE_Y;
            */
            sprite = EnemySpriteFactory.Instance.CreateSpikeCrossSprite();
        }
       

        public void Update()
        {
            sprite.Update();
            /*
            Move();
            CURRENT_FRAME += FRAME_SPD;
            if (CURRENT_FRAME >= TOTAL_FRAME)
                CURRENT_FRAME = START_FRAME;
            */
        }
        private void Animate()
        {

            //WIDTH = Texture.Width / Columns;
            //HEIGHT = Texture.Height / Rows;

            //ROW = (int)CURRENT_FRAME / Columns;
            //COL = (int)CURRENT_FRAME % Columns;

        }
        public void Draw(SpriteBatch spriteBatch)
        {

            sprite.Draw(spriteBatch);
        }

        /*
         * Responsible for setting the the frame numbers(r,c) of the sprite
         */
        private void setFrames()
        {
            //Rows = SPIKE_R;
            //Columns = SPIKE_C;
            //CURRENT_FRAME = START_FRAME;
            //TOTAL_FRAME = Rows * Columns;
        }

        /*
         * Responsible for loading the sprite image
         */
        public Texture2D Load()
        {
            //setFrames();
            return ContentLoad.Load<Texture2D>(assetName: "SPIKES");
        }

        public void Move()
        {
           
        }

        public void Health()
        {
            throw new NotImplementedException();
        }

        public void Attack()
        {
            throw new NotImplementedException();
        }

        public void ItemDrop()
        {
            throw new NotImplementedException();
        }
    }
}

