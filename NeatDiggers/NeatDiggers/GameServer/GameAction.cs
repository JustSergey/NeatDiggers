using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Abilities;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer
{
    public enum Target
    {
        None,
        Player,
        Position
    }

    public enum GameActionType
    {
        Move,
        Dig,
        Attack,
        UseItem,
        DropItem,
        UseAbility,
        TakeTheFlag
    }

    public class GameAction
    {
        public Player CurrentPlayer { get; set; }
        public GameActionType Type { get; set; }
        public Item Item { get; set; }
        public Ability Ability { get; set; }
        public string TargetPlayerId { get; set; }
        public Vector TargetPosition { get; set; }
    }
}