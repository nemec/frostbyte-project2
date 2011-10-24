﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    class Sprite : WorldObject
    {
        internal Sprite(string name, Actor actor)
        {
            mName = name;
            mActor = actor;
            mLastUpdate = new GameTime();

            LoadBehavior = () => { };
            UpdateBehavior = () => { };
            EndBehavior = () => { };

            //adds the sprite to the level
            (This.Game.CurrentLevel != This.Game.NextLevel && This.Game.NextLevel != null ? This.Game.NextLevel : This.Game.CurrentLevel).AddSprite(this);
            Speed = 3;

            if (mActor != null)
            {
                if (mActor.Animations[mActor.CurrentAnimation].Built)
                {
                    if (mActor.Animations[mActor.CurrentAnimation].NumFrames > 1) mAnimating = true;
                }
            }

            Center = new Vector2(GetAnimation().Width / 2, GetAnimation().Height / 2);
        }

        internal Sprite(string name, Actor actor, int collisionlist)
            : this(name, actor)
        {
            CollisionList = collisionlist;
        }

        #region Properties
        /// <summary>
        ///     changes to the specified frame of the animation beginning at 0
        /// </summary>
        internal int Frame { get; set; }

        /// <summary>
        ///     the sprite's speed
        /// </summary>
        internal float Speed { get; set; }

        /// <summary>
        /// State for moving, idling, or attacking.
        /// </summary>
        protected SpriteState State = SpriteState.Idle;

        #endregion Properties

        #region Variables

        protected Vector2 PreviousPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

/// <summary>
        /// Tells whether to animate or not
        /// </summary>
        private bool mAnimating;

        /// <summary>
        /// Tells if the object has been drawn the first time
        /// </summary>
        //private bool mDrawn;

        /// <summary>
        /// Number that indicates when the Sprite'a animation was last updated.
        /// </summary>
        private GameTime mLastUpdate;

        /// <summary>
        /// This Sprite's Actor.
        /// </summary>
        protected Actor mActor;
        #endregion Variables

        #region Behaviors
        /// <summary>
        /// Sprite's Load Behavior
        /// </summary>
        internal Behavior LoadBehavior;

        /// <summary>
        /// Sprite's Update Behavior
        /// </summary>
        internal Behavior UpdateBehavior;

        /// <summary>
        /// Sprite's End Behavior
        /// </summary>
        internal Behavior EndBehavior;
        #endregion Behaviors

        #region Methods

        internal override List<CollisionObject> GetCollision()
        {
            return GetAnimation().CollisionData;
        }

        internal override List<Vector2> GetHotSpots()
        {
            return GetAnimation().HotSpots;
        }

        /// <summary>
        ///     changes to the specified animation beginning at 0
        /// </summary>
        internal SpriteFrame GetAnimation()
        {
            return mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame];
        }

        /// <summary>
        /// changes to the specified animation beginning at 0.
        /// </summary>
        /// <param name="animation">The animation to select (begins at 0)</param>
        internal void SetAnimation(int animation)
        {
            //give us the best one we can
            mActor.CurrentAnimation = mActor.Animations.Count > animation ? animation : mActor.Animations.Count - 1;
            //continue on same frame uncomment to start anim from beginning
            mActor.Frame = mActor.Frame%mActor.Animations[mActor.CurrentAnimation].Frames.Count; 
        }

        /// <summary>
        /// Pauses or resumes an animation.
        /// </summary>
        internal void ToggleAnim() { mAnimating = !mAnimating; }

        /// <summary>
        /// Causes the animation to play.
        /// </summary>
        internal void StartAnim() { mAnimating = true; }

        /// <summary>
        /// Causes the animation to stop.
        /// </summary>
        internal void StopAnim() { mAnimating = false; }

        /// <summary>
        ///Resets the Sprite's animation to the first frame.
        /// </summary>
        internal void Rewind() { mActor.Frame = 0; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Check for collision with background and move enemy out of collision with background until no collisions exist
        /// </summary>
        protected void checkBackgroundCollisions()
        {
            List<Vector2> gridLocations = new List<Vector2>();
            List<CollisionObject> collisionObjects = this.GetCollision();
            TileList map = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;

            foreach (CollisionObject collisionObject in collisionObjects)
            {
                gridLocations.AddRange(collisionObject.GridLocations(this));
            }

            foreach (Vector2 tile in gridLocations)
            {
                Tile output;
                if (map.TryGetValue((int)tile.X, (int)tile.Y, out output))
                {
                    CollisionHelper collisionHelper = new CollisionHelper();
                    collisionHelper.Pos = new Vector2(tile.X * This.CellSize, tile.Y * This.CellSize);
                    switch (output.Type)
                    {
                        case TileTypes.Bottom:
                        case TileTypes.BottomConvexCorner:
                            collisionHelper.bgCollision = new Collision_AABB(0, new Vector2(0.0f, 0.5f * This.CellSize), new Vector2(This.CellSize, This.CellSize));
                            break;

                        case TileTypes.BottomCorner:
                        case TileTypes.Corner:
                        case TileTypes.ConvexCorner:
                        case TileTypes.SideWall:
                        case TileTypes.Wall:
                            collisionHelper.bgCollision = new Collision_AABB(0, new Vector2(0.0f, 0.0f), new Vector2(This.CellSize, This.CellSize));
                            break;

                        default:
                            collisionHelper.bgCollision = null;
                            break;
                    }

                    if (collisionHelper.bgCollision == null)
                    {
                        continue;
                    }

                    foreach (CollisionObject collisionObject in collisionObjects)
                    {
                        if (Collision.DetectCollision(collisionHelper, collisionHelper.bgCollision, this, (dynamic)collisionObject))
                        {
                            Vector2[] testPoints = new Vector2[4];
                            Vector2 direction = Pos - PreviousPos;
                            direction.Normalize();
                            float slope = direction.Y / direction.X;
                            Vector2 centerPos = this.CenterPos;

                            testPoints[0] = new Vector2((collisionHelper.Pos.Y - centerPos.Y) / slope + centerPos.X, collisionHelper.Pos.Y);
                            testPoints[1] = new Vector2((collisionHelper.Pos.Y + This.CellSize - centerPos.Y) / slope + centerPos.X, collisionHelper.Pos.Y + This.CellSize);
                            testPoints[2] = new Vector2(collisionHelper.Pos.X, slope * (collisionHelper.Pos.X - centerPos.X) + centerPos.Y);
                            testPoints[3] = new Vector2(collisionHelper.Pos.X + This.CellSize, slope * (collisionHelper.Pos.X + This.CellSize - centerPos.X) + centerPos.Y);

                            int closestPointIndex = 0;
                            for (int i = 1; i < testPoints.Length; i++)
                            {
                                if (Vector2.DistanceSquared(PreviousPos, testPoints[i]) < Vector2.DistanceSquared(PreviousPos, testPoints[closestPointIndex]))
                                {
                                    closestPointIndex = i;
                                }
                            }

                            if (closestPointIndex == 0)
                            {
                                if ((output.Type == TileTypes.SideWall || output.Type == TileTypes.BottomCorner || output.Type == TileTypes.ConvexCorner) && output.Hflip == true)
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.TL.X - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                                else if ((output.Type == TileTypes.SideWall || output.Type == TileTypes.BottomCorner || output.Type == TileTypes.ConvexCorner) && output.Hflip == false)
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.BR.X + .001f;
                                else
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.TL.Y - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                            }
                            else if (closestPointIndex == 1)
                            {
                                if ((((output.Type == TileTypes.SideWall || output.Type == TileTypes.BottomCorner) && output.Hflip == true)) ||
                                    (output.Type == TileTypes.BottomConvexCorner && output.Hflip == false))
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.TL.X - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                                else if ((((output.Type == TileTypes.SideWall || output.Type == TileTypes.BottomCorner) && output.Hflip == false)) ||
                                    (output.Type == TileTypes.BottomConvexCorner && output.Hflip == true))
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.BR.X + .001f;
                                else
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.BR.Y + .001f;
                            }
                            else if (closestPointIndex == 2)
                            {
                                if (output.Type == TileTypes.Wall || (output.Type == TileTypes.ConvexCorner && output.Hflip == false))
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.BR.Y + .001f;
                                else if (output.Type == TileTypes.Bottom || (output.Type == TileTypes.BottomConvexCorner && output.Hflip == true))
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.TL.Y - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                                else
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.TL.X - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                            }
                            else
                            {
                                if (output.Type == TileTypes.Wall || (output.Type == TileTypes.ConvexCorner && output.Hflip == true))
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.BR.Y + .001f;
                                else if (output.Type == TileTypes.Bottom || (output.Type == TileTypes.BottomConvexCorner && output.Hflip == false))
                                    Pos.Y = collisionHelper.Pos.Y + collisionHelper.bgCollision.TL.Y - 2 * (collisionObject as Collision_BoundingCircle).Radius - .001f;
                                else
                                    Pos.X = collisionHelper.Pos.X + collisionHelper.bgCollision.BR.X + .001f;
                            }

                        }
                    }

                }
            }
        }
        #endregion Methods

        #region Draw
        /// <summary>
        /// Draw the Scene
        /// </summary>
        /// <param name="gameTime">Game time as given by the game class</param>
        internal override void Draw(GameTime gameTime)
        {
            //Frame so we don't have to find it so often
            SpriteFrame frame = GetAnimation();
            if (mAnimating == true)
            {
                //used to update the animation. Occurs once the frame's pause * sprite's speed occurs.
                if (mLastUpdate.TotalGameTime.TotalMilliseconds + frame.Pause * Speed < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    //obtain current peg 
                    Vector2 ppos = frame.AnimationPeg;
                    mActor.Frame = (mActor.Frame + 1) % mActor.Animations[mActor.CurrentAnimation].NumFrames;
                    //update frame so we don't need to worry
                    frame = mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame];
                    //obtain next peg
                    Vector2 npos = frame.AnimationPeg;
                    //move current position to difference of two
                    Pos += (ppos - npos);
                    mLastUpdate = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                }
            }
            if (Visible == true && frame.Image != null)
            {
                This.Game.spriteBatch.Draw(
                        frame.Image,
                        Pos + frame.AnimationPeg,
                        new Rectangle((int)frame.StartPos.X, (int)frame.StartPos.Y, frame.Width, frame.Height),
                        Color.White,
                        Angle,
                        GetAnimation().AnimationPeg,
                        Scale,
                        Hflip ?
                            Vflip ?
                                SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                                : SpriteEffects.FlipHorizontally
                            :
                            Vflip ?
                                 SpriteEffects.FlipVertically
                                : SpriteEffects.None
                        ,
                        0
                    );
            }
        }

        #region Collision
        /// <summary>
        /// checks for collision with sprite of a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal Sprite CollisionWithSprite(string name)
        {
            //        //get the frame for readability
            //SpriteFrame frame = GetAnimation();
            ////get the first sprite with this name
            //Sprite s= This.Game.getCurrentLevel().findSpriteByName(name);
            //if(s==null)
            //    return null;
            //for(int i=0; i <  frame.CollisionData.Count; i++)
            //    if(frame.CollisionData[i].checkCollisions(s.getCollisionData(), s.Pos + s.GetAnimation().AnimationPeg, Pos + frame.AnimationPeg))//if there is a collision
            //        return s;
            return null;//if there aren't collisions
        }
        /// <summary>
        /// Returns collision data
        /// </summary>
        /// <returns></returns>
        //internal vector<Collision> getCollisionData();
        #endregion Collision

        internal override void Update()
        {
            PreviousPos = Pos;
            UpdateBehavior();
            checkBackgroundCollisions();
        }
        #endregion Methods
    }
}