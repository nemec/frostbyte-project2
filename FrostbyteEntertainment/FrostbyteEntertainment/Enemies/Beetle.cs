using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;


namespace Frostbyte.Enemies
{
    internal partial class Beetle : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("beetle-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("beetle-idle-diagdown.anim"),
        };

        #endregion Variables

        internal Beetle(string name, float speed, int health, Vector2 initialPos)
            : base(name, new Actor(Animations), speed, health)
        {
            Pos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ImmobilePersonality();
            
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
