using System;
using System.Linq;
using System.Collections.Generic;

namespace HashCode2022_OnePizze
{
    class Program
    {
        static string[] INPUT_FILES = {
            @"c:\temp\hashcode\a_an_example.in.txt",
            @"c:\temp\hashcode\b_basic.in.txt",
            @"c:\temp\hashcode\c_coarse.in.txt",
            @"c:\temp\hashcode\d_difficult.in.txt",
            @"c:\temp\hashcode\e_elaborate.in.txt"
        };

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            foreach (string fileName in INPUT_FILES)
            {
                DateTime solveStartTime = DateTime.Now;
                Solve(fileName);
                Console.WriteLine("Solve time: {0}", new TimeSpan(DateTime.Now.Ticks - solveStartTime.Ticks));
            }

            Console.WriteLine("Runtime: {0}", new TimeSpan(DateTime.Now.Ticks - startTime.Ticks));
        }

        static void Solve(string fileName)
        {
            Problem p = Problem.LoadProblem(fileName);

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine(fileName);
            Console.WriteLine("Clients: {0}, Ingredients: {1}", p.Clients.Count, p.Ingredients.Count);

            Solution solution = new Solution();
            solution = OptimizeSolution(p, solution);

            Console.WriteLine("Solution score: {0}", p.Evaluate(solution));
            p.GenerateOutput(solution, fileName + ".out");
        }

        static Solution OptimizeSolution(Problem p, Solution solution)
        {
            Random random = new Random(17);
            SolutionResults solutionResults = p.GetSolutionClients(solution);
            List<int> unsedClients = new List<int>(solutionResults.UnusedClients);
            int score = solutionResults.GoodClients.Count;

            // 100,000 = 2,5,5,1805,2065
            // 1M = 2,5,5,1805,2078
            int retries = 0;
            while (retries < 1000000)
            {
                retries++;

                if (unsedClients.Count == 0)
                    break;

                // Select random client, try to add the client to the solution, see if there's an improvement

                // Select random client
                int randomClientPos = random.Next(unsedClients.Count);
                int randomClientId = unsedClients[randomClientPos];

                // Find missing "like" ingredients & "dislike" ingredients
                Client client = p.Clients[randomClientId];
                HashSet<int> missingLikeIngredients = new HashSet<int>();
                foreach (int ingredient in client.Likes)
                    if (!solution.Ingredients.Contains(ingredient))
                        missingLikeIngredients.Add(ingredient);

                HashSet<int> includedDislikeIngredients = new HashSet<int>();
                foreach (int ingredient in client.Dislikes)
                    if (solution.Ingredients.Contains(ingredient))
                        includedDislikeIngredients.Add(ingredient);

                int deltaEvalScore = p.EvaluateDelta(solution, solutionResults, missingLikeIngredients, includedDislikeIngredients);
                
                if (deltaEvalScore >= score)
                {
                    Solution newSolution = new Solution();
                    newSolution.Ingredients.UnionWith(solution.Ingredients);
                    newSolution.Ingredients.UnionWith(missingLikeIngredients);
                    newSolution.Ingredients.ExceptWith(includedDislikeIngredients);

                    SolutionResults newSolutionResults = p.GetSolutionClients(newSolution);

                    if (deltaEvalScore > score)
                        retries = 0;

                    solution = newSolution;
                    solutionResults = newSolutionResults;
                    score = deltaEvalScore;
                    unsedClients = new List<int>(newSolutionResults.UnusedClients);
                }
            }

            return solution; 
        }

        static Solution BuildSolution(Problem p)
        {
            Solution solution = new Solution();
            // 2, 5, 5, 1788, 1568 (order by appearance time in input file)
            List<Ingredient> ingredients = new List<Ingredient>(p.Ingredients);
            // 2, 5, 4, 1793, 1814
            ingredients = ingredients.OrderBy(o => -(o.ClientLikes.Count - o.ClientDislikes.Count)).ToList();
            // 2, 5, 2, 1791, 1376
            //ingredients = ingredients.OrderBy(o => (o.ClientLikes.Count - o.ClientDislikes.Count)).ToList();
            // 2, 5, 4, 1792, 1694
            //ingredients = ingredients.OrderBy(o => -(o.ClientLikes.Count)).ToList();
            // 2, 5, 2, 1797, 1469
            //ingredients = ingredients.OrderBy(o => (o.ClientLikes.Count)).ToList();
            // 2, 5, 3, 1792, 1403
            //ingredients = ingredients.OrderBy(o => -(o.ClientDislikes.Count)).ToList();
            // 2, 5, 4, 1796, 1800 
            //ingredients = ingredients.OrderBy(o => (o.ClientDislikes.Count)).ToList();
            while (ingredients.Count > 0)
            {
                Ingredient ingredient = GreedyFindIngredientToAdd(ingredients, p, solution);
                if (ingredient == null)
                    break;
                solution.Ingredients.Add(ingredient.ID);
                ingredients.Remove(ingredient);
            }

            return solution;
        }

        static Ingredient GreedyFindIngredientToAdd(List<Ingredient> ingredients, Problem p, Solution solution)
        {
            if (ingredients.Count == 0)
                return null;

            SolutionResults solutionResults = p.GetSolutionClients(solution);
            HashSet<int> currentScoreClients = solutionResults.GoodClients;
            int currentScore = solutionResults.GoodClients.Count;
            Ingredient best = null;
            int bestScore = 0;

            foreach (Ingredient ingredient in ingredients)
            {
                int newScore = currentScore;
                foreach (Client client in ingredient.ClientLikes)
                {
                    if ((solutionResults.ClientSolutionLikes[client.ID] == client.Likes.Count - 1)
                        &&(solutionResults.ClientSolutionDislikes[client.ID] == 0)) 
                        newScore++;
                }

                foreach (Client client in ingredient.ClientDislikes)
                {
                    if (currentScoreClients.Contains(client.ID))
                        newScore--;
                }
                
                if (best == null)
                {
                    if (newScore >= currentScore)
                    {
                        best = ingredient;
                        bestScore = newScore;
                    }
                } 
                else if (newScore > bestScore)
                {
                    best = ingredient;
                    bestScore = newScore;
                }
            }

            return best;
        }

    }
}
