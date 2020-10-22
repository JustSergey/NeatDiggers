using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    class UserInfo
    {
        public int Id;
        public string Name;
    }

    class User
    {
        int id;
        string name;
        Random random;

        public User(string name)
        {
            this.name = name;
            random = new Random();
            id = random.Next();
        }

        public UserInfo GetInfo()
        {
            return new UserInfo
            {
                Id = id,
                Name = name
            };
        }
    }
}
