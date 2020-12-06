using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public interface IGameClient
    {
        Task ChangeState(Room room);
        Task ChangeStateWithAction(Room room, GameAction gameAction);
    }
}
