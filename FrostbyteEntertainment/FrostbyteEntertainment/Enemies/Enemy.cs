using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace Frostbyte
{
    internal abstract class Enemy : Sprite
    {
        

        #region Variables

        //State
        public float health;
        
        //Elemental Properties
        protected Element elementType = Element.DEFAULT;

        //Movement
        //protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        //protected movementTypes currentMovementType = 0;
        internal TimeSpan movementStartTime;
        /// <summary>
        /// \todo make this into an enum with bits or w/e it's called (Dan knows)
        /// </summary>
        //protected bool isRamming, isCharging, isStealth=false, isFrozen, isAttacking = false;
        //protected EnemyStatus Status = EnemyStatus.Wander;
        internal IPersonality Personality;
        internal Vector2 direction;

        #endregion Variables

        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor) 
        {
            Personality = new AmbushPersonality(this);
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

        internal Sprite GetClosestTarget(List<Sprite> targets)
        {
            return GetClosestTarget(targets, float.PositiveInfinity);
        }

        /// <summary>
        /// Returns a sprite in targets that is closest to the enemy's current position
        /// and within aggroDistance distance from the current position.
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="aggroDistance"></param>
        /// <returns></returns>
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
                    Vector2.DistanceSquared(target.Pos, CenterPos) <
                    Vector2.DistanceSquared(min.Pos, CenterPos))
                {
                    if (Vector2.DistanceSquared(target.Pos, CenterPos) <= aggroDistance * aggroDistance)
                    {
                        min = target;
                    }
                }
            }
            return min;
        }

        

        //todo:
        //fill in AI Movement Functions
        //create projectile class (projectiles modify health of enemies/players) 
        //complete checkBackgroundCollisions
    }
}
