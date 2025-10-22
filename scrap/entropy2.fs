( endgame entropy for 3 greens )

\ score only the missing letters
: clear-greens ( -- )  len 0 do
    i greens + c@ '-' - if  -1 -2 i 2 cells * scoring + 2!  then loop ;
: clear-greens ;
: score2 ( target guess -- score )
    init-scoring  clear-greens  score-greens  score-yellows ;

\ update scored for each word in the working set
: score-working2  ( w -- #words )
    scored #scores cells erase   0 ( #words )
    working @ begin >r
        over r@  swap score  cells scored +  1 swap +!  1+
    r> next? until nip ;

\ calculate the entropy for a guess against all the words in the working set
: entropy ( w -- ) ( F: -- entropy )
    score-working2 ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word from the working set with the highest entropy
\  : biggest ( w1 w2 -- w1 w2 | w2 w2 ) ( F: e -- e' )
\      dup entropy  fover fover f< if  nip dup  fswap  then  fdrop

: max-entropy ( -- w )    
    0e ( entropy ) working @ ( default ) dup begin
        dup entropy  fover fover f< if  nip dup  fswap  then  fdrop
    next? until fdrop ;

\ find the word with the highest entropy from all words
: max-entropy2 ( -- w )
    0e ( entropy ) working @ ( default ) #words 0 do
        i entropy2  fover fover f< if  drop i  fswap  then  fdrop
    loop fdrop ;
