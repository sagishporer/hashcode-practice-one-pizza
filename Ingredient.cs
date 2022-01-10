using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2022_OnePizze
{
    class Ingredient
    {
        public int ID { private set; get; }
        public string Name { private set; get; }

        public HashSet<Client> ClientLikes { private set; get; }
        public HashSet<Client> ClientDislikes { private set; get; }

        public Ingredient(int id, string name)
        {
            ID = id;
            Name = name;

            ClientLikes = new HashSet<Client>();
            ClientDislikes = new HashSet<Client>();
        }
    }
}
