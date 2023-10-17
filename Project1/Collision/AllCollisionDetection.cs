﻿using Microsoft.Xna.Framework;
using Project1.Collision_Response;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Project1.Constants;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project1.Collision
{
    internal class AllCollisionDetection
    {
        // TODO: IMPLEMENT BOUNDING BOXES FOR INTERFACES AND CLASSES INHERTING THEM

        /*
         * Detect every object the player/enemies could be colliding with
         * For each player/enemy, detect if colliding with list of colliders
         * Walls, doors, enemies, items, etc.
         * Knock player back if collision is a wall or enemy (calculated in DirectionalCollsionDetection.cs, handled in collision response class)
         * Additional optimizations might be helpful (like a quadtree)
         */

        // player and enemy rects, the latter is a list
        List<IEnemy> enemyRects = new List<IEnemy>();
        //List<Rectangle> enemyRects = new List<Rectangle>();

        Player link;

        #region Collision rectangles TODO: USE ENTITIES INSTEAD
        // TODO: ROOM MANAGER MUST GENERATE THESE LISTS ON NEW ROOM LOAD TO PASS INTO COLLISION
        /*
         * List of rectangles for collision boxes for Link
         * Includes: Items, doors, boundaries, enemies, and non-Link damagers (attacks and enemies without health)
         */
        List<Rectangle> roomCollisionRectsForLink = new List<Rectangle>();

        /*
         * List of rectangles for collision boxes for enemies
         * Includes: Boundaries and Link's weapons 
         */
        List<Rectangle> roomCollisionRectsForEnemies = new List<Rectangle>();
        #endregion

        #region Collision Entity Lists
        /*
         * List of rectangles for collision boxes for Link
         * Includes: Items, doors, boundaries, enemies, and non-Link damagers (attacks and enemies without health)
         */
        List<IItem> roomItems = new List<IItem>();       
        List<IDoor> roomDoors = new List<IDoor>();
        List<IEnvironment> roomBoundaries = new List<IEnvironment>();
        List<IEnemy> roomEnemies = new List<IEnemy>();
        //TODO: change to interface
        List<Rectangle> enemyAttackInstances = new List<Rectangle>();

        /*
         * List of rectangles for collision boxes for enemies
         * Includes: Boundaries and Link's weapons 
         * 
         * TODO: Differentiate between whether weapons belong to Link or an enemy
         */
        List<IWeaponMelee> weaponMelees = new List<IWeaponMelee>();
        List<IWeaponProjectile> weaponProjectiles = new List<IWeaponProjectile>();
        #endregion

        #region Collision Detection Entities (player & room enemies)
        /*
         * Detect every object the player could be colliding with
         * pass in list of axis-alligned bounding rectangles
         * Additional directional collision information required for enemies, attacks and boundaries         
         */
        public void DetectAllCollisionsLinkEntity(CollisionType collisionType, Player link)
        {
            bool isColliding = false;

            // check if player intersects a room rectangle
                
            // if the CollisionType is damage or boundary, directional collision check required
            Enum collisionDirection = DIRECTION.left;
                    
            foreach (var item in roomItems)
            {
                isColliding = link.BoundingBox.Intersects(item.BoundingBox);
                if (isColliding)
                {
                    PlayerCollisionResponse.ItemResponse(item);
                }                  
            }
            foreach (var door in roomDoors)
            {
                isColliding = link.BoundingBox.Intersects(door.BoundingBox);
                if (isColliding)
                {
                    PlayerCollisionResponse.DoorResponse(door);
                }               
            }
            foreach (var boundary in roomBoundaries)
            {
                isColliding = link.BoundingBox.Intersects(boundary.BoundingBox);
                if (isColliding)
                {
                    DetectCollisionDirection(link.BoundingBox, boundary.BoundingBox, collisionDirection);
                    PlayerCollisionResponse.BoundaryResponse(link, collisionDirection);
                }
            }
            foreach (var enemy in roomEnemies)
            {
                isColliding = link.BoundingBox.Intersects(enemy.BoundingBox);
                if (isColliding)
                {
                    DetectCollisionDirection(link.BoundingBox, enemy.BoundingBox, collisionDirection);
                    PlayerCollisionResponse.DamageResponse(link, collisionDirection);
                }
            }
            foreach (var enemyAttack in enemyAttackInstances)
            {
                isColliding = link.BoundingBox.Intersects(enemyAttack);
                if (isColliding)
                {
                    DetectCollisionDirection(link.BoundingBox, enemyAttack, collisionDirection);
                    PlayerCollisionResponse.DamageResponse(link, collisionDirection);
                }
            }
            /*
            * only one collision per frame 
            * if object detects a collision, no more collisions allowed for that frame
            * might be worth removing
            */
            //if (isColliding) { break; }
        }

        public void DetectAllCollisionsEnemiesEntity(CollisionType collisionType)
        {
            // pass in list of axis-alligned bounding rectangles

            bool isColliding = false;

            /*
             * additional directional collision information required for enemies, attacks and boundaries
             */
            foreach (var enemy in enemyRects)
            {
                foreach (var boundary in roomBoundaries)
                {
                    isColliding = enemy.BoundingBox.Intersects(boundary.BoundingBox);
                    if (isColliding)
                    {
                        Enum collisionDirection = DIRECTION.left;
                        DetectCollisionDirection(enemy.BoundingBox, boundary.BoundingBox, collisionDirection);

                        EnemyCollisionResponse.BoundaryResponse(enemy, collisionDirection);
                    }
                }
                foreach (var weaponMelee in weaponMelees)
                {
                    isColliding = enemy.BoundingBox.Intersects(weaponMelee.BoundingBox);
                    if (isColliding)
                    {
                        Enum collisionDirection = DIRECTION.left;
                        DetectCollisionDirection(enemy.BoundingBox, weaponMelee.BoundingBox, collisionDirection);

                        EnemyCollisionResponse.DamageResponse(enemy, collisionDirection);
                    }
                }
                foreach (var weaponProjectile in weaponProjectiles)
                {
                    isColliding = enemy.BoundingBox.Intersects(weaponProjectile.BoundingBox);
                    if (isColliding)
                    {
                        Enum collisionDirection = DIRECTION.left;
                        DetectCollisionDirection(enemy.BoundingBox, weaponProjectile.BoundingBox, collisionDirection);

                        EnemyCollisionResponse.DamageResponse(enemy, collisionDirection);
                    }
                }
            }
        }
        #endregion

        /*
         * Change collisionDirection enum to correct direction
         * 
         * Detect collision (Rectangle intersect test)
         * Determine whether distance between the rectangles is positive or negative
         * Get overlap rect
         * If overlap rect's width > height, it's a top/bottom collision, otherwise left/right
         * Use positive or negative direction to determine which side
         */
        void DetectCollisionDirection(Rectangle targetRect, Rectangle roomRect, Enum collisionDirection)
        {
            Rectangle overlap = new Rectangle();            
            Rectangle.Intersect(ref targetRect, ref roomRect, out overlap);

            bool positiveDirection = Vector2.Distance(new Vector2(targetRect.Center.X, targetRect.Center.Y), new Vector2(roomRect.Center.X, roomRect.Center.Y)) > 0;

            if (overlap.Width > overlap.Height)
            {
                // top/bottom collision
                if (positiveDirection)
                {
                    // bottom collision
                    collisionDirection = DIRECTION.down;
                } 
                else
                {
                    // top collision
                    collisionDirection = DIRECTION.up;
                }
            }
            else
            {
                // left right collision
                if (positiveDirection)
                {
                    // left collision
                    collisionDirection = DIRECTION.left;
                }
                else
                {
                    // right collision
                    collisionDirection = DIRECTION.right;
                }
            }
        }

        #region OUTDATED, USES RECTS NOT ENTITIES! Collision Detection (player & room enemies)
        public void DetectAllCollisionsLink(CollisionType collisionType, Rectangle link)
        {
            // pass in list of axis-alligned bounding rectangles

            bool isColliding = false;

            /*
             * additional directional collision information required for enemies, attacks and boundaries
             */

            foreach (var roomRect in roomCollisionRectsForLink)
            {
                // check if player/enemy intersects a room rectangle
                isColliding = link.Intersects(roomRect);
                /*
                // if yes
                if (isColliding)
                {
                    // if the CollisionType is damage or boundary, directional collision check required
                    Enum collisionDirection = DIRECTION.left;

                    switch (collisionType)
                    {
                        case CollisionType.ITEM:
                            // TODO: change to entity
                            PlayerCollisionResponse.ItemResponse();
                            break;
                        case CollisionType.DOOR:
                            // TODO: get correct door transition
                            PlayerCollisionResponse.DoorResponse();
                            break;
                        case CollisionType.BOUNDARY:
                            DetectCollisionDirection(link, roomRect, collisionDirection);
                            // TODO: Pass in player entity
                            PlayerCollisionResponse.BoundaryResponse(collisionDirection);
                            break;
                        case CollisionType.DAMAGE:
                            DetectCollisionDirection(link, roomRect, collisionDirection);
                            // TODO: Pass in player entity
                            PlayerCollisionResponse.DamageResponse(collisionDirection);
                            break;
                    }
                }
                */
                /*
                * only one collision per frame 
                * if object detects a collision, no more collisions allowed for that frame
                * might be worth removing
                */
                //if (isColliding) { break; }
            }

        }

        public void DetectAllCollisionsEnemies(CollisionType collisionType)
        {
            // pass in list of axis-alligned bounding rectangles

            //bool isColliding = false;

            /*
             * additional directional collision information required for enemies, attacks and boundaries
             */
            //foreach (var enemy in enemyRects)
            {
                //foreach (var roomRect in roomCollisionRectsForEnemies)
                {
                    /*
                    // check if player/enemy intersects a room rectangle
                    isColliding = enemy.Intersects(roomRect);

                    // if yes
                    if (isColliding)
                    {
                        Enum collisionDirection = DIRECTION.left;
                        switch (collisionType)
                        {
                            case CollisionType.BOUNDARY:
                                DetectCollisionDirection(enemy, roomRect, collisionDirection);
                                // TODO: Pass in enemy entity
                                EnemyCollisionResponse.BoundaryResponse(collisionDirection);
                                break;
                            case CollisionType.DAMAGE:
                                DetectCollisionDirection(enemy, roomRect, collisionDirection);
                                // TODO: Pass in enemy entity
                                EnemyCollisionResponse.DamageResponse(collisionDirection);
                                break;
                        }
                    }
                    */
                    /*
                     * only one collision per frame 
                     * if object detects a collision, no more collisions allowed for that frame
                     * might be worth removing
                     */
                    //if (isColliding) { break; }
                }

            }
        }
        #endregion
    }
}
