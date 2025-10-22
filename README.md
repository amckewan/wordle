# Wordle solver in Forth

This is a Forth program that a provides a simple Wordle game and solver
based on the [online Wordle game](https://www.nytimes.com/games/wordle/index.html)
that is now owned by the *New York Times*.

We use the original list of 12,972 5-letter words that are allowed as guesses.
Game solutions are selected from a 2,315-word subset that we call
the "hidden" list.

The goal is to solve all 2,315 puzzles,
ideally without knowing the hidden word list.

There has been quite a bit of analysis done on Wordle and several people
have written an optimal solver. According to
[The best strategies for Wordle](https://sonorouschocolate.com/notes/index.php/The_best_strategies_for_Wordle),
we know that we can solve all puzzles in 5 or fewer guesses with an average
of about 3.4 guesses per puzzle.

Andrew McKewan 2025

## Wordle Game
You can play wordle with `new` which picks a random secret word from the
hidden list.
Make a guess with `g aword`. After each guess it displays the history
of guesses and scores along with what we know about each letter.
This is the same information we see in the real Wordle UI.
```
new  ok

g raise 

1 raise 
  ---GG 

Greens:  ---se
Yellows: 
Greys:   a i r 
Unused:  b c d f g h j k l m n o p q t u v w x y z  ok

g count 

1 raise 
  ---GG 
2 count 
  -Y--- 

Greens:  ---se
Yellows: o 
Greys:   a c i n r t u 
Unused:  b d f g h j k l m p q v w x y z  ok

g whose 

1 raise 
  ---GG 
2 count 
  -Y--- 
3 whose 
  GGGGG You WIN!  ok
```

## Wordle Solver
The solver starts with a "working set" that contains all of the Wordle words.
We can start with either the full word list or just the 2,315
hidden words depending on the value of the `hidden` variable.

After each guess (see next), we pune the working set by removing any words
that could not have produces that score. For example, if we get a green 'a'
in the first position, we can elimiate all words that don't start with 'a'.

We also have an endgame strategy for when we have 3 or 4 greens but not
enough guesses left to try them all.

There are a number of variables we use to control and tune the solver.

## Guessing Algorithms
There are four different guessing algorithms:

1. *Simple* - just take the first word from the working set
2. *Random* - pick a word at random
3. *Tally* - use letter frequency to rank words
4. *Entropy* - use entropy to rank works

## Results from the basic algorithms
Here are the results without knowing the hidden word list:

```
$ gforth main.fs -e "endgame off .solver tryall bye"

Hidden:   0 
Endgame:  0 
Fence:    100 
Allon2:   0 

Using simple-guess 
    1 Solved in 1 
   52 Solved in 2 
  393 Solved in 3 
  851 Solved in 4 
  739 Solved in 5 
  220 Solved in 6 
   59 Failed 
Average: 4.19    11.921 sec 

Using random-guess 
    1 Solved in 1 
   29 Solved in 2 
  276 Solved in 3 
  684 Solved in 4 
  701 Solved in 5 
  412 Solved in 6 
  212 Failed 
Average: 4.14    10.213 sec 

Using tally-guess 
    0 Solved in 1 
   25 Solved in 2 
  377 Solved in 3 
 1016 Solved in 4 
  657 Solved in 5 
  189 Solved in 6 
   51 Failed 
Average: 4.17    13.204 sec 

Using entropy-guess 
    0 Solved in 1 
   53 Solved in 2 
  188 Solved in 3 
 1386 Solved in 4 
  638 Solved in 5 
   50 Solved in 6 
    0 Failed 
Average: 4.19    426.873 sec 
```
If we use the hidden word list, we get much better average scores.
Note that the entropy guesser fails on one word when it uses the hidden
list.
```
$ gforth main.fs -e "hidden on endgame off .solver tryall bye"

Hidden:   -1 
Endgame:  0 
Fence:    100 
Allon2:   0 

Using simple-guess 
    1 Solved in 1 
   52 Solved in 2 
  393 Solved in 3 
  851 Solved in 4 
  739 Solved in 5 
  220 Solved in 6 
   59 Failed 
Average: 4.19    2.387 sec 

Using random-guess 
    2 Solved in 1 
  105 Solved in 2 
  538 Solved in 3 
  952 Solved in 4 
  533 Solved in 5 
  140 Solved in 6 
   45 Failed 
Average: 3.94    2.097 sec 

Using tally-guess 
    1 Solved in 1 
  146 Solved in 2 
  865 Solved in 3 
  994 Solved in 4 
  255 Solved in 5 
   40 Solved in 6 
   14 Failed 
Average: 3.61    2.432 sec 

Using entropy-guess 
    0 Solved in 1 
  128 Solved in 2 
  536 Solved in 3 
 1520 Solved in 4 
  127 Solved in 5 
    3 Solved in 6 
    1 Failed 
Average: 3.71    112.949 sec
```

## Endgame Stategies
When we get scores with 3 or 4 greens, we can try to reduce the number
of missing letters

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
| Simple     |  59   |  55     |
| Random     | 247   | 196     |
| Tally      |  49   |  40     |
| Entropy    |  17   |  13     |

If we just use the endgame on the puzzles we failed to solve without it,
we see that 2 additional puzzled are solved. We would have solved those
two words with our guesser, but failed when we use the endgame strategy.

# Glossary

**score** ( target guess -- score )  *calculate the score for a word*

**guess** ( guess -- score )  *submit a guess to the game*

# Acknowledgements
I took some ideas inluding the scoring algorithm from
[The best strategies for Wordle](https://sonorouschocolate.com/notes/index.php/The_best_strategies_for_Wordle)
and <https://github.com/alex1770/wordle>

The information-theory math for the entropy guesser is from the video
[Solving Wordle using information theory](https://www.youtube.com/watch?v=v68zYyaEmEA)
