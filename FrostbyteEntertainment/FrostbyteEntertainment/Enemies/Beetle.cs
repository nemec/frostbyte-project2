﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Frostbyte.Enemies
{
    internal partial class Beetle : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("beetle-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-attack-up.anim"),
        };
        #endregion Variables

        public Beetle(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
            startAttackDistance = 15; //in pixels
            This.Game.AudioManager.AddSoundEffect("Effects/Beetle_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Beetle_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }

            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null)
            {
                if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) > this.startAttackDistance * this.startAttackDistance - 100)
                {
                    Personality.Update();
                }
            }
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 0, 0, 750) && isAttackAnimDone)
            {
                float range = 150.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) <= this.startAttackDistance * this.startAttackDistance)
                    {
                        mAttacks.Add(Attacks.Melee(target, this, 5, 18, 40).GetEnumerator());
                        attackStartTime = This.gameTime.TotalGameTime;
                    }
                }
            }
        }
    }
}
