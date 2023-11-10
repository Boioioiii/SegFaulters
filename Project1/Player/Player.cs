using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using static Project1.Constants;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Project1
{
    public class Player
    {
        public IPlayerState playerState { get; set; }
        public static Rectangle BoundingBox;
        private GraphicsDeviceManager _graphics;
        private static ContentManager Content;
        private static Vector2 position = RESPAWN_UP;

        public static int[] itemInventory = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        //this enum is used to access the invetory by item type:
        //public enum ITEMS { Arrow = 0, Bomb = 1, Boomerang = 2, Bow = 3, Clock = 4, Fairy = 5, Heart = 6, HeartContainer = 7, Key = 8, Map = 9, Rupee = 10, Sword = 11, Triforce = 12 };

        //Needed for link sprite to draw
        private static IPlayerSprite sprite;

        private static bool isMoving = false;

        public static int playerSpeed = 5;

        // cardinal direction player is facing, starts with up on 1 and progresses clockwise (e.g. 4 is left-facing)
        private static int linkDirection = (int)DIRECTION.right;

        /*
        // frames used for still and animation
        public static Texture2D linkRight1;
        public static Texture2D linkLeft1;
        public static Texture2D linkUp1;
        public static Texture2D linkDown1;

        // frames used for animation only
        public static Texture2D linkRight2;
        public static Texture2D linkLeft2;
        public static Texture2D linkUp2;
        public static Texture2D linkDown2;

        // frames used for attacking
        public static Texture2D linkAttackRight;
        public static Texture2D linkAttackLeft;
        public static Texture2D linkAttackUp;
        public static Texture2D linkAttackDown;
        */

        // attacking metrics
        public static bool isAttacking = false;
        private static float AttackTimer;

        // weapons
        private static bool isAttackingWithSword = false;
        private static bool isAttackingWithBoomerang = false;
        private static bool isAttackingWithBow = false;
        public static bool isAttackingWithBomb = false;

        // Check damage cooldown period to get hit again
        private static bool isDamaged = false;
        private static float DamageTimer;

        // Link will flash after damaged, indicating temporary invincibility
        private static bool renderLink = true;
        private static float FlashTimer;

        // how many animation frames per second, not the framerate of the game
        private static float FrameTimer;

        // link only has two frames of animation
        private static bool isSecondFrame = false;

        private static IWeapon[] weaponsArray;
        //private static IWeapon weapon;
        private static int currentWeaponIndex;
        private static IWeapon spriteWeapon;
        private static IActiveObjects objManager;

        private static int onScreen;

        //private static int posX;
        //private static int posY;
        //private Rectangle rect;
        private static bool remainOnScreen;
        private static int damageFlash;
        public Player()
        {

            //posX = 0;
            //posY = 0;
            remainOnScreen = false;
        }

        // initialize timing metrics for attacks and damage
        public static void Initialize()
        {
            FrameTimer = Constants.FRAMETIME;
            AttackTimer = Constants.ATTACK_SECONDS;
            DamageTimer = Constants.INVINCIBILITY_SECONDS;
            FlashTimer = Constants.FLASHTIME;
            damageFlash = 0;
            //IWeapon[] temp = { new Bomb(), new Arrow2(), new Boomerange()};
            //weaponsArray = temp;
            //weaponsArray[0] = new Bomb();
            //weaponsArray[1] = new Arrow2();
            //weaponsArray[2] = new Boomerange();
            //weaponsArray[3] = new Orb();

            objManager = new ActiveObjects();

            sprite = PlayerSpriteFactory.Instance.CreateLinkSprite();

            // weapon = new Bomb();

            BoundingBox = new Rectangle(0, 0, LINK_BOUNDING_DIMENSION, LINK_BOUNDING_DIMENSION);
        }


        public static void LoadContent(ContentManager content)
        {
            //Try getting rid of this and use only sprite factory stuff

            //update this to have only sword
            IWeapon[] temp = { new Bomb(), new Arrow2(), new Boomerange(), new Sword() };
            weaponsArray = temp;

            /*
            // still and movement
            linkRight1 = content.Load<Texture2D>("linkRight1");
            linkLeft1 = content.Load<Texture2D>("linkLeft1");
            linkUp1 = content.Load<Texture2D>("linkUp1");
            linkDown1 = content.Load<Texture2D>("linkDown1");

            // movement only
            linkRight2 = content.Load<Texture2D>("linkRight2");
            linkLeft2 = content.Load<Texture2D>("linkLeft2");
            linkUp2 = content.Load<Texture2D>("linkUp2");
            linkDown2 = content.Load<Texture2D>("linkDown2");

            // Attack using weapon or item
            linkAttackRight = content.Load<Texture2D>("linkAttackRight");
            linkAttackLeft = content.Load<Texture2D>("linkAttackLeft");
            linkAttackUp = content.Load<Texture2D>("linkAttackUp");
            linkAttackDown = content.Load<Texture2D>("linkAttackDown");
            */
        }


        //change the current frame to the next frame
        public static void Update(GameTime gameTime)
        {
            // update timers for attack, damage, flash
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            AttackTimer -= elapsedSeconds;
            DamageTimer -= elapsedSeconds;
            FlashTimer -= elapsedSeconds;

            // set bounding box position to link position
            BoundingBox.Location = new Point((int)position.X + 5, (int)position.Y + 20);
            // move link to bounding box
            //sprite.Update(linkDirection, position);
            // call collision and pass in link

            if (isDamaged)
            {
                DamageInvincibility();
            }



            KeyboardState keystate = Keyboard.GetState();


            #region Print to debug console currently pressed keys
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (var key in keystate.GetPressedKeys())
                sb.Append("Key: ").Append(key).Append(" pressed ");

            if (sb.Length > 0)
            {
                //System.Diagnostics.Debug.WriteLine(sb.ToString());
            }
            else
                isMoving = false;
            #endregion

            // Move our sprite based on arrow keys being pressed:

            if (isAttacking)
            {
                WaitForAttack();
            }


            // Link can't move when attacking
            if (!isAttacking)
            {
                if (keystate.IsKeyDown(Keys.Z) || keystate.IsKeyDown(Keys.N))
                {
                    // attack using his sword
                    isAttacking = true;
                    isAttackingWithSword = true;
                    //currentWeaponIndex = (int)WEAPONS.sword;
                    AudioManager.PlaySoundEffect(sword);
                    //sprite.Update(linkDirection, position);
                }
                else if (keystate.IsKeyDown(Keys.I))
                {
                    // attack using his 
                    isAttacking = true;
                    isAttackingWithBoomerang = true;
                    //currentWeaponIndex = (int)WEAPONS.boom;
                }
                else if (keystate.IsKeyDown(Keys.U))
                {
                    // attack using his 
                    isAttacking = true;
                    isAttackingWithBow = true;
                    //currentWeaponIndex = (int)WEAPONS.bow;
                }
                else if (keystate.IsKeyDown(Keys.T))
                {
                    // attack using his sword
                    isAttacking = true;
                    isAttackingWithBomb = true;
                    //currentWeaponIndex = (int)WEAPONS.bomb;

                    isAttackingWithSword = false;
                    isAttackingWithBoomerang = false;
                    isAttackingWithBow = false;

                }


                /*

                if (keystate.IsKeyDown(Keys.Left) || keystate.IsKeyDown(Keys.A))
                {
                    position.X -= playerSpeed;
                    //room boundary controller, roomBounds var may need to be changed to new values
                    position.X = Math.Clamp(position.X, roomBoundsMinX, roomBoundsMaxX);
                    isMoving = true;
                    linkDirection = (int)DIRECTION.left;

                    //posX -= playerSpeed;


                }
                else if (keystate.IsKeyDown(Keys.Down) || keystate.IsKeyDown(Keys.S))
                {
                    position.Y += playerSpeed;
                    position.Y = Math.Clamp(position.Y, roomBoundsMinY, roomBoundsMaxY);
                    isMoving = true;
                    linkDirection = (int)DIRECTION.down;
                    //posX += playerSpeed;

                }
                else if (keystate.IsKeyDown(Keys.Up) || keystate.IsKeyDown(Keys.W))
                {
                    position.Y -= playerSpeed;
                    position.Y = Math.Clamp(position.Y, roomBoundsMinY, roomBoundsMaxY);
                    isMoving = true;
                    linkDirection = (int)DIRECTION.up;
                    //posY -= playerSpeed;
                }
                else if (keystate.IsKeyDown(Keys.Right) || keystate.IsKeyDown(Keys.D))
                {
                    position.X += playerSpeed;
                    position.X = Math.Clamp(position.X, roomBoundsMinX, roomBoundsMaxX);
                    isMoving = true;
                    linkDirection = (int)DIRECTION.right;
                    //posY += playerSpeed;
                }
                */

                //move method to increment/decrement position depending on direction
            }

            if (keystate.IsKeyDown(Keys.E))
            {
                DamageInvincibility();
            }

            //Move(keystate);

            if (keystate.IsKeyDown(Keys.E))
            {
                isDamaged = true;
                DamageInvincibility();
                AudioManager.PlaySoundEffect(lowHealth);
            }

            if (keystate.IsKeyDown(Keys.T) || keystate.IsKeyDown(Keys.Y))
            {
                // cycle between which block is currently being shown
            }
            if (keystate.IsKeyDown(Keys.U) || keystate.IsKeyDown(Keys.I))
            {

            }
            if (keystate.IsKeyDown(Keys.O) || keystate.IsKeyDown(Keys.P))
            {

            }



            //if (remainOnScreen)
            //{
            //    spriteWeapon.Update();
            //}

        }

        public static Rectangle getPositionAndRectangle()
        {
            return sprite.getRectangle();
        }
        public static void CheckTime()
        {
            onScreen += Game1.deltaTime.ElapsedGameTime.Milliseconds;
            damageFlash += Game1.deltaTime.ElapsedGameTime.Milliseconds;
        }


        public static void CheckOnScreen()
        {
            CheckTime();
            if (onScreen > 1000)
            {
                remainOnScreen = false;
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // timer for Draw()
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameTimer -= elapsedSeconds;

            //CheckOnScreen();

            //manage non persistent weapon
            if (remainOnScreen)
            {
                spriteWeapon.Draw();
            }
            else
            {
                onScreen = 0;
                remainOnScreen = false;
            }

            if (renderLink)
            {
                if (isAttacking)
                {
                    //DrawBasedOnAttackType(spriteBatch);

                    //if (isAttackingWithSword)
                    //{
                    //    //Sword.Draw(position, linkDirection);
                    //    sprite.Draw(spriteBatch, "attack", linkDirection, position);

                    //    /*
                    //    switch (linkDirection)
                    //    {
                    //        //get rid of the switch case since you can just
                    //        //call draw with the direction variable
                    //        case 1:
                    //            Sword.Draw(position, linkDirection);
                    //            sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    //            break;
                    //        case 2:
                    //            Sword.Draw(position, linkDirection);
                    //            sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    //            break;
                    //        case 3:
                    //            Sword.Draw(position, linkDirection);
                    //            sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    //            break;
                    //        case 4:
                    //            Sword.Draw(position, linkDirection);
                    //            sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    //            break;
                    //    }
                    //    */
                    //}
                    ////pass corresponding index of weapon to Attack() method so get
                    ////rid of these if branches
                    //else
                    //{
                    //    //Attack(currentWeaponIndex);

                    //    //sprite.Draw(spriteBatch, "attack", linkDirection, position);

                    //}

                    /*
                    else if (isAttackingWithBoomerang)
                    {
                        Attack((int)WEAPONS.boom);

                        sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    }
                    else if (isAttackingWithBow)
                    {
                        
                        Attack((int)WEAPONS.bow);
                    
                        sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    }
                    else if (isAttackingWithBomb)
                    {
                        Attack((int)WEAPONS.bomb);
                        sprite.Draw(spriteBatch, "attack", linkDirection, position);
                    }
                    */
                }
                else
                {

                    DrawBasedOnMovementType(spriteBatch);
                    /*
                    if (isMoving)
                    {
                        //can be condensed since it should default to still
                        //when its is not on secondFrame && isMoving

                        DrawBasedOnMove();
                        CheckFrameTimer();
                        if (isSecondFrame)
                        {
                            //tell sprite how to draw
                            sprite.Draw(spriteBatch, "move", linkDirection, position);

                        }
                        else
                        {
                            //tell sprite how to draw
                            sprite.Draw(spriteBatch, "still", linkDirection, position);
                        }

                    }
                    else
                    {
                        //tell sprite how to draw
                        sprite.Draw(spriteBatch, "still", linkDirection, position);

                    }
                    */
                }
            }

        }

        //decide which method of drawing link should be used based on his current state
        private static void DrawBasedOnMovementType(SpriteBatch spriteBatch)
        {
            if (isMoving)
            {
                CheckFrameTimer();
                if (isSecondFrame)
                {
                    //tell sprite how to draw
                    sprite.Draw(spriteBatch, "move", linkDirection, position);

                }
                else
                {
                    //tell sprite how to draw
                    sprite.Draw(spriteBatch, "still", linkDirection, position);
                }

            }
            else
            {
                //tell sprite how to draw
                sprite.Draw(spriteBatch, "still", linkDirection, position);

            }


        }

        //Decide which way to go about drawing based on which weapon needs to be drawn
        //on the screen
        private static void DrawBasedOnAttackType(SpriteBatch spriteBatch)
        {
            if (isAttackingWithSword)
            {
                //draw sword sprite according to link's position, does not persist on screen
                DrawNonpersistantWeapon((int)WEAPONS.sword);

            }
            else if (isAttackingWithBoomerang)
            {
                //add weapon to manager so it'll stay on the screen
                spriteWeapon = new Boomerange();
                objManager.addNewWeapon(spriteWeapon);

            }
            else if (isAttackingWithBow)
            {
                // add weapon to manager so it'll stay on the screen
                spriteWeapon = new Arrow2();
                objManager.addNewWeapon(spriteWeapon);

            }
            else if (isAttackingWithBomb)
            {
                // add weapon to manager so it'll stay on the screen
                spriteWeapon = new Bomb();
                objManager.addNewWeapon(spriteWeapon);

            }
            //draw link with attack frames
            sprite.Draw(spriteBatch, "attack", linkDirection, position);
        }


        //Draw a weapon that does not need to be persistent on screen
        public static void DrawNonpersistantWeapon(int weaponType)
        {
            //set spriteWeapon to element in weapon array to get rid of these
            //if branches

            //weapon = weaponsArray[weaponType];


            //spriteWeapon = weaponsArray[weaponType];
            spriteWeapon = new Bomb();
            /*
            switch (weaponType)
            {
                case "bomb":
                    weapon = new Bomb();
                    spriteWeapon = weapon;
                    break;

                case "bow":
                    weapon = new Arrow2();
                    spriteWeapon = weapon;
                    break;
                //TODO:Reimplement later
                //case "boomerange":
                //    weapon = new Boomerange();
                //    spriteWeapon = weapon;
                //    break;
                case "orb":
                    weapon = new Orb();
                    spriteWeapon = weapon;
                    break;

            }
            */

            //remainOnScreen = true;
            //spriteWeapon.Attack();
            //spriteWeapon.Draw();


            switch (weaponType)
            {
                case 0:
                    //spriteWeapon.Attack();
                    Game1.GameObjManager.addNewWeapon(spriteWeapon);
                    break;
                    //case 1:
                    //    Game1.GameObjManager.addNewWeapon(new Arrow2());
                    //    break;
                    //case 2:
                    //    Game1.GameObjManager.addNewWeapon(new Boomerange());
                    //    break;
            }




        }






        // if 1 second has passed since attacking, revert attack keystate to false (allowing for other actions)
        public static void WaitForAttack()
        {
            if (AttackTimer <= 0)
            {
                isAttacking = false;
                AttackTimer = Constants.ATTACK_SECONDS;
                isAttackingWithBoomerang = false;
                isAttackingWithBow = false;
                isAttackingWithSword = false;
            }
        }

        // Link cannot take damage for x seconds after getting hit
        public static void DamageInvincibility()
        {
            //if (DamageTimer <= 0)
            //{
            //    renderLink = true;
            //    isDamaged = false;
            //    DamageTimer = INVINCIBILITY_SECONDS;
            //}
            //else
            //{
            //    if (FlashTimer <= 0)
            //    {
            //        renderLink = !renderLink;
            //        FlashTimer = FLASHTIME;
            //    }
            //}

            if (damageFlash <= 50)
            {
                CheckTime();
                renderLink = false;

            }
            else if (damageFlash > 10)
            {
                renderLink = true;
                isDamaged = false;
                damageFlash = 0;
            }
        }

        // if Timer > FRAMETIME, switch the frame
        public static void CheckFrameTimer()
        {
            if (FrameTimer <= 0)
            {
                isSecondFrame = !isSecondFrame;
                FrameTimer = FRAMETIME;
            }
        }

        //Command Functions
        public static void attackSword()
        {
            isAttacking = true;
            isAttackingWithSword = true;
            //currentWeaponIndex = (int)WEAPONS.sword;
        }

        public static void attackBoom()
        {
            isAttacking = true;
            isAttackingWithBoomerang = true;
            //currentWeaponIndex = (int)WEAPONS.boom;
        }

        public static void attackBow()
        {
            isAttacking = true;
            isAttackingWithBow = true;
            //currentWeaponIndex = (int)WEAPONS.bow;
        }

        public static void attackBomb()
        {
            isAttacking = true;
            isAttackingWithBomb = true;
            //currentWeaponIndex = (int)WEAPONS.bomb;
        }

        public static void left()
        {
            position.X -= playerSpeed;
            //room boundary controller, roomBounds var may need to be changed to new values
            position.X = Math.Clamp(position.X, roomBoundsMinX, roomBoundsMaxX);
            isMoving = true;
            linkDirection = (int)DIRECTION.left;

        }

        public static void down()
        {
            position.Y += playerSpeed;
            //room boundary controller, roomBounds var may need to be changed to new values
            position.Y = Math.Clamp(position.Y, roomBoundsMinY, roomBoundsMaxY);
            isMoving = true;
            linkDirection = (int)DIRECTION.down;
        }

        public static void up()
        {
            position.Y -= playerSpeed;
            //room boundary controller, roomBounds var may need to be changed to new values
            position.Y = Math.Clamp(position.Y, roomBoundsMinY, roomBoundsMaxY);
            isMoving = true;
            linkDirection = (int)DIRECTION.up;
        }

        public static void right()
        {
            position.X += playerSpeed;
            //room boundary controller, roomBounds var may need to be changed to new values
            position.X = Math.Clamp(position.X, roomBoundsMinX, roomBoundsMaxX);
            isMoving = true;
            linkDirection = (int)DIRECTION.right;
        }

        public static void damage()
        {
            isDamaged = true;
            DamageInvincibility();
        }

        public static Vector2 getUserPos()
        {

            return new Vector2(position.X, position.Y);
        }

        public static int getUserDirection()
        {
            return linkDirection;
        }

        public static void PickUpItem(ITEMS itemToAdd)
        {
            itemInventory[(int)itemToAdd]++;
        }

        public static bool UseItem(ITEMS itemToDelete)
        {
            bool didItemGetUsed;
            if (itemInventory[(int)itemToDelete] > 0)
            {
                itemInventory[(int)itemToDelete]--;
                didItemGetUsed = true; //if player has item, it got used.
            }
            else
            {
                didItemGetUsed = false; //if player does not have item, it did not get used.
            }

            return didItemGetUsed;
        }

        public static void setPosition(Vector2 newPostion)
        {
            position = newPostion;
        }

        public static Vector2 getPosition()
        {
            return position;
        }

        public static bool getPlayerAttackingState()
        {
            return isAttacking;
        }

    }
}
