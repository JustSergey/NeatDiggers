using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public class User
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
        public string Id { get; set; }

        public string GetId() => $"{Token.GetHashCode()}{Code.GetHashCode()}".Replace('-', '1');
    }
}
