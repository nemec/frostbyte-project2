﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class Spider : Frostbyte.Enemy
    {

        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("spider-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-up.anim"),
        };

        #endregion Variables

        public Spider(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new PulseChargePersonality(this);
            ElementType = Element.Normal;
            Scale = .4f;

            startAttackDistance = 20;

            This.Game.AudioManager.AddSoundEffect("Effects/Spider_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Spider_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (isAttacking)
            {
                mAttack.MoveNext();
                isAttacking = !mAttack.Current;
            }
            else
            {
                float range = 150.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) < this.startAttackDistance * this.startAttackDistance)
                    {
                        isAttacking = true;
                        isAttackingAllowed = false;
                        mAttack = Attacks.Melee(target, this, 5, 18, 20, new TimeSpan(0,0,0,0,250)).GetEnumerator();
                    }
                }
            }     
        }

    }
}