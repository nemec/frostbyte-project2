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
        protected Element elementType = Element.DEFAULT;

        //Movement
        protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        protected movementTypes currentMovementType = 0;
        protected TimeSpan movementStartTime;
        /// <summary>
        /// \todo make this into an enum with bits or w/e it's called (Dan knows)
        /// </summary>
        protected bool isRamming, isCharging, isStealth=false, isFrozen, isAttacking = false;
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
            //(This.Game.CurrentLevel as FrostbyteLevel).TileMap
            updateMovement();
            checkBackgroundCollisions();
            updateAttack();            
        }

        /// \todo what is this for?
        //private override void checkBackgroundCollisions()
        //{
        //    //throw new NotImplementedException();
        //}
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        #region AI Movements
        //These are only to update position of enemy
        
        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        protected bool charge(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float chargeSpeed = Speed * speedMultiplier;
            int playerToCharge = 0;


            // choose which player to charge
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                playerToCharge = 1;
                isCharging = true;
            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                playerToCharge = 2;
                isCharging = true;
            }

            else return true;

            if (isCharging)
            {

                if (playerToCharge == 1)
                {
                    direction = P1Coord - CenterPos();
                    direction.Normalize();
                    Pos += direction * chargeSpeed;

                }
                else if (playerToCharge == 2)
                {
                    direction = P2Coord - CenterPos();
                    direction.Normalize();
                    Pos += direction * chargeSpeed;
                }
            }
            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            return false;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        protected bool pulseCharge(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = (float) Math.Sin( (2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0 ) * (2 * Math.PI) ) + 1.5f;

            return charge(P1Coord, P2Coord, aggroDistance, speedMultiplier);
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        protected bool ram(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            // check this stuff before committing to the ram
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float ramSpeed = Speed * speedMultiplier;

            if ( !isRamming && (distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                direction = P1Coord - CenterPos();
                isRamming = true;
            }

            else if (!isRamming && (distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                direction = P2Coord - CenterPos();
                isRamming = true;
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

            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player - complete
        /// </summary>
        protected bool stealthCharge(Vector2 P1Coord, Vector2 P2Coord, TimeSpan duration, float aggroDistance, float visibleDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float chargeSpeed = Speed * speedMultiplier;
            int playerToCharge = 0;

            // choose which player to charge
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                playerToCharge = 1;
                isCharging = true;
             
                // decide whether or not to stealth
                if (distToP1 <= visibleDistance * visibleDistance)
                {
                    mVisible = true;
                }
                else
                {
                    mVisible = false;
                }
            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                playerToCharge = 2;
                isCharging = true;

                // decide whether or not to stealth
                if (distToP2 <= visibleDistance * visibleDistance)
                {
                    mVisible = true;
                }
                else
                {
                    mVisible = false;
                }
            }

            if (isCharging)
            {
                if (This.gameTime.TotalGameTime <= movementStartTime + duration)
                {
                    if (playerToCharge == 1)
                    {
                        direction = P1Coord - CenterPos();
                        direction.Normalize();
                        Pos += direction * chargeSpeed;
                    }
                    else if (playerToCharge == 2)
                    {
                        direction = P2Coord - CenterPos();
                        direction.Normalize();
                        Pos += direction * chargeSpeed;
                    }
                }
                else
                {
                    isCharging = false;
                    mVisible = true;
                    return true;
                }
            }
            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player - complete
        /// </summary>
        protected bool stealthCamp(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());

            if (aggroDistance * aggroDistance >= distToP1 /*|| aggroDistance <= distToP2*/)
            {
                isFrozen = false;
                mVisible = true;
            }

            else if ( aggroDistance * aggroDistance < distToP1 )
            {
                isFrozen = true;
                mVisible = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and move away until you are y distance away
        /// </summary>
        protected bool stealthRetreat(Vector2 P1Coord, Vector2 P2Coord, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float fleeSpeed = Speed * speedMultiplier;
            int playerToFlee = 0;


            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= safeDistance * safeDistance))
            {
                // charge P1
                mVisible = false;
                playerToFlee = 1;
                isCharging = true;
            }

            else if ((distToP2 < distToP1) && (distToP2 <= safeDistance * safeDistance))
            {
                // charge P2
                mVisible = false;
                playerToFlee = 2;
                isCharging = true;
            }

            else
            {
                isCharging = false;
                mVisible = true;
                return true;
            }

            if (isCharging)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {

                    direction = P1Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }

                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    direction = P2Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }
            }

            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            return false;
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        protected bool retreat(Vector2 P1Coord, Vector2 P2Coord, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float fleeSpeed = Speed * speedMultiplier;
            int playerToFlee = 0;


            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= safeDistance * safeDistance))
            {
                // charge P1
                playerToFlee = 1;
                isCharging = true;
                
            }

            else if ((distToP2 < distToP1) && (distToP2 <= safeDistance * safeDistance))
            {
                // charge P2
                playerToFlee = 2;
                isCharging = true;
            }

            else
            {
                isCharging = false;
                return true;
            }

            if (isCharging)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {
                    direction = P1Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    direction = P2Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }

            }

            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }
            return false;
        }

        /// <summary>
        /// Move away when x distance from target until z distance from player
        /// </summary>
        protected bool teaseRetreat(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos());
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos());
            float fleeSpeed = Speed * speedMultiplier;
            int playerToFlee = 0;



            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance) || (isCharging && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P1
                playerToFlee = 1;
                isCharging = true;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance) || (isCharging && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P2
                playerToFlee = 2;
                isCharging = true;
            }

            else if ( Math.Min(distToP1, distToP2) >= safeDistance * safeDistance )
            {
               // isCharging = false;
               // return true;
            }

            if (isCharging)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {
                    direction = P1Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    direction = P2Coord - CenterPos();
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }

            }

            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }
            return false;
        }

        /// <summary>
        /// Stop moving for x seconds - complete
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
            double distToP1 = Vector2.DistanceSquared(P1Coord, Pos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, Pos);
            int playerToSwap = 0;

            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                playerToSwap = 1;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                playerToSwap = 2;
            }

            else
            {
                return false;
            }


            if ((playerToSwap == 1) && (distToP1 < aggroDistance * aggroDistance))
            { 
                This.Game.CurrentLevel.GetSpritesByType("Mage")[0].Pos = Pos;
                Pos = P1Coord;
                return true;
            }
            else if ((playerToSwap == 2) && (distToP2 < aggroDistance * aggroDistance))
            {
                This.Game.CurrentLevel.GetSpritesByType("Mage")[1].Pos = Pos;
                Pos = P2Coord;
                return true;
            }

            return false;
            
        }

        #endregion AI Movements

        

        //todo:
        //fill in AI Movement Functions
        //create projectile class (projectiles modify health of enemies/players) 
        //complete checkBackgroundCollisions
    }
}
