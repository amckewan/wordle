( Pruning the working set )

\ The working set contains the words that could be the solution.
\ One byte per word, 0=absent, 1=present.
\ We start with all the words then prune the set after each score.

#solution-words constant #words ( start easy )

create working   #words allot

: all-words  working #words 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

: #working ( -- n )  0 #words 0 do i has + loop ;
: .working  0  #words 0 do i has if i ww w. 1+ then loop  . ." words " ;

\ Prune green if the letter at pos doesn't match the guess
: match-guess ( w pos -- f )    swap over + c@  swap guess@ = ;
: prune-green ( w pos -- w/0 )  over swap match-guess not and ;

\ For yellow, we first check to make sure the letter is not at that position.
\ Then look at the whole word and check that there are enough greys to satisfy
\ the yellow scores.
: yellow-scores ( c -- n )
    0  len 0 do  i yellow? if over i guess@ = - then  loop nip ;
: grey-guesses ( w c -- n )
    0  len 0 do
      i grey? if  i 2>r  over r> + c@ over = negate  r> +   then
    loop nip nip ;
: prune-yellow ( w pos -- w/0 )
    2dup match-guess if ( prune ) drop else
    guess@  2dup grey-guesses  over yellow-scores < nip and  then ;

\ For grey, check that there are none of this letter in any grey spots
: prune-grey ( w pos -- w/0 )  over swap guess@ grey-guesses 0> and ;

\ Prune all the words letter-by-letter
: prune-letter ( w pos -- w/0 )
    dup green?  if  prune-green  else
    dup yellow? if  prune-yellow else
                    prune-grey   then then ;
: prune-word ( w -- w/0 )
    len 0 do  dup i prune-letter if unloop exit then  loop ( ok ) drop 0 ;
: prune ( -- )
    #words 0 do  i has if  i ww prune-word if i remove then  then loop ;


( === Unit Tests === )

TESTING #WORKING
T{ all-words #working -> #words }T
T{ working 10 erase  #working -> #words 10 - }T
T{ 0 working #words + 1- c!  #working -> #words 11 - }T
T{ working #words erase  #working -> 0 }T

TESTING PRUNE-GREEN
   w ABCDE guess w!
   w GG--- score w!
T{ w ABCDE 0 prune-green -> 0 }T
T{ w ABxxx 0 prune-green -> 0 }T
T{ w xBxxx 0 prune-green 0= -> 0 }T
T{ w Axxxx 1 prune-green 0= -> 0 }T

TESTING PRUNE-YELLOW
   w ABCDE guess w!
   w YY--- score w!
T{ w AAAAA 0 prune-yellow 0= -> 0 }T
T{ w xAAAA 0 prune-yellow -> 0 }T
T{ w xxAxx 0 prune-yellow -> 0 }T
T{ w xBxxx 1 prune-yellow 0= -> 0 }T
T{ w xxxxB 1 prune-yellow -> 0 }T

   w AACDE guess w!
   w YY--- score w!
T{ w xxAxx 0 prune-yellow 0= -> 0 }T
T{ w xxAAx 0 prune-yellow -> 0 }T

0 [if]

( === Test prune-grey === )
: expect-grey-ok     test 2dup prune-grey     if s" grey ok"    expected swap w. . else 2drop then ;
: expect-grey-prune  test 2dup prune-grey not if s" grey prune" expected swap w. . else 2drop then ;

: test-prune-grey
    s" prune-grey" begin-unit-tests

    [w] AABCD guess w!
    [w] G---- score w!
    [w] Axxxx 1 expect-grey-ok
    [w] AAxxx 1 expect-grey-prune
    [w] AxxxA 1 expect-grey-prune
    [w] ABxxx 2 expect-grey-prune
    [w] AxBxx 2 expect-grey-prune

    report-unit-tests ;
test-prune-grey

forget-unit-tests
[then]
