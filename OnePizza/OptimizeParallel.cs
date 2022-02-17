using System;
using System.Collections.Generic;
using System.Threading;

namespace HashCode2022_OnePizze
{
    class OptimizeParallel
    {
        private Problem Problem { get; set; }
        private string FileName { get; set; }
        private Solution BestSolution { get; set; }
        int BestSolutionScore;

        int RandomSeed;

        public OptimizeParallel(Problem problem, string fileName)
        {
            this.Problem = problem;
            this.FileName = fileName;
        }

        public Solution Optimize(Solution solution)
        {
            this.RandomSeed = 1;

            this.BestSolution = (Solution)solution.Clone();
            this.BestSolutionScore = this.Problem.Evaluate(this.BestSolution);

            List<Thread> workThreads = new List<Thread>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Thread thread = new Thread(ThreadRunner);
                thread.Start();
                workThreads.Add(thread);
            }

            for (int i = 0; i < workThreads.Count; i++)
                workThreads[i].Join();

            return this.BestSolution;
        }

        private void ThreadRunner()
        {
            long startTimeTicks = DateTime.Now.Ticks;
            Random random;

            lock (this)
            {
                random = new Random((int)((++this.RandomSeed) * DateTime.Now.Ticks));
            }

            int currentSolutionScore;
            Solution currentSolution;
            SolutionResults solutionResults;
            List<int> unsedClients;

            lock (this)
            {
                currentSolution = (Solution)this.BestSolution.Clone();
                currentSolutionScore = this.BestSolutionScore;
                solutionResults = this.Problem.GetSolutionClients(currentSolution);
                unsedClients = new List<int>(solutionResults.UnusedClients);
            }

            while (true)
            {
                lock (this)
                {
                    if (currentSolutionScore < this.BestSolutionScore)
                    {
                        currentSolution = (Solution)this.BestSolution.Clone();
                        currentSolutionScore = this.BestSolutionScore;
                        solutionResults = this.Problem.GetSolutionClients(currentSolution);
                        unsedClients = new List<int>(solutionResults.UnusedClients);
                    }
                }

                if (unsedClients.Count == 0)
                    break;

                // Select random client, try to add the client to the solution, see if there's an improvement
                int randomSize = random.Next(10) + 1;
                HashSet<int> missingLikeIngredients = new HashSet<int>();
                HashSet<int> includedDislikeIngredients = new HashSet<int>();
                for (int i = 0; i < randomSize; i++)
                { 
                    // Select random client
                    int randomClientPos = random.Next(unsedClients.Count);
                    int randomClientId = unsedClients[randomClientPos];

                    // Find missing "like" ingredients & "dislike" ingredients
                    Client client = this.Problem.Clients[randomClientId];
                    foreach (int ingredient in client.Likes)
                        if (!currentSolution.Ingredients.Contains(ingredient))
                            missingLikeIngredients.Add(ingredient);

                    foreach (int ingredient in client.Dislikes)
                        if (currentSolution.Ingredients.Contains(ingredient))
                            includedDislikeIngredients.Add(ingredient);
                }

                int deltaEvalScore = this.Problem.EvaluateDelta(currentSolution, solutionResults, missingLikeIngredients, includedDislikeIngredients);

                if (deltaEvalScore >= currentSolutionScore)
                {
                    bool improved = (deltaEvalScore > currentSolutionScore);

                    Solution newSolution = new Solution();
                    newSolution.Ingredients.UnionWith(currentSolution.Ingredients);
                    newSolution.Ingredients.UnionWith(missingLikeIngredients);
                    newSolution.Ingredients.ExceptWith(includedDislikeIngredients);

                    SolutionResults newSolutionResults = this.Problem.GetSolutionClients(newSolution);

                    currentSolution = newSolution;
                    solutionResults = newSolutionResults;
                    currentSolutionScore = deltaEvalScore;
                    unsedClients = new List<int>(newSolutionResults.UnusedClients);

                    if (improved)
                        lock (this)
                        {
                            if (currentSolutionScore > this.BestSolutionScore)
                            {
                                Console.WriteLine("{0} {1}", new TimeSpan(DateTime.Now.Ticks - startTimeTicks), currentSolutionScore);

                                this.BestSolutionScore = currentSolutionScore;
                                this.BestSolution = (Solution)currentSolution.Clone();
                                this.Problem.GenerateOutput(this.BestSolution, this.FileName);
                            }
                        }
                }
            }
        }
    }
}
