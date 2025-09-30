# Wordle solver in Forth
Andrew McKewan 2025

# Glossary

**score** ( target guess -- score )  *calculate the score for a word*

**guess** ( guess -- score )  *submit a guess to the game*

# Acknowledgements
I took some ideas inluding the scoring algorithm from
[The best strategies for Wordle](https://sonorouschocolate.com/notes/index.php/The_best_strategies_for_Wordle)
and <https://github.com/alex1770/wordle>

The information-theory math for the entropy guesser is from the video
[Solving Wordle using information theory](https://www.youtube.com/watch?v=v68zYyaEmEA)
