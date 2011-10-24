using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;

namespace Frostbyte
{
    internal abstract partial class Enemy : OurSprite
    {
        #region Variables
        //Movement
        //protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        //protected movementTypes currentMovementType = 0;
        internal TimeSpan movementStartTime = TimeSpan.MaxValue;

        //protected EnemyStatus Status = EnemyStatus.Wander;
        internal IPersonality Personality;
        protected Vector2 mDirection = new Vector2();
        #endregion Variables

        #region Properties
        internal Vector2 Direction
        {
            get
            {
                return mDirection;
            }
            set
            {
                State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;

                mDirection = value;
                mDirection.Normalize();
                double angle = Math.Atan2(mDirection.Y, mDirection.X);
                if (-Math.PI / 8 < angle && angle < Math.PI / 8)
                {
                    Orientation = Orientations.Right;
                }
                else if (Math.PI / 8 < angle && angle < 3 * Math.PI / 8)
                {
                    Orientation = Orientations.Down_Right;
                }
                else if (3 * Math.PI / 8 < angle && angle < 5 * Math.PI / 8)
                {
                    Orientation = Orientations.Down;
                }
                else if (5 * Math.PI / 8 < angle && angle < 7 * Math.PI / 8)
                {
                    Orientation = Orientations.Down_Left;
                }
                else if (-3 * Math.PI / 8 < angle && angle < -Math.PI / 8)
                {
                    Orientation = Orientations.Up_Right;
                }
                else if (-5 * Math.PI / 8 < angle && angle < -3 * Math.PI / 8)
                {
                    Orientation = Orientations.Up;
                }
                else if (-7 * Math.PI / 8 < angle && angle < -5 * Math.PI / 8)
                {
                    Orientation = Orientations.Up_Left;
                }
                else
                {
                    Orientation = Orientations.Left;
                }
            }
        }

        //Elemental Properties
        protected Element ElementType { get; set; }
        #endregion


        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor)
        {
            Personality = new WanderingMinstrelPersonality(this);
            UpdateBehavior = update;
            (This.Game.CurrentLevel as FrostbyteLevel).enemies.Add(this);
            Speed = speed;
            Health = _health;
        }

        public void update()
        {
            //(This.Game.CurrentLevel as FrostbyteLevel).TileMap
            updateMovement();
            //update animation facing direction
            switch (Orientation)
            {
                case Orientations.Down:
                    SetAnimation(0 + 5 * State.GetHashCode());
                    break;
                case Orientations.Down_Right:
                    Hflip = false;
                    SetAnimation(1 + 5 * State.GetHashCode());
                    break;
                case Orientations.Down_Left:
                    Hflip = true;
                    SetAnimation(1 + 5 * State.GetHashCode());
                    break;
                case Orientations.Right:
                    Hflip = false;
                    SetAnimation(2 + 5 * State.GetHashCode());
                    break;
                case Orientations.Left:
                    Hflip = true;
                    SetAnimation(2 + 5 * State.GetHashCode());
                    break;
                case Orientations.Up_Right:
                    Hflip = false;
                    SetAnimation(3 + 5 * State.GetHashCode());
                    break;
                case Orientations.Up_Left:
                    Hflip = true;
                    SetAnimation(3 + 5 * State.GetHashCode());
                    break;
                case Orientations.Up:
                    SetAnimation(4 + 5 * State.GetHashCode());
                    break;
            }

            //checkBackgroundCollisions();
            updateAttack();
        }

        /// \todo what is this for?
        //private override void checkBackgroundCollisions()
        //{
        //    //throw new NotImplementedException();
        //}
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        internal Sprite GetClosestTarget(List<Sprite> targets)
        {
            return GetClosestTarget(targets, float.PositiveInfinity);
        }

        /// <summary>
        /// Returns a sprite in targets that is closest to the enemy's current position
        /// and within aggroDistance distance from the current position.
        /// </summary>
        internal Sprite GetClosestTarget(List<Sprite> targets, float aggroDistance)
        {
            Sprite min = null;
            foreach (Sprite target in targets)
            {
                if (target == this)
                {
                    continue;
                }
                if (min == null ||
                    Vector2.DistanceSquared(target.CenterPos, CenterPos) <
                    Vector2.DistanceSquared(min.CenterPos, CenterPos))
                {
                    if (Vector2.DistanceSquared(target.CenterPos, CenterPos) <= aggroDistance * aggroDistance)
                    {
                        min = target;
                    }
                }
            }
            return min;
        }

        /// \todo create projectile class (projectiles modify health of enemies/players) 
        /// \todo complete checkBackgroundCollisions
    }
}
