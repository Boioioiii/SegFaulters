using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Enemies;
using static Project1.Constants;

namespace Project1
{
    public class DogMonsterSprite : ISprite
    {
        private Texture2D[] Texture;

        public static bool newAttack;
        private IDirectionStateManager direction_state_manager;
        private IAnimation animation_manager;
        private ITime time_manager;
        private IMove movement_manager;

        private (Rectangle, Rectangle) rectangles;
        private IEntityState state_manager;

        private IWeapon weapon;

        public DogMonsterSprite(Texture2D[] spriteSheet)
		{
            newAttack = true;

            Texture = spriteSheet;

            //replace starting direction based on lvl loader info
            direction_state_manager = new DirectionStateEnemy(Direction.Up);
            time_manager = new TimeTracker(false);
            animation_manager = new Animation(0, DM_TOTAL, time_manager, direction_state_manager);
            state_manager = new EntityState();
            //PARM VALUES WILL CHANGE BASED ON ROOM LOADER
            movement_manager = new Movement(direction_state_manager, this, time_manager, SPRITE_X_START, SPRITE_Y_START, 0);
        }

        /*
         * Update Boss's animation and movement
         */
        public void Update()
        {
            if (state_manager.IsAlive())
            {
                Attack();
                Move();
            }

            UpdateFrames();
        }

        /*
         * Update Boss's frames
         */
        public void UpdateFrames()
        {
            animation_manager.Animate();
        }

        public void Attack()
        {
            //Check if enemy is currently attacking:

            if (!state_manager.IsAttacking())
            {
                if (time_manager.checkIfAttackTime())
                {
                    state_manager.setIsAttacking(true);
                    state_manager.setIsMoving(false);
                    state_manager.setNewAttack(true);
                    EntityAttackAction();
                }
            }
        }

        private void EntityAttackAction()
        {
            //we can assume that we are here bc it is attacking
            //Boomerange
            //check if we need to start a new attack
            
            if (state_manager.startNewAttack())
            {
                //create weapons
                state_manager.setNewAttack(false);
            }
            else
            {
                // means the entity is waiting for the weapon to comeback
                //get weapon obj status of returned/finished) : place holder using state_isAttacking for now
                //get weapon status of "finished" which means we need to be able to get access to the weapon obj
                if (state_manager.IsAttacking())
                {//if weapon is finished and returned to enetity
                    //set move to true
                    state_manager.setIsMoving(true);
                    //set isAttacking to false;
                    state_manager.setIsAttacking(false);
                }
                //dont do anything otherwise
            }
        }


        /*
         * Move the boss
         */
        public void Move()
        {
            if (!state_manager.IsAttacking())
            {
                //Movement will be fixed
                movement_manager.WanderMove();
            }
        }

        /*
         * Draw the Boss
         */
        public void Draw(SpriteBatch spriteBatch)
        {
            setRectangles();
            spriteBatch.Draw(Texture[animation_manager.getCurrentFrame()], rectangles.Item2, rectangles.Item1, Color.White);
        }

        public void setRectangles()
        {
            int x = movement_manager.getPosition().Item1;
            int y = movement_manager.getPosition().Item2;
            int height = Texture[animation_manager.getCurrentFrame()].Height;
            int width = Texture[animation_manager.getCurrentFrame()].Width;
            rectangles.Item1 = new Rectangle(1, 1, width, height);
            rectangles.Item2 = new Rectangle(x, y, width, height);
        }


        public void setPos(int x, int y)
        {
            movement_manager.setPosition(x, y);
        }

        public (int, int) getPos()
        {
            return movement_manager.getPosition();
        }

        public (Rectangle, Rectangle) GetRectangle()
        {
            return rectangles;
        }
    }
}




////Factor out into Manager manager
//public void ChangeDirectionX()
//{
//    this.Direction = Direction * -1;
//    if (WalkLeft)
//    {
//        WalkLeft = false;
//        START_FRAME = 4;
//        total_frame = 6;
//    }
//    else
//    {
//        WalkLeft = true;
//        START_FRAME = 6;
//        total_frame = 8;
//    }

//    current_frame = START_FRAME;
//}

////Factor out into Direction Manager
//public void ChangeDirectionY()
//{
//    this.Direction = Direction * -1;
//    if (WalkDown)
//    {
//        WalkDown = false;
//        START_FRAME = UP_DIRECTION_SPRITE;
//        total_frame = 4;
//    }
//    else
//    {
//        WalkDown = true;
//        START_FRAME = DOWN_DIRECTION_SPRITE;
//        total_frame = 2;
//    }

//    current_frame = START_FRAME;
//}