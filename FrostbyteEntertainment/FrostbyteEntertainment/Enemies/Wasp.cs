using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Frostbyte.Enemies
{
    internal partial class Wasp : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim")
        };

        #endregion Variables

        internal Wasp(string name, float speed, int health)
            : base(name, new Actor(Animations), speed, health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new DontGetNearMePersonality(this);
            ElementType = Element.Normal;
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
