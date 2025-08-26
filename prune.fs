( Pruning the working set )

\ The working set contains the words that could be the solution.
\ One byte per word, 0=absent, 1=present
\ We start with all the words then prune the set based on each score.

create working  #words allot

: all-words  working fill-set ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working +  0 swap c! ;

: any-green? ( -- f ) \ true if there are any greens in the score
    false  len 0 do  i green? or  loop ;

: green-ok? ( w -- f ) \ w must match the green guess letters
    true  len 0 do  i green? if  over i + c@  guess i + c@ =  and  then loop  nip ;

: prune-green
    any-green? if ( otherwise don't bother )
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

\ Since we don't track yellows yet, we can only remove those words
\ that have a match in the yellow position (which otherwise would be green).
: yellow-ok? ( w -- f ) 
    true  len 0 do  i yellow? if  over i + c@  guess i + c@ <>  and  then loop  nip ;

: prune-yellow
    #words 0 do  i has if
        i ww yellow-ok? not if  i remove  then
    then loop ;


\ : -yellow ( char pos w -- char pos )
\     >r 2dup r@ + c@ <> if  over r@ has-letter if ( ok) r> drop exit  then then
\     r> working remove ;

\ : prune-yellow ( char pos -- )  ['] -yellow working for-each  2drop ;

\ For grey, we remove words that do have that letter.

\ : -grey ( char w -- char )  2dup has-letter if working remove else drop then ;

\ : prune-grey ( char -- )  ['] -grey working for-each  drop ;

: prune-grey ( todo ) ;


( === Unit Tests === )
include unit-test.fs

: #working ( -- n )  working set-size ;

( === Test green-ok? === )
: expect-green-ok      test dup green-ok? not if fail ." expect green ok " w. else drop then ;
: expect-green-not-ok  test dup green-ok?     if fail ." expect green not ok " w. else drop then ;

: test-green-ok
    cr ." Testing green-ok?..." begin-unit-tests

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
    cr ." Testing prune-green..." begin-unit-tests

    \ remove all but the 23 words that start with Q
    all-words
    [W] QUERT guess w!
    [W] G---- score w!
    prune-green
    working set-size 23 expect-equal

    \ there are 3 Zs
    all-words
    [W] ZUERT guess w!
    [W] G---- score w!
    prune-green
    working set-size 3 expect-equal

    \ there are 424 words ending in E
    all-words
    [W] ABCDE guess w!
    [W] ----G score w!
    prune-green
    working set-size 424 expect-equal

    report-unit-tests ;
test-prune-green

( === Test yellow-ok? === )
: expect-yellow-ok      test dup yellow-ok? not if fail ." expect yellow ok " w. else drop then ;
: expect-yellow-not-ok  test dup yellow-ok?     if fail ." expect yellow not ok " w. else drop then ;

: test-yellow-ok
    cr ." Testing yellow-ok?..." begin-unit-tests

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
    cr ." Testing prune-yellow..." begin-unit-tests

    \ remove the 23 words that start with Q
    all-words
    [W] QUERT guess w!
    [W] Y---- score w!
    prune-yellow
    working set-size #words 23 - expect-equal

    \ there are 3 Zs
    all-words
    [W] ZUERT guess w!
    [W] Y---- score w!
    prune-yellow
    working set-size #words 3 - expect-equal

    \ there are 424 words ending in E
    all-words
    [W] ABCDE guess w!
    [W] ----Y score w!
    prune-yellow
    working set-size #words 424 - expect-equal

    report-unit-tests ;
test-prune-yellow


( === Test has-letter === )
: expect-has ( w char -- )  test  swap 2dup has-letter not
  if fail ." expected " w. ." to contain " emit  else 2drop then ;

: expect-has-not ( w char -- )  test  swap 2dup has-letter
  if fail ." expected " w. ." not to contain " emit  else 2drop then ;

: test-has-letter
  cr ." Testing has-letter..." begin-unit-tests

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


0 [if]
( === Test prune-grey === )
: test-prune-grey
    cr ." Testing prune-grey..." begin-unit-tests

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
