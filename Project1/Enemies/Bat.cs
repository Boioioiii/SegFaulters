using Microsoft.Xna.Framework;
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
using System.Diagnostics;

namespace Project1
{
	public class Bat : IEntity
	{
       

        public Rectangle BoundingBox => getPositionAndRectangle();

        private ISprite sprite;
        
        /*
         * Initalize Bat Features
         */
        public Bat((int, int) position, (String, int)[] items)
		{
            sprite = EnemySpriteFactory.Instance.CreateBatSprite(position, items);

        }

        /*
        * Update the Bat
        */
        public void Update()
        {
            sprite.Update();
           
        }

        /*
         * Draw the Bat
         */
        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
    
        }

        /*
         * Have Bat Attack
         */
        public void Attack()
        {
           //Attacks
        }

        /*
         * Have the Bat drop a Item
         */
        public void ItemDrop()
        {
            //Items they drop
        }

        //fix later
        private Rectangle getPositionAndRectangle()
        {
            return sprite.GetRectangle().Item2;

        }

        public void setPosition(int x, int y)
        {
           sprite.setPos(x, y);

        }
    }
}

