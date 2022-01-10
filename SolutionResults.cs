using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2022_OnePizze
{
    class SolutionResults
    {
        public HashSet<int> GoodClients;
        public HashSet<int> UnusedClients;
        public Dictionary<int, int> ClientSolutionLikes;
        public Dictionary<int, int> ClientSolutionDislikes;
        public SolutionResults()
        {
            GoodClients = new HashSet<int>();
            UnusedClients = new HashSet<int>();
            ClientSolutionLikes = new Dictionary<int, int>();
            ClientSolutionDislikes = new Dictionary<int, int>();
        }
    }
}
