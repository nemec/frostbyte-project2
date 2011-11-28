﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Frostbyte.Obstacles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Frostbyte.Levels
{
    internal class WorldMap
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static int visited = 0;

        private static List<Vector2> LevelPositions = null;

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;
            LevelInitTime = TimeSpan.MinValue;

            This.Game.AudioManager.AddBackgroundMusic("Music/WorldMapBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/WorldMapBG", 0.1f);

            if (visited == 0)
            {
                LevelPositions = new List<Vector2>(new Vector2[]{
                    new Vector2(530, 170),
                    new Vector2(815, 238),
                    new Vector2(525, 570),
                    new Vector2(170, 445),
                    new Vector2(525, 406)
                });
            }

            Sprite s = new Sprite("map", new Actor(l.GetAnimation("WorldMap.anim")));
            s.ZOrder = int.MinValue;

            /*Text title = new Text("titletext", "text", visited.ToString());
            title.CenterOn(new Vector2(v.Width / 2, 100));
            title.DisplayColor = Color.Chartreuse;*/

            context.GetTexture("regen");
            ConcentricCircles c = new ConcentricCircles("cc", 75 / 2);
            c.ZOrder = 100;

            if (visited < LevelPositions.Count)
            {
                c.SpawnPoint = LevelPositions[visited];
            }
            visited++;
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            if (LevelInitTime == TimeSpan.MinValue)
            {
                LevelInitTime = gameTime.TotalGameTime;
            }

            if ((This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked)
            {
                // Go to next
                (This.Game.CurrentLevel as FrostbyteLevel).LevelCompleted = true;
            }
        }
    }
}
