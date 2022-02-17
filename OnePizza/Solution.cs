using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2022_OnePizze
{
    class Solution : ICloneable
    {
        public HashSet<int> Ingredients;

        public Solution()
        {
            this.Ingredients = new HashSet<int>();
        }

        public object Clone()
        {
            Solution other = new Solution();
            other.Ingredients.UnionWith(this.Ingredients);

            return other;
        }
    }
}
