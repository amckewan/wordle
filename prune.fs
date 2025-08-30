( Pruning the working set )

\ The working set contains the words that could be the solution.
\ One byte per word, 0=absent, 1=present.
\ We start with all the words then prune the set after each score.
\ This code uses 'guess' and 'score' from the game.

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
    2dup match-guess if ( prune ) drop exit then
    ( w pos ) guess@
    ( w c ) 2dup grey-guesses
    ( w c grey ) over yellow-scores
    ( w c grey yellow ) < nip and ;


\ For a grey score, check that this letter doesn't match (score green) and
\ there are none of this letter in other grey spots (score yellow)
: prune-grey ( w pos -- w/0 )
    2dup match-guess if ( prune) drop else ( w pos ) over swap guess@ ( w w c ) grey-guesses 0> and then ;

: prune-letter ( w pos -- w/0 )
    dup green?  if  prune-green  else
    dup yellow? if  prune-yellow else
                    prune-grey   then then ;

: prune-word ( w -- w/0 )
    len 0 do
      dup i prune-letter if  unloop exit  then
    loop ( ok ) drop 0 ;

: prune ( -- )
    #words 0 do  i has if
        i ww prune-word if  i remove  then
    then loop ;


( === Unit Tests === )
include unit-test.fs

: test-#working
    s" #working" begin-unit-tests
    all-words                           #working #words expect-equal
    working 10 bounds do  0 i c!  loop  #working #words 10 - expect-equal
    0 working #words + 1- c!            #working #words 11 - expect-equal
    working #words erase                #working 0 expect-equal
    report-unit-tests ;
test-#working

( === Test prune-green === )
: expect-green-ok    ( w pos -- )  test 2dup prune-green     if s" green ok"    expected swap w. . else 2drop then ;
: expect-green-prune ( w pos -- )  test 2dup prune-green not if s" green prune" expected swap w. . else 2drop then ;

: test-prune-green
    s" prune-green" begin-unit-tests
    [W] ABCDE guess w!
    [W] GG--- score w!
    [W] ABxxx 0 expect-green-ok
    [W] ABxxx 1 expect-green-ok
    [W] Axxxx 1 expect-green-prune
    [W] xBxxx 0 expect-green-prune
    [W] ABCDE 3 expect-green-ok
    report-unit-tests ;
test-prune-green

( === Test prune-yellow === )
: expect-yellow-ok     test 2dup prune-yellow     if s" yellow ok"    expected swap w. . else 2drop then ;
: expect-yellow-prune  test 2dup prune-yellow not if s" yellow prune" expected swap w. . else 2drop then ;

: test-prune-yellow
    s" prune-yellow" begin-unit-tests

    [W] ABCDE guess w!
    [W] YY--- score w!
    [W] Axxxx 0 expect-yellow-prune
    [W] xBxxx 1 expect-yellow-prune
    [W] xxAxx 0 expect-yellow-ok
    [W] xxAAA 0 expect-yellow-ok
    [W] xxxxB 1 expect-yellow-ok

    [W] AACDE guess w!
    [W] YY--- score w!
    [W] xxAxx 0 expect-yellow-prune
    [W] xxAAx 0 expect-yellow-ok

    report-unit-tests ;
test-prune-yellow


0 [if]
( === Test prune-green === )
: test-prune-green
    s" prune-green" begin-unit-tests

    \ remove all but the 23 words that start with Q
    all-words
    [W] QUERT guess w!
    [W] G---- score w!
    prune-green
    #working 23 expect-equal

    \ there are 3 Zs
    all-words
    [W] ZUERT guess w!
    [W] G---- score w!
    prune-green
    #working 3 expect-equal

    \ there are 424 words ending in E
    all-words
    [W] ABCDE guess w!
    [W] ----G score w!
    prune-green
    #working 424 expect-equal

    report-unit-tests ;
test-prune-green


( === Test prune-yellow === )
: test-prune-yellow
    s" prune-yellow" begin-unit-tests

    \ remove the 23 words that start with Q
    all-words
    [W] QUERT guess w!
    [W] Y---- score w!
    prune-yellow
    #working #words 23 - expect-equal

    \ there are 3 Zs
    all-words
    [W] ZUERT guess w!
    [W] Y---- score w!
    prune-yellow
    #working #words 3 - expect-equal

    \ there are 424 words ending in E
    all-words
    [W] ABCDE guess w!
    [W] ----Y score w!
    prune-yellow
    #working #words 424 - expect-equal

    report-unit-tests ;
