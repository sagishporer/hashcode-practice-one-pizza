using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2022_OnePizze
{
    class Problem
    {
        public List<Ingredient> Ingredients { private set; get; }
        public Dictionary<string, Ingredient> IngredientsLookup { private set; get; }

        public List<Client> Clients { private set; get; }

        private Problem()
        {
            Clients = new List<Client>();
            Ingredients = new List<Ingredient>();
            IngredientsLookup = new Dictionary<string, Ingredient>();
        }

        public void GenerateOutput(Solution solution, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(solution.Ingredients.Count);
            foreach (int ingredient in solution.Ingredients)
            {
                sb.Append(' ');
                sb.Append(this.Ingredients[ingredient].Name);
            }

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(sb.ToString());
        }

        public int EvaluateDelta(Solution solution, SolutionResults solutionResults, 
            HashSet<int> addedIngredients, HashSet<int> removedIngredients)
        {
            HashSet<int> removedClients = new HashSet<int>();
            HashSet<int> addedClients = new HashSet<int>();

            HashSet<Client> newCandidateClients = new HashSet<Client>();
            HashSet<Client> addedIngredientsDislikeClients = new HashSet<Client>();
            foreach (int addedIngredient in addedIngredients)
            {
                newCandidateClients.UnionWith(this.Ingredients[addedIngredient].ClientLikes);
                addedIngredientsDislikeClients.UnionWith(this.Ingredients[addedIngredient].ClientDislikes);
            }

            HashSet<Client> removedIngredientsLikeClients = new HashSet<Client>();
            foreach (int removedIngredient in removedIngredients)
            {
                removedIngredientsLikeClients.UnionWith(this.Ingredients[removedIngredient].ClientLikes);
                newCandidateClients.UnionWith(this.Ingredients[removedIngredient].ClientDislikes);
            }

            // New solution added clients (they can't be in the existing solution - added likes or removed dislikes)
            HashSet<int> newIngredients = new HashSet<int>(solution.Ingredients);
            newIngredients.UnionWith(addedIngredients);
            newIngredients.ExceptWith(removedIngredients);
            foreach (Client client in newCandidateClients)
            {
                if (client.IsGoodPizza(newIngredients))
                    addedClients.Add(client.ID);
            }

            // Current solution client removed - removed like ingredient
            foreach (Client client in removedIngredientsLikeClients)
                if (solutionResults.GoodClients.Contains(client.ID))
                    removedClients.Add(client.ID);

            // Current solution client removed - added dislike ingredient
            foreach (Client client in addedIngredientsDislikeClients)
                if (solutionResults.GoodClients.Contains(client.ID))
                    removedClients.Add(client.ID);

            return solutionResults.GoodClients.Count + addedClients.Count - removedClients.Count;
        }

        public int Evaluate(Solution solution)
        {
            int score = 0;
            foreach (Client client in this.Clients)
            {
                if (client.IsGoodPizza(solution.Ingredients))
                    score++;
            }

            return score;
        }

        public SolutionResults GetSolutionClients(Solution solution)
        {
            SolutionResults solutionResults = new SolutionResults();
            HashSet<int> solutionClients = new HashSet<int>();
            foreach (Client client in this.Clients)
            {
                int dislikeCount = client.DislikeCount(solution.Ingredients);
                solutionResults.ClientSolutionDislikes.Add(client.ID, dislikeCount);

                int likeCount = client.LikeCount(solution.Ingredients);
                solutionResults.ClientSolutionLikes.Add(client.ID, likeCount);

                if (((client.Likes.Count - likeCount) == 0) && (dislikeCount == 0))
                    solutionResults.GoodClients.Add(client.ID);
                else
                    solutionResults.UnusedClients.Add(client.ID);
            }

            return solutionResults;
        }

        public static Problem LoadProblem(string fileName)
        {
            Problem p = new Problem();

            using (StreamReader sr = new StreamReader(fileName))
            {
                int clientsNum = int.Parse(sr.ReadLine());
                for (int i = 0; i < clientsNum; i++)
                {
                    Client client = new Client(i);

                    HashSet<string> likes = ReadItemsLine(sr.ReadLine());
                    p.GenerateIngredients(likes);
                    foreach (string like in likes)
                    {
                        p.IngredientsLookup[like].ClientLikes.Add(client);
                        client.Likes.Add(p.IngredientsLookup[like].ID);
                    }

                    HashSet<string> dislikes = ReadItemsLine(sr.ReadLine());
                    p.GenerateIngredients(dislikes);
                    foreach (string dislike in dislikes)
                    {
                        p.IngredientsLookup[dislike].ClientDislikes.Add(client);
                        client.Dislikes.Add(p.IngredientsLookup[dislike].ID);
                    }

                    p.Clients.Add(client);
                }
            }

            return p;
        }

        private void GenerateIngredients(HashSet<string> ingredientStrings)
        {
            foreach (string ingredientString in ingredientStrings)
            {
                if (this.IngredientsLookup.ContainsKey(ingredientString))
                    continue;

                Ingredient ingredient = new Ingredient(this.Ingredients.Count, ingredientString);
                this.Ingredients.Add(ingredient);
                this.IngredientsLookup.Add(ingredientString, ingredient);
            }
        }

        private static HashSet<string> ReadItemsLine(string str)
        {
            string[] parts = str.Split(' ');
            int cnt = int.Parse(parts[0]);
            HashSet<string> output = new HashSet<string>();
            for (int i = 0; i < cnt; i++)
                output.Add(parts[1 + i]);

            return output;
        }
    }
}
