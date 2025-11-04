( fixed guess )

: maybe-count 
    guess ( the last one ) vowels 2 <
    if [w] COUNT else tally-guess then ;

\ Always start with RAISE and COUNT (unless we have 2 vowels)
: simple-guess ( -- w )
    guesses 0=  if [w] RAISE else
    guesses 1 = if maybe-count else
    tally-guess then then ;
