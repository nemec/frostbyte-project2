using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Frostbyte
{
    internal enum EnemyStatus
    {
        Wander,
        Ram,
        Charge,
        Stealth,
        Frozen,
        Attack
    }

    internal interface IPersonality
    {
        EnemyStatus Status { get; set; }
        void Update();
    }

    internal class AmbushPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal AmbushPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType("Mage");
            while (true)
            {
                float dist = Vector2.DistanceSquared(master.GetClosestTarget(targets).CenterPos, master.CenterPos);
                while (dist > 150 * 150)
                {
                    master.charge(targets, 1000f, 1f);
                    dist = Vector2.DistanceSquared(master.GetClosestTarget(targets).CenterPos, master.CenterPos);
                    yield return null;
                }
                while (!master.stealthCamp(targets, 100f, 150f))
                {
                    yield return null;
                }
                while (!master.charge(targets, 100f, 2))
                {
                    yield return null;
                }
            }
        }
    }

    internal static class EnemyAI
    {
        //These are only to update position of enemy

        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        internal static bool charge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            Sprite min = ths.GetClosestTarget(targets, aggroDistance);

            if (min == null)  // No targets, so just continue on
            {
                return true;
            }

            float chargeSpeed = ths.Speed * speedMultiplier;
            ths.direction = min.Pos - ths.CenterPos;
            ths.direction.Normalize();
            ths.Pos += ths.direction * chargeSpeed;

            return false;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        internal static bool pulseCharge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = (float)Math.Sin((2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0) * (2 * Math.PI)) + 1.5f;

            return ths.charge(targets, aggroDistance, speedMultiplier);
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        internal static bool ram(this Enemy ths, List<Sprite> targets, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Ram)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;

                Sprite target = ths.GetClosestTarget(targets, aggroDistance);
                if (target != null)
                {
                    ths.direction = target.Pos - ths.CenterPos;
                    ths.direction.Normalize();
                    ths.Personality.Status = EnemyStatus.Ram;
                }
            }

            float ramSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Ram)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Pos += ths.direction * ramSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player - complete
        /// </summary>
        internal static bool stealthCharge(this Enemy ths, List<Sprite> targets, TimeSpan duration, float aggroDistance, float visibleDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, aggroDistance);
            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
                if (Vector2.DistanceSquared(target.Pos, ths.CenterPos) <= visibleDistance * visibleDistance)
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.mVisible = true;
                    return true;
                }
                else
                {
                    ths.mVisible = false;
                }
            }

            float chargeSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.direction = target.Pos - ths.CenterPos;
                    ths.direction.Normalize();
                    ths.Pos += ths.direction * chargeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.mVisible = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player - complete
        /// </summary>
        internal static bool stealthCamp(this Enemy ths, List<Sprite> targets, float aggroDistance, float ignoreDistance)
        {
            Sprite target = ths.GetClosestTarget(targets, aggroDistance);

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                ths.mVisible = true;
                return true;
            }
            else
            {
                target = ths.GetClosestTarget(targets);
                if(target != null && (Vector2.DistanceSquared(target.Pos, ths.CenterPos) >
                    (ignoreDistance * ignoreDistance))){
                        ths.Personality.Status = EnemyStatus.Wander;
                        ths.mVisible = true;
                        return true;
                }

                ths.Personality.Status = EnemyStatus.Frozen;
                ths.mVisible = false;
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and move away until you are y distance away
        /// </summary>
        internal static bool stealthRetreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            ths.mVisible = ths.retreat(targets, safeDistance, speedMultiplier);
            return ths.mVisible;
        }

        internal static bool retreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            return ths.retreat(targets, TimeSpan.MaxValue, safeDistance, speedMultiplier);
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        internal static bool retreat(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, safeDistance);
            float fleeSpeed = ths.Speed * speedMultiplier;

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
            }
            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.direction = target.Pos - ths.CenterPos;
                    ths.direction.Normalize();
                    ths.Pos -= ths.direction * fleeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Move away when x distance from target until z distance from player
        /// </summary>
        /*internal static bool teaseRetreat(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, ths.CenterPos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, ths.CenterPos);
            float fleeSpeed = Speed * speedMultiplier;
            int playerToFlee = 0;



            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance) || (ths.Personality.Status == EnemyStatus.Charge && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P1
                playerToFlee = 1;
                ths.Personality.Status = EnemyStatus.Charge;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance) || (ths.Personality.Status == EnemyStatus.Charge && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P2
                playerToFlee = 2;
                ths.Personality.Status = EnemyStatus.Charge;
            }

            else if (Math.Min(distToP1, distToP2) >= safeDistance * safeDistance)
            {
                // isCharging = false;
                // return true;
            }

            if (ths.Personality.Status == EnemyStatus.Charge)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {
                    ths.direction = P1Coord - ths.CenterPos;
                    ths.direction.Normalize();
                    Pos -= ths.direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    ths.direction = P2Coord - ths.CenterPos;
                    ths.direction.Normalize();
                    Pos -= ths.direction * fleeSpeed;
                }

            }

            else
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }
            return false;
        }*/

        /// <summary>
        /// Stop moving for x seconds - complete
        /// </summary>
        internal static bool freeze(this Enemy ths, TimeSpan duration)
        {
            if (ths.Personality.Status != EnemyStatus.Frozen)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
                ths.Personality.Status = EnemyStatus.Frozen;
            }

            else if (This.gameTime.TotalGameTime >= ths.movementStartTime + duration)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wander around for x seconds or until within a certain distance from a target
        /// 
        /// </summary>
        internal static bool wander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float arcAngle)
        {
            Sprite min = ths.GetClosestTarget(targets, safeDistance);

            if (min != null)  // Near a target, move on to something else
            {
                return true;
            }
            Random r = new Random();
            double angle = Math.Atan2(ths.direction.Y, ths.direction.X) + (2 * r.NextDouble() - 1) * arcAngle;
            ths.direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            /*float chargeSpeed = Speed;
            ths.direction = min.Pos - ths.CenterPos;
            ths.direction.Normalize();*/
            ths.Pos += ths.direction * ths.Speed / 2;  // Wandering should be *slow*

            return false;
        }

        /// <summary>
        /// Switch Position with the target
        /// </summary>
        /*internal static bool swap(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance)
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

        }*/
    }
}
