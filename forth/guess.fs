( entropy guesser )

\ ----------------------------------------------------------------
\ Using the ideas from "Solving Wordle using information theory"
\ https://www.youtube.com/watch?v=v68zYyaEmEA
\
\ P = probability that we get a particular score
\ I = information bits, I = log2(1/P) or -log2(P)
\ e.g. P = 1/16, I = 4
\
\ Entropy = sum (P*I) over all possible scores
\ ----------------------------------------------------------------

: flog2 ( F: f -- log2_f )  flog [ 2e flog ] fliteral f/ ;

: probability ( n total -- ) ( F: -- prob )  swap s>f s>f f/ ;
: ibits ( F: prob -- ibits )  flog2 fnegate ;

\ for each score, how many words from the working set got that score
create scored   #scores cells allot

: score-working  ( w -- #words )
    scored #scores cells erase   0 ( #words )
    working @ begin >r
        over r@ swap score  cells scored +  1 swap +!  1+
    r> next? until nip ;

\ calculate the entropy for a guess against all the words in the working set
: entropy ( w -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word from the working set with the highest entropy
: max-entropy ( -- w )    
    0e ( entropy ) working @ ( default ) dup begin
        dup entropy  fover fover f< if  nip dup  fswap  then  fdrop
    next? until fdrop ;

\ find the word with the highest entropy from all words
: max-entropy-all ( -- w )
    0e ( entropy ) 0 ( default ) #words 0 do
        i entropy  fover fover f< if  drop i  fswap  then  fdrop
    loop fdrop ;

\ It takes time for the first guess which is always the same
\   all-words              time max-entropy w.  85.293 sec   tares
\   all-words prune-hidden time max-entropy w.   2.601 sec   raise
\ When we consider all words, "tares" has higher entropy
\   w raise entropy f. 5.91973684251619
\   w tares entropy f. 6.19405254437545
\ The proven best first word is "salet", which we don't prize quite as much:
\   w salet entropy f. 6.01684287539826

w salet value first-guess

\ guess the word with the highest entropy
100 value fence \ entropy is ineffective when the working set gets too small
: guess ( -- w )
    guesses    0= if first-guess exit then
    remaining 1 = if ( only one left ) working @ exit then
    #greens len = if ( we know it )    greens    exit then
    remaining fence <  guesses 5 < and if  max-entropy-all  exit then
    max-entropy ;
