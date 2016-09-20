//http://www.lolking.net/items/2043

using System;
using System.Linq;

using LeagueSharp;
using SharpDX;
using System.Threading.Tasks;

namespace AFK_Bot_for_summoners_rift
{
    internal static class Program
    {
        private static int LastOnUpdateTick;

        private static Vector3 PlayerPosition
        {
            get
            {
                switch (Game.MapId)
                {
                    //case GameMapId.CrystalScar:
                    //    break;
                    //case GameMapId.TwistedTreeline:
                    //    break;
                    case GameMapId.SummonersRift:
                        switch (ObjectManager.Player.Team)
                        {
                            //case GameObjectTeam.Unknown:
                            //    break;
                            case GameObjectTeam.Order:
                                return new Vector3(8572, 3258, 54.43658f);
                            case GameObjectTeam.Chaos:
                                return new Vector3(6424, 11656, 54.83006f);
                                //case GameObjectTeam.Neutral:
                                //    break;
                                //default:
                                //    break;
                        }
                        break;
                        //case GameMapId.HowlingAbyss:
                        //    break;
                        //default:
                        //    break;
                }

                return Vector3.Zero;
            }
        }

        private static Vector3 WardPosition
        {
            get
            {
                switch (Game.MapId)
                {
                    //case GameMapId.CrystalScar:
                    //    break;
                    //case GameMapId.TwistedTreeline:
                    //    break;
                    case GameMapId.SummonersRift:
                        switch (ObjectManager.Player.Team)
                        {
                            //case GameObjectTeam.Unknown:
                            //    break;
                            case GameObjectTeam.Order:
                                return new Vector3(8476, 2908, 51.13f);
                            case GameObjectTeam.Chaos:
                                return new Vector3(6412, 11998, 56.4768f);
                                //case GameObjectTeam.Neutral:
                                //    break;
                                //default:
                                //    break;
                        }
                        break;
                        //case GameMapId.HowlingAbyss:
                        //    break;
                        //default:
                        //    break;
                }

                return Vector3.Zero;
            }
        }

        private static void Main(string[] args)
        {
            Hacks.AntiAFK = true;
            Hacks.UseGameObjectCache = true;

            if (Game.Mode == GameMode.Running)
            {
                Game_OnStart(new EventArgs());
            }
            else
            {
                Game.OnStart += Game_OnStart;
            }
        }

        private static void Game_OnStart(EventArgs args)
        {
            if (Game.MapId != GameMapId.SummonersRift)
            {
                Game.PrintChat("<font color = \"#00D8FF\"><b>AFK Bot for summoners lift:</b></font> You have to be in Summoner's rift to use this assembly.");
                return;
            }

            if (!ObjectManager.Player.IsRanged)
            {
                Game.PrintChat("<font color = \"#00D8FF\"><b>AFK Bot for summoners lift:</b></font> You have to be a ranged champion to use this assembly.");
                return;
            }

            Game.OnEnd += Game_OnEnd;
            Game.OnUpdate += Game_OnUpdate;

            Game.PrintChat("<font color = \"#00D8FF\"><b>AFK Bot for summoners lift:</b></font> Loaded.");
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                Game.Quit();
            });
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var nowTick = Environment.TickCount;
            if (nowTick - LastOnUpdateTick < 333)
            {
                return;
            }

            LastOnUpdateTick = Environment.TickCount;

            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (PlayerPosition == Vector3.Zero ||
               WardPosition == Vector3.Zero)
            {
                return;
            }

            if (ObjectManager.Player.InShop())
            {
                ObjectManager.Player.BuyItem(ItemId.Vision_Ward);
            }

            if (ObjectManager.Player.Distance(PlayerPosition) > 50)
            {
                if (ObjectManager.Player.CanMove)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, PlayerPosition);
                }
            }
            else
            {
                if (FogOfWar.InFog(WardPosition))
                {
                    Extensions.UseItem((int)ItemId.Vision_Ward, WardPosition);
                }
                else
                {
                    var target = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => !x.IsDead && !x.IsAlly && Extensions.InAutoAttackRange(x));
                    if (target != null)
                    {
                        if (ObjectManager.Player.CanAttack)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                }
            }
        }
    }
}
