using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer
{
    public enum GameActionType
    {
        Move,
        Dig,
        Attack,
        UseItem,
        DropItem,
        UseAbility
    }

    public class GameAction
    {
        public Player CurrentPlayer;
        public GameActionType Type;
        public Item Item;
        public Player TargetPlayer;
        public Vector TargetPosition;
    }
}
