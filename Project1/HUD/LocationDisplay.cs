﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static System.Formats.Asn1.AsnWriter;
using static Project1.Constants;
using static Project1.Game1;
//using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Project1.HUD
{
	public class LocationDisplay : IHUDEntity
    {
        //Some of these should be static and aren't
        private Texture2D titleRect;
        private Texture2D positionRect;
        private Vector2 coordTitle;
        private Vector2 coordMap;
        private float fullMenuOffset = (SCREEN_HEIGHT / 3) * 2;
        private bool reset = false;
        private SpriteFont font;
        private int titleOffset;
        private int activeRoomNumber;
        private Rectangle mapDestination;
        private Texture2D mapSprite;
        private Rectangle positionDestination;
        private static int mapRowHeight = (HUD_HEIGHT - 100) / 6;
        private static int mapStartY = 103;
        private static int roomWidth = 42;
        private static int startingRoomX = (HUD_SECTION_WIDTH / 3) + 5;
        private static int totalRooms = 18;
        private int startingRoomY = mapStartY + (5 * mapRowHeight);
        //holds the coordinates of the active green box for each room
        public static Vector2[] positionCoords;

        public LocationDisplay(GraphicsDevice graphics, ContentManager content)
        {
            Vector2[] tempPositionCoords = { new Vector2(startingRoomX, startingRoomY), new Vector2(startingRoomX - roomWidth, startingRoomY), new Vector2(startingRoomX + roomWidth, startingRoomY), new Vector2(startingRoomX, startingRoomY - mapRowHeight), new Vector2(startingRoomX, startingRoomY - (2 * mapRowHeight)), new Vector2(startingRoomX + roomWidth, startingRoomY - (2 * mapRowHeight)), new Vector2(startingRoomX - roomWidth, startingRoomY - (2 * mapRowHeight)), new Vector2(startingRoomX - roomWidth, startingRoomY - (3 * mapRowHeight)), new Vector2(startingRoomX, startingRoomY - (3 * mapRowHeight)), new Vector2(startingRoomX, startingRoomY - (4 * mapRowHeight)), new Vector2(startingRoomX, startingRoomY - (5 * mapRowHeight)), new Vector2(startingRoomX - roomWidth, startingRoomY - (5 * mapRowHeight)), new Vector2(startingRoomX + roomWidth, startingRoomY - (3 * mapRowHeight)), new Vector2(startingRoomX + (2 * roomWidth) + 5, startingRoomY - (3 * mapRowHeight)), new Vector2(startingRoomX + (2 * roomWidth) + 5, startingRoomY - (4 * mapRowHeight)), new Vector2(startingRoomX + (3 * roomWidth) + 5, startingRoomY - (4 * mapRowHeight)), new Vector2(startingRoomX + (3 * roomWidth) + 5, startingRoomY - (4 * mapRowHeight)), new Vector2(startingRoomX - (2 * roomWidth), startingRoomY - (3 * mapRowHeight))};
            positionCoords = tempPositionCoords;

            font = content.Load<SpriteFont>("HUDFont");
            
            activeRoomNumber = 0;

            titleRect = new Texture2D(graphics, HUD_SECTION_WIDTH, HUD_HEIGHT / 3);
            positionRect = new Texture2D(graphics, 1, 1);
            positionRect.SetData(new[] { Color.Green });

            //PLease keep the following comment for future debugging purposes
            //titleOffset = (HUD_SECTION_WIDTH - (int)font.MeasureString("-Room " + (activeRoomNumber + 1) + "-").X) / 2;
            //or:
            titleOffset = (HUD_SECTION_WIDTH - (int)font.MeasureString("-Level 1-").X) / 2;

            coordTitle = new Vector2(titleOffset, 0);
            coordMap = new Vector2(0, titleRect.Height);
            mapSprite = content.Load<Texture2D>(assetName: "RoomSprite");
            mapDestination = new Rectangle((int)coordMap.X, (int)coordMap.Y, HUD_SECTION_WIDTH, HUD_HEIGHT - titleRect.Height);

            positionDestination = new Rectangle((int)positionCoords[activeRoomNumber].X, (int)positionCoords[activeRoomNumber].Y, mapDestination.Width / 8, mapDestination.Height / 10);

        }

        public void Update()
        {
            //update indicator based on which room we're in
            activeRoomNumber = RoomManager.GetCurrentRoomIndex() % totalRooms;
            positionDestination.X = (int)positionCoords[activeRoomNumber].X;
            positionDestination.Y = (int)positionCoords[activeRoomNumber].Y;
            //debug why we have to add mapDestination and positionDestination later
            if (HeadsUpDisplay.IsFullMenuDisplay())
            {
                coordTitle.Y += fullMenuOffset;
                coordMap.Y += fullMenuOffset;
                mapDestination.Y += (int)fullMenuOffset;
                startingRoomY += (int)fullMenuOffset;
                positionDestination.Y += (int)fullMenuOffset;
                reset = true;
            } else if(reset)
            {
                coordTitle.Y -= fullMenuOffset;
                coordMap.Y -= fullMenuOffset;
                startingRoomY -= (int)fullMenuOffset;
                mapDestination.Y -= (int)fullMenuOffset;
                positionDestination.Y -= (int)fullMenuOffset;
                reset = false;
            }
            
        }

        //Keeping this in case bugs are found with GetCurrentRoomIndex implementation
        /*
        private void setRoomNum()
        {
            Dictionary<int, bool> roomList = RoomManager.GetActiveList();
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i])
                {
                    activeRoomNumber = i % totalRooms;
                    //just to avoid doing uneccessary iterations:
                    i = roomList.Count;
                }
            }
        }
        */

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw title
            //PLease keep the following comment for future debugging purposes
            //spriteBatch.DrawString(font, "-Room " + (activeRoomNumber + 1) + "-", coordTitle, Color.White);
            //or:
            spriteBatch.DrawString(font, "-Level 1-", coordTitle, Color.White);

            //draw entire map
            spriteBatch.Draw(mapSprite, mapDestination, Color.Blue);
            //draw location indicator
            spriteBatch.Draw(positionRect, positionDestination, Color.Green);
        }
    }
}

