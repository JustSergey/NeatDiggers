using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public class Effect
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        private Action<Player> cancellingAction;

        public Effect(Action<Player> cancellingAction) => this.cancellingAction = cancellingAction;

        public void Cancel(Player player) => cancellingAction(player);
    }
}
