# Wordle solver in Forth
Andrew McKewan 2025

## To Do
remove lowercasing
try using word numbers throughout
make secret a value pointing to the word list

# Results
## Basic Algorithms
Here we use the full word list without using endgame strategies
to get a baseline for each guessing method.

```
hidden off endgame off try-all
Using simple-guess                      
    1 Solved in 1 
   52 Solved in 2 
  393 Solved in 3 
  851 Solved in 4 
  739 Solved in 5 
  220 Solved in 6 
   59 Failed 
Average: 4.19    11.960 sec 

Using random-guess 
    0 Solved in 1 
   23 Solved in 2 
  276 Solved in 3 
  659 Solved in 4 
  717 Solved in 5 
  393 Solved in 6 
  247 Failed 
Average: 4.08    10.087 sec 

Using tally-guess 
    0 Solved in 1 
   25 Solved in 2 
  366 Solved in 3 
  995 Solved in 4 
  682 Solved in 5 
  198 Solved in 6 
   49 Failed 
Average: 4.20    14.846 sec 

Using entropy-guess 
    0 Solved in 1 
   53 Solved in 2 
  732 Solved in 3 
 1093 Solved in 4 
  345 Solved in 5 
   75 Solved in 6 
   17 Failed 
Average: 3.82    154.146 sec 
```
## 4-Greens Endgame
Here we use an endgame strategy that kicks on once we have 4 greens.
It uses the full wordlist to find words that cover as many of the possible
fifth letters as possible.

```
Using simple-guess 
    1 Solved in 1 
   52 Solved in 2 
  379 Solved in 3 
  808 Solved in 4 
  714 Solved in 5 
  306 Solved in 6 
   55 Failed 
Average: 4.26    11.954 sec 

Using random-guess 
    1 Solved in 1 
   25 Solved in 2 
  225 Solved in 3 
  664 Solved in 4 
  705 Solved in 5 
  499 Solved in 6 
  196 Failed 
Average: 4.27    10.366 sec 

Using tally-guess 
    0 Solved in 1 
   25 Solved in 2 
  315 Solved in 3 
  879 Solved in 4 
  738 Solved in 5 
  318 Solved in 6 
   40 Failed 
Average: 4.36    15.222 sec 

Using entropy-guess 
    0 Solved in 1 
   53 Solved in 2 
  682 Solved in 3 
  979 Solved in 4 
  451 Solved in 5 
  137 Solved in 6 
   13 Failed 
Average: 3.95    156.419 sec
```
As we can see, there is a small reduction in the number of failures:

| Algorithm  | Basic | Endgame |
| ---------- | ----- | ------- |
| Simple   |  59 |  55 |
| Random   | 247 | 196 |
| Tally    |  49 |  40 |
| Entropy  |  17 |  13 |

# Glossary

**score** ( target guess -- score )  *calculate the score for a word*

**guess** ( guess -- score )  *submit a guess to the game*

# Acknowledgements
I took some ideas inluding the scoring algorithm from
[The best strategies for Wordle](https://sonorouschocolate.com/notes/index.php/The_best_strategies_for_Wordle)
and <https://github.com/alex1770/wordle>

The information-theory math for the entropy guesser is from the video
[Solving Wordle using information theory](https://www.youtube.com/watch?v=v68zYyaEmEA)