test-prune-yellow


( === Test has-letter === )
: expect-has ( w char -- )  test  swap 2dup has-letter not
  if fail ." expected " w. ." to contain " emit  else 2drop then ;

: expect-has-not ( w char -- )  test  swap 2dup has-letter
  if fail ." expected " w. ." not to contain " emit  else 2drop then ;

: test-has-letter
  s" has-letter" begin-unit-tests

  clear-score ( to avoid side effects )
  [w] ABCDE [char] A expect-has
  [w] ABCDE [char] B expect-has
  [w] ABCDE [char] E expect-has
  [w] ABCDE [char] M expect-has-not

  \ make sure we skip greens
  green score c! ( score the first letter green )
  [w] ABCDE [char] A expect-has-not
  [w] ABCDA [char] A expect-has
  [w] ABCDE [char] B expect-has

  report-unit-tests ;
test-has-letter

( === Test -yellow === )
: expect-something ( w f a n -- )  rot if 2drop drop exit then
    fail ." guess " guess w. ." score " score w. ." word " rot w. ." Expected " type space ;

: expect-yellow-ok      dup -yellow not s" yellow ok"     expect-something ;
: expect-yellow-not-ok  dup -yellow     s" yellow not ok" expect-something ;

: expect-grey-ok        dup -grey not   s" grey ok"       expect-something ;
: expect-grey-not-ok    dup -grey       s" grey not ok"   expect-something ;

: test-yellow  s" -yellow" begin-unit-tests
    [w] AOONA guess w!
    [w] G---Y score w!
    [w] AAxxx expect-yellow-ok
    [w] AAAxx expect-yellow-ok
    [w] xxxxx expect-yellow-not-ok
    [w] Axxxx expect-yellow-not-ok ( need 1 more yellow )
    [w] xxxxA expect-yellow-not-ok ( should be green )

    [w] AAOOA guess w!
    [w] GY--- score w!
    [w] Axxxx expect-yellow-not-ok
    [w] AAxxx expect-yellow-not-ok
    [w] AxAxx expect-yellow-not-ok
    [w] AxAAx expect-yellow-ok

    report-unit-tests ;
test-yellow


( === Test -grey === )
: test-grey  s" -grey" begin-unit-tests
    \ NOTE: scores must be correct for G and Y which we do first
\   [w] ABACK secret w!
    [w] AOONA guess w!
    [w] G---Y score w!
    [w] xxxxx expect-grey-ok
    [w] Axxxx expect-grey-ok
    [w] AAxxx expect-grey-ok
    [w] AAAxx expect-grey-ok
    [w] xxxxA expect-grey-not-ok

    [w] xOxxx expect-grey-not-ok
    [w] xxxOx expect-grey-ok
    [w] xxxxO expect-grey-not-ok ( should score 1 yellow )

\   [w] AxAxx secret w!
    [w] AAOOA guess w!
    [w] GY--- score w!
    [w] Axxxx expect-grey-ok
    [w] AAxxx expect-grey-not-ok
    [w] AxAxx expect-grey-ok
    [w] AxAAx expect-grey-not-ok


    report-unit-tests ;
test-grey



( === Test prune-grey === )
: test-prune-grey
    s" prune-grey" begin-unit-tests

    all-words
    clear-score
    [char] Q prune-grey
    #working #words 29 - expect-equal

    \ 23 words start with Q, so if we mark the first
    \ letter green, we should only remove the other 6.
    all-words
    clear-score
    green score c!
    [char] Q prune-grey
    #working #words 6 - expect-equal

    \ word 3 is ABBEY, 4 is ABBOT
    \ we have just guessed xBxBB which scored -G-Y-
    \ when we prune the last grey B, we should not remove
    \ ABBEY or ABBOT
    all-words
    clear-score
    [w] ABBEY secret w!
    [w] xBxBB guess w!
    [w] -G-Y- score w!
    [char] B 2 used!

    [char] B 1 prune-green
    [char] B 3 prune-yellow
    #working 2 expect-equal
    [char] B prune-grey
    #working 2 expect-equal

    report-unit-tests ;
test-prune-grey

forget-unit-tests
[then]
