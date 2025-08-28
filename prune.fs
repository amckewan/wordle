( Pruning the working set )

\ The working set contains the words that could be the solution.
\ One byte per word, 0=absent, 1=present.
\ We start with all the words then prune the set after each score.

create working   #words allot

: all-words  working #words 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

: #working ( -- n )  0 #words 0 do i has + loop ;
\ : .() ( n -- )  ." (" 0 .r ." ) " ;
: .working  0  #words 0 do i has if i ww w. 1+ then loop  ." (" 0 .r ." ) " ;

: prune-green ( w pos -- w/0 )
    2dup + c@  swap guess@ = if ( ok ) drop 0 then ;

: prune-yellow ( w pos -- w/0 )
    2dup + c@  swap guess@ <> if ( ok ) drop 0 then ;

: prune-grey ( w pos -- w/0 )
    2dup + c@  swap guess@ <> if ( ok ) drop 0 then ;

: prune-letter ( w pos -- w/0 )
    dup green?  if  prune-green  else
    dup yellow? if  prune-yellow else
                    prune-grey   then then ;

: prune-word ( w -- w/0 )
    len 0 do
      dup i prune-letter if  unloop exit  then
    loop ( keep it ) drop 0 ;

: prune ( -- )
    #words 0 do  i has if
        i ww prune-word if  i remove  then
    then loop ;





0 [if]
\ Check if a word matches the greens, return true if it doesn't
: -green ( w -- f )  true ( unless we miss a green letter )
    len 0 do  i green? if  over i + c@  i guess + c@ =  and  then loop  nip not ;


\ Prune the working set based on the screen letters
: #greens ( -- f )  0  len 0 do i green? 1 and + loop ;
: green-ok? ( w -- f )  true ( unless we miss a green letter )
    len 0 do  i green? if  over i + c@  i guess + c@ =  and  then loop  nip ;
: prune-green ( -- )
    #greens if ( otherwise don't bother )
      #words 0 do  i has if
        i ww green-ok? not if  i remove  then
    then loop then ;

\ For a yellow score, we remove words that either have the letter
\ at that position (would be scored green) or don't have that letter at all.

\ Check if a word has this letter in any position.
\ Ignore green and yellow letters.
: has-letter ( char w -- f ) \ FLAWED!
    len 0 do
        over i used + c@ =  i green? or  not if
\        i grey? if
            2dup i + c@ = if ( found one ) 2drop true  unloop exit then
        then
    loop 2drop false ;

: #letters ( c w -- n )  0 swap  len bounds do ( c n ) over i c@ = 1 and +  loop  nip ;

: non-green-letters ( c w -- n )
    0   len 0 do  >r  2dup i + c@ =  i green? not and  1 and  r> +  loop  nip nip ;


: -yellow ( w -- f )
    len 0 do  i yellow? if
      \ check that we don't match the guess at that position (otherwise it would be greeen)
      dup i + c@  guess i + c@ = if  drop true  unloop exit then

      \ make sure there is at least one of those letters somewhere else in the word
      \ ignore green letters since they can't satisfy the yellow score
      dup i + c@  over non-green-letters 1 < if  drop true  unloop exit then

    then loop drop false ;


\ Since we don't track yellows yet, we can only remove those words
\ that have a match in the yellow position (which otherwise would be green).
: yellow-ok? ( w -- f ) 
    true  len 0 do  i yellow? if  over i + c@  guess i + c@ <>  and  then loop  nip ;
: prune-yellow
    #words 0 do  i has if
        i ww yellow-ok? not if  i remove  then
    then loop ;

\ For grey, we will need to remember our yellow guesses so we don't remove
\ words that have the grey letter if it scored yellow.
\ For now, just remove the word with the letter in the grey position.
: grey-ok? ( w -- f ) 
    true  len 0 do  i grey? if  over i + c@  guess i + c@ <>  and  then loop  nip ;

\ count occurrances of each letter (used for grey pruning)
create letters 26 allot
: >letter ( c -- a )  [char] A -  letters + ;
: count-letters ( w -- )  letters 26 erase
    len bounds do  i c@ >letter  dup c@ 1+ swap c!  loop ;

\ This has the number of green and yellow letters also in the guess
create greys len allot
: prepare-greys ( -- ) ;

: -grey ( w -- f )
    len 0 do  i grey? if
      dup c@ guess i + c@ = if  drop true  unloop exit then
      ( todo... )
    then 1+ loop drop false ;





: prune-grey
    #words 0 do  i has if
        i ww -grey if  i remove  then
    then loop ;


: prune  prune-green  prune-yellow  prune-grey ;

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

( === Test green-ok? === )
: expect-green-ok      test dup green-ok? not if fail ." expect green ok " w. else drop then ;
: expect-green-not-ok  test dup green-ok?     if fail ." expect green not ok " w. else drop then ;

: test-green-ok
    s" green-ok?" begin-unit-tests
    [W] ABCDE guess w!
    [W] GG--- score w!
    [W] ABxyz expect-green-ok
    [W] ABCDE expect-green-ok
    [W] Axxxx expect-green-not-ok
    [W] xBxxx expect-green-not-ok
    [W] xxxxx expect-green-not-ok
    report-unit-tests ;
test-green-ok

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

( === Test yellow-ok? === )
: expect-yellow-ok      test dup yellow-ok? not if fail ." expect yellow ok " w. else drop then ;
: expect-yellow-not-ok  test dup yellow-ok?     if fail ." expect yellow not ok " w. else drop then ;

: test-yellow-ok
    s" yellow-ok?" begin-unit-tests

    [W] ABCDE guess w!
    [W] YY--- score w!
    [W] xxAxx expect-yellow-ok
    [W] xxBxx expect-yellow-ok
    [W] xxBxx expect-yellow-ok
    [W] xxxAB expect-yellow-ok
    [W] Axxxx expect-yellow-not-ok
    [W] xBxxx expect-yellow-not-ok
    [W] ABxxx expect-yellow-not-ok

    report-unit-tests ;
test-yellow-ok


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


0 [if]
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
[then]

\ forget-unit-tests
[then]
