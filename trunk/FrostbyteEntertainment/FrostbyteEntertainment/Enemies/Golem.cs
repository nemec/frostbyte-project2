using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte.Enemies
{
    class Golem : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> anims = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("golem-idle-down.anim"),
                This.Game.CurrentLevel.GetAnimation("golem-idle-right.anim"),
                This.Game.CurrentLevel.GetAnimation("golem-idle-up.anim"),
                This.Game.CurrentLevel.GetAnimation("golem-idle-diagup.anim"),
                This.Game.CurrentLevel.GetAnimation("golem-idle-diagdown.anim"),
        };

        #endregion Variables

        internal Golem(string name, float speed, int health)
            : base(name, new Actor(anims), speed, health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new DontGetNearMePersonality(this);
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType("Mage");
            Personality.Update();
        }

        protected override void updateAttack()
        {
            float range = 100.0f;
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType("Mage");
            Sprite target = GetClosestTarget(targets, range);
            if (target != null)
            {
                // Attack!
            }
        }
    }
}

