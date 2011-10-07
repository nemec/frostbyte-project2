using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace Frostbyte.Enemies
{
    internal abstract class Enemy : Sprite
    {
        #region Variables

        //State
        public float health;
        
        //Elemental Properties
        protected enum elementTypes { Earth, Lightning, Water, Fire, Neutral };
        protected elementTypes elementType;

        //Movement
        protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        protected movementTypes currentMovementType = 0;
        protected TimeSpan movementStartTime;
        protected bool isRamming, isCharging, isStealth, isFrozen, isAttacking = false;
        protected Vector2 direction;

        #endregion Variables

        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor) 
        {
            UpdateBehavior = update;
            (This.Game.CurrentLevel as FrostbyteLevel).enemies.Add(this);
            Speed = speed;
            health = _health;
        }

        public void update()
        {
            updateMovement();
            checkBackgroundCollisions();
            updateAttack();
        }

        private void checkBackgroundCollisions()
        {
            //throw new NotImplementedException();
        }
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        #region AI Movements
        //These are only to update position of enemy
        
        /// <summary>
        /// Update enemy position directly toward target for given duration
        /// </summary>
        protected bool charge(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration
        /// </summary>
        protected bool pulseCharge(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        protected bool ram(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            // check this stuff before committing to the ram
            double distToP1 = (P1Coord.X - Pos.X) * (P1Coord.X - Pos.X) + (P1Coord.Y - Pos.Y) * (P1Coord.Y - Pos.Y);
            double distToP2 = (P2Coord.X - Pos.X) * (P2Coord.X - Pos.X) + (P2Coord.Y - Pos.Y) * (P2Coord.Y - Pos.Y);
            float ramSpeed = Speed * speedMultiplier;

            if ( !isRamming && (distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                direction = P1Coord - Pos;
                isRamming = true;
                movementStartTime = This.gameTime.TotalGameTime;
            }

            else if (!isRamming && (distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                direction = P1Coord - Pos;
                isRamming = true;
                movementStartTime = This.gameTime.TotalGameTime;
            }

            if (isRamming)
            {
                // Snapshot current gameTime
                

                if (This.gameTime.TotalGameTime <= movementStartTime + duration)
                {
                    direction.Normalize();
                    Pos += direction * ramSpeed;
                }
                else
                {
                    isRamming = false;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player
        /// </summary>
        protected bool stealthCharge(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player
        /// </summary>
        protected bool stealthCamp(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance)
        {
            return true;
        }

        /// <summary>
        /// Be Invisible and move away until x seconds have passed
        /// </summary>
        protected bool stealthRetreat(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        protected bool retreat(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Move away when x distance from target until y seconds have passed or z distance from player
        /// </summary>
        protected bool teaseRetreat(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            return true;
        }

        /// <summary>
        /// Stop moving for x seconds
        /// </summary>
        protected bool freeze(TimeSpan duration)
        {
            if (!isFrozen)
            {
                movementStartTime = This.gameTime.TotalGameTime;
                isFrozen = true;
            }

            else if (This.gameTime.TotalGameTime >= movementStartTime + duration)
            {
                isFrozen = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Switch Position with the target
        /// </summary>
        protected bool swap(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance)
        {
            return true;
        }

        #endregion AI Movements

        //todo:
        //fill in AI Movement Functions
        //create projectile class (projectiles modify health of enemies/players) 
        //complete checkBackgroundCollisions
    }
}
