using System.Linq;

using LeagueSharp;
using SharpDX;

namespace AFK_Bot_for_summoners_lift
{
    internal static class Extensions
    {
        static Extensions()
        {

        }

        internal static float Distance(this Obj_AI_Base unit, Vector3 point, bool squared = false)
        {
            return unit.ServerPosition.To2D().Distance(point.To2D(), squared);
        }

        internal static float Distance(this Vector2 v, Vector2 to, bool squared = false)
        {
            return squared ? Vector2.DistanceSquared(v, to) : Vector2.Distance(v, to);
        }

        internal static Vector2 To2D(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        internal static bool HasItem(int id, Obj_AI_Hero hero = null)
        {
            return (hero ?? ObjectManager.Player).InventoryItems.Any(slot => slot.Id == (ItemId)id);
        }

        internal static bool UseItem(int id, Vector3 position)
        {
            if (position != Vector3.Zero)
            {
                foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id))
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, position);
                }
            }

            return false;
        }

        internal static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
            if (target != null && target.IsValid)
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && ObjectManager.Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        internal static bool InAutoAttackRange(AttackableUnit target)
        {
            if (target == null || target.IsDead)
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target);

            return                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    ObjectManager.Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool InShop(this Obj_AI_Base unit)
        {
            float fountainRange = 562500; //750 * 750

            if (Game.MapId == GameMapId.SummonersRift)
            {
                fountainRange = 1000000; //1000 * 1000
            }

            var fpos = unit.Team == ObjectManager.Player.Team ?
                ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsAlly).Position : ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsEnemy).Position;

            return unit.IsVisible && unit.Distance(fpos, true) <= fountainRange;
        }
    }
}
