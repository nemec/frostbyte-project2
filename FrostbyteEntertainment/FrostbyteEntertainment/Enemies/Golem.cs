using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class Golem : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("golem-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-up.anim"),
        };

        #endregion Variables

        internal Golem(string name, int health, Vector2 initialPos, float speed)
            : base(name, new Actor(Animations), speed, health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new DontGetNearMePersonality(this);
            ElementType = Element.Normal;
            Pos = initialPos;
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
            float range = 100.0f;
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            Sprite target = GetClosestTarget(targets, range);
            if (target != null)
            {
                // Attack!
            }
        }
    }
}

