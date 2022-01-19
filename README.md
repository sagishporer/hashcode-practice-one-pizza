# Google Hash Code 2022 practice problem solution: One Pizza

## Algorithm description:
### Hill-climbing algorithm 
- Empty initialization.
- Update state if new state equals or better in score,
- Random steps.

### Details
1. Initialize solution - no ingredients.
2. Select a random client which does not want the current pizza.
3. Add missing ingredients the random client like.
4. Remove existing ingredients the random client dislike.
5. If the new ingredients score is equal or higher than the existing - update the current pizza ingredients.
6. Goto 2. 

### Notes
- Tried greedy init. Did not improve the solution.
- When calculating the score with the new ingredients - perform a delta calculation which is faster.
- The commited code is hill-climb, constant seed random, stop condition: no improvment found in the last 1M iterations.

## Scores 
Tested the algorithm in 2 modes:
- 1M Iterations - Stop condition: no improvment found in the last 1M iterations. The algorithm takes about 10 minutes to run with the constant seed random in the code.
- Max - continue hill-climbing without limit. The best solution for E was found after ~8 hours. Time based random seed.

| Input | 1M Iterations | Max |
| --- | --- | --- |
| A - An Example | 2 | = |
| B - Basic | 5 | = |
| C - Coarse | 5 | = |
| D - Difficult | 1,805 | = |
| E - Elaborate | 2,078 | 2,082 |
| Total | 3,895 | 3,899 |
