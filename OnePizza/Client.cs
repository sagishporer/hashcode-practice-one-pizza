using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2022_OnePizze
{
    class Client
    {
        public int ID { private set; get; }
        public HashSet<int> Likes { private set; get; }
        public HashSet<int> Dislikes { private set; get; }

        public Client(int id)
        {
            ID = id;
            Likes = new HashSet<int>();
            Dislikes = new HashSet<int>();
        }

        public bool IsGoodPizza(HashSet<int> ingredients)
        {
            if (LikeCount(ingredients) < this.Likes.Count)
                return false;

            if (IsDislike(ingredients))
                return false;

            return true;
        }

        public int LikeCount(HashSet<int> ingredients)
        {
            return Utils.IntersectSize<int>(this.Likes, ingredients);
        }

        public int DislikeCount(HashSet<int> ingredients)
        {
            return Utils.IntersectSize<int>(this.Dislikes, ingredients);
        }

        public bool IsDislike(HashSet<int> ingredients)
        {
            int dislikesCnt = DislikeCount(ingredients);
            if (dislikesCnt > 0)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}
