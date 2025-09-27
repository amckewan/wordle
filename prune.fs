( Pruning the working set )

\ pruning is similar to scoring and we re-use the scoring table
\ target = the word we are evaluating for pruning
\ guess = the guess we made
\ the score for the guess passed on the stack

\  : prune-green? ( score -- f )
\      false swap  len 0 do
\          3 /mod swap green = if
\              ( green score, prune if guess and target don't match )
\              i match not if
\                  2drop true ( prune ) unloop exit
\              then
\              i i mark-used ( ok, mark letter used )
\          then
\      loop drop ;

: prune-green? ( score -- f )
    for-scoring do
        3 /mod swap green = if
            ( prune if guess and target don't match )
            i 2@ - if  drop true  unloop exit  then
            -1 -2 i 2! ( ok, mark letter used )
        then
    2 cells +loop   drop false ;

TESTING PRUNE-GREEN?
\    guess   score   target
T{ w ABCDE s G-G-G w AxCxE  rot init-scoring  prune-green? -> false }T
T{ w ABCDE s G-G-G w BBCDE  rot init-scoring  prune-green? -> true  }T
T{ w ABCDE s G-G-G w ABBDE  rot init-scoring  prune-green? -> true  }T
T{ w ABCDE s G-G-G w ABCDD  rot init-scoring  prune-green? -> true  }T
T{ w ABCDE s G-G-G w ABCDE  rot init-scoring  prune-green? -> false }T
T{ 0 2 cells * scoring + @ 0< -> true  }T
T{ 1 2 cells * scoring + @ 0< -> false }T
T{ 2 2 cells * scoring + @ 0< -> true  }T
T{ 3 2 cells * scoring + @ 0< -> false }T
T{ 4 2 cells * scoring + @ 0< -> true  }T


: -match ( c -- a false | true ) \ look for match in target
    scoring cell+ #scoring bounds do
        dup i @ = if ( found ) drop i false unloop exit then
    2 cells +loop ;

TESTING -MATCH
T{ w AAAAA w xxxxx init-scoring  'A' -match -> scoring cell+ false }T
T{ w xxAxx w xxxxx init-scoring  'A' -match -> 2 2 cells * scoring + cell+ false }T
T{ w xxxxx w xxxxx init-scoring  'A' -match 0<> -> true  }T
T{ w ABCDE w xxxxx init-scoring  'B' -match nip -> false }T
T{ w ABCDE w xxxxx init-scoring  'C' -match nip -> false }T
T{ w ABCDE w xxxxx init-scoring  'D' -match nip -> false }T
T{ w ABCDE w xxxxx init-scoring  'E' -match nip -> false }T
T{ w ABCDE w xxxxx init-scoring  'F' -match 0<> -> true  }T

\  : prune-yellow? ( score -- f )
\      len 0 do  3 /mod swap yellow = if
\          i match if ( prune ) 2drop true unloop exit then
\          i guess + c@ target find-match if ( ok ) i mark-used else
\              ( prune ) drop true unloop exit
\          then
\      then
\      loop   drop false ;

: prune-yellow? ( score -- f )
    for-scoring do
        3 /mod swap yellow = if
            i 2@ = if ( prune ) drop true unloop exit then
            i @ -match if ( prune ) drop true unloop exit then
            -1 swap ! ( mark )
        then
    2 cells +loop   drop false ;

TESTING PRUNE-YELLOW?
\    guess   score   target
T{ w ABCDE s YY--- w AAAAA  rot init-scoring  prune-yellow? -> true }T
T{ w ABCDE s YY--- w xxAAA  rot init-scoring  prune-yellow? -> true }T
T{ w ABCDE s YY--- w xxBBB  rot init-scoring  prune-yellow? -> true }T
T{ w ABCDE s YY--- w xxABx  rot init-scoring  prune-yellow? -> false }T
T{ w ABCDE s YY--- w BAxxx  rot init-scoring  prune-yellow? -> false }T
T{ w ABCDE s YY--- w BABAB  rot init-scoring  prune-yellow? -> false }T

T{ w AACDE s YY--- w xxAxx  rot init-scoring  prune-yellow? -> true }T
T{ w AACDE s YY--- w xxAAx  rot init-scoring  prune-yellow? -> false }T
T{ w EERIE s Y--Y- w VIXEN  rot init-scoring  prune-yellow? -> false }T

0 [if]
: prune-grey? ( score -- f )  false swap
    len 0 do  3 /mod swap 0= if
        i guess + c@ target find-match if
            ( prune ) drop 2drop true unloop exit
        then
    then loop drop ;

TESTING PRUNE-GREY?
\    guess   score   target
T{ w ABCDE s ----- w xxxxx  rot init-scoring  prune-grey? -> false }T
T{ w ABCDE s ----- w xAxxx  rot init-scoring  prune-grey? -> true }T
T{ w ABCDE s ----- w xxxxA  rot init-scoring  prune-grey? -> true }T
T{ w ABCDE s ----- w Cxxxx  rot init-scoring  prune-grey? -> true }T

: prune? ( guess score target -- f )
    rot init-scoring
    dup prune-green?  if true else
    dup prune-yellow? if true else
    dup prune-grey?   then then nip ;


TESTING PRUNE?
\    guess   score   target
T{ w EERIE s Y--Y- w VIXEN  prune? -> false }T
T{ w EERIE s Y--Y- w xIExx  prune? -> false }T
T{ w EERIE s Y--Y- w Exxxx  prune? -> true }T
T{ w EERIE s Y--Y- w xxxIx  prune? -> true }T


\ Prune the working set, removing words that wouldn't get this score
: pruner ( guess score -- )
    working-size 0 do  i has if
        2dup i ww prune? if  i remove  then
    then loop 2drop ;

: prune ( -- )  guesses 1- hist@ pruner ;

[then]



0 [if]
=================================


   w ABCDE to guess
   w YY--- to score
T{ w AAAAA -u prune-yellow -> true  }T
T{ w xxAAA -u prune-yellow -> true  }T
T{ w xxBBB -u prune-yellow -> true  }T
T{ w xxABx -u prune-yellow -> false }T
T{ w BAxxx -u prune-yellow -> false }T
T{ w BABAB -u prune-yellow -> false }T

   w AACDE to guess
   w YY--- to score
T{ w xxAxx -u prune-yellow -> true  }T
T{ w xxAAx -u prune-yellow -> false }T

   w EERIE to guess
   w Y--Y- to score
T{ w VIXEN -u prune-yellow -> false }T

: find-unused ( w l -- pos t | f ) \ find the first unused pos in w that matches
    len 0 do i used? not if
        over i get over = if 2drop i true unloop exit then
    then loop 2drop false ;

: prune-yellow ( w -- f )  false ( default ok )
    len 0 do  yellow i scored if
        \ prune if the word has the guessed letter at this position
        over guess i match if ( prune ) invert leave then
        \ prune if we can't find a matching unused letter (else mark it used)
        over guess i get find-unused if used! else ( prune ) invert leave then
    then loop nip ;

: prune-grey ( w -- f )  false ( ok )
    len 0 do  grey i scored if
        \ prune if there are any of this letter in the unused positions
        over guess i get find-unused if ( prune ) drop not leave then
    then loop nip ;


: prune?  ( guess score target -- f )
    rot init-scoring
    prune-green?  if true exit then
    prune-yellow? if true exit then
    prune-grey? ;

: wfind ( c w -- pos t | f )
    len 0 do
        2dup i + c@ = if 2drop i true unloop exit then
    loop 2drop false ;

\ build a mask that will exclude non-green letters found in score
\ create lookup table and use that so we do the hard word at compile time
create gmasks #scores cells allot
marker forget
: gmask ( score -- mask )     0 ( mask )
    len 0 do
        swap 3 /mod swap ( m s' color )
        green = cmask and i left ( letter mask )
        rot or ( s' m' )
    loop nip ;

TESTING GMASK
T{ 0 gmask -> 0 }T
T{ s ----- gmask -> 0 }T
T{ s g---- gmask -> cmask }T
T{ s g-yy- gmask -> cmask }T
T{ s --g-- gmask -> cmask 2 left }T
T{ s ggggg gmask -> $1FFFFFF ( 25 bits ) }T

: fill-it #scores 0 do  i gmask  i cells gmasks + !  loop ;
fill-it forget
: gmask ( score -- mask ) cells gmasks + @ ;


: -green ( guess score w -- gmask f ) \ non-zero if w should be pruned
    rot xor ( 0=green ) swap gmask  swap over and ;

TESTING -GREEN
T{ w AAAAA s G---- w Axxxx -green 0<>     -> cmask false }T
T{ w AAAAA s G---- w Bxxxx -green 0<> nip ->       true  }T




0 [if]
0 value pguess
0 value pscore      \ the score
0 value pruned      \ letter mask, set if this letter has been used
\ re-use score and used from score.fs

\ carry all four of these? use pick? locals?
: -yellow ( w -- f )
    4-0 do
        i pscore + c@ yellow = if
            dup pguess i match if ( prune ) drop true unloop exit then
            
        then
    -1 +loop drop false ;

create pruning len allot ( the word )
create guess len allot
create score len allot
create used len allt

\ find the first unused pos in w that matches
: unused ( l -- pos t | f )   false swap
    len 0 do i used + c@ 0= if
       dup i pruning + c@ = if i true rot leave then
    then loop drop ;

: -yellow ( -- f )  false ( assume ok )
    len 0 do  i score + c@ yellow = if
        \ prune if the word has the guessed letter at this position
        i pruning + c@ i guess + c@ = if ( prune ) invert leave then
        \ prune if we can't find a matching unused letter (else mark it used)
        i guess + c@ unused if cmask swap used + c! else ( prune ) invert leave then
    then loop ;


: unused ( w l -- pos t | f ) \ find the first unused pos in w that matches
    len 0 do i used + c@ 0= if
        over i get over = if 2drop i true unloop exit then
    then loop 2drop false ;

: prune-yellow ( w -- f )  false ( default ok )
    len 0 do ( w f ) i score + c@ yellow = if
        \ prune if the word has the guessed letter at this position
        over i get i guess + c@ = if ( prune ) invert leave then
\        over guess i match if ( prune ) invert leave then
        \ prune if we can't find a matching unused letter (else mark it used)
        over i guess + c@ unused if cmask swap used + c! else ( prune ) invert leave then
\        over guess i get unused if used! else ( prune ) invert leave then
    then loop nip ;

\ Using 'score' and 'used' from score.fs as local variables
\ it's more convenient using these as arrays
\ then we only need guess and w

: prune? ( guess score w -- f )   swap >r
    2dup xor r@ gmask and if ( prune green ) 2drop true  r> drop exit then
    r>  dup score s!  gmask used w!  swap guess w! ( -- w )

    swap dup gmask to pruned to pscore ( module locals ) \ or locals



    \ >r 2dup r@ -green if r> drop  2drop true exit then
    ( guess score gmask, w on rstack )
    \  swap score!
;




: score-greens ( secret guess -- ) \ sets all of score and used
    xor ( 0=green ) len 0 do
        dup i mask and 0= ( green? ) green and  dup i score + c!  i used + c!   
    loop drop ;


\ we prune a word if it doesn't yield the same score
\ could this word have got the same score?
\ for a green score, w must have that same letter
\ for a yellow score
: prune? ( guess score w -- f )
    \ 1. it has to match the greens
    \ mask out non-green chars (according to the score) then =
    \ here score as 5 bytes or whatever is useful...


: prune# ( guess score -- n ) \ how many left if we got this guess/score?
    #words 0 do i has if i ww prune-word if i remove then then loop ;





: prune-green? ( w -- f ) \ 
: prune-green  ( w -- f )  false ( default ok )
    len 0 do  green i scored if
        \ prune if the green letter doesn't match the guess, marking it used
        over guess i match if i used! else ( prune ) invert leave then
    then loop nip ;

: find-unused ( w l -- pos t | f ) \ find the first unused pos in w that matches
    len 0 do i used? not if
        over i get over = if 2drop i true unloop exit then
    then loop 2drop false ;

: prune-yellow ( w -- f )  false ( default ok )
    len 0 do  yellow i scored if
        \ prune if the word has the guessed letter at this position
        over guess i match if ( prune ) invert leave then
        \ prune if we can't find a matching unused letter (else mark it used)
        over guess i get find-unused if used! else ( prune ) invert leave then
    then loop nip ;

: prune-grey ( w -- f )  false ( ok )
    len 0 do  grey i scored if
        \ prune if there are any of this letter in the unused positions
        over guess i get find-unused if ( prune ) drop not leave then
    then loop nip ;

: prune-word ( w -- f )  0 to used
    dup prune-green not if dup prune-yellow not if prune-grey then then 0<> ;

: prune  #words 0 do i has if i ww prune-word if i remove then then loop ;


( === TESTS === )
: -u  0 to used ;



TESTING prune-green
   w ABCDE to guess
   w G-G-G to score
T{ w AxCxE -u prune-green -> false }T
T{ w BBCDE -u prune-green -> true  }T
T{ w ABBDE -u prune-green -> true  }T
T{ w ABCDD -u prune-green -> true  }T
T{ w ABCDE -u prune-green -> false }T
T{ 0 used? -> 1 }T
T{ 1 used? -> 0 }T
T{ 2 used? -> 1 }T
T{ 3 used? -> 0 }T
T{ 4 used? -> 1 }T

TESTING find-unused
: useit ( w -- )  -u  len 0 do dup i get 16 - if i used! then loop drop ;
T{ w 11000 useit w AAAAA char A '@' - find-unused -> 2 true }T
T{ w 11000 useit w AAxxA char A '@' - find-unused -> 4 true }T
T{ w 11000 useit w AADBC char A '@' - find-unused -> false }T
T{ w 00000 useit w ABCDE char A '@' - find-unused -> 0 true }T
T{ w 00000 useit w ABCDE char B '@' - find-unused -> 1 true }T
T{ w 00000 useit w ABCDE char E '@' - find-unused -> 4 true }T
T{ w 11111 useit w ABCDE char A '@' - find-unused -> false }T
T{ w 11111 useit w ABCDE char C '@' - find-unused -> false }T
T{ w 11111 useit w ABCDE char D '@' - find-unused -> false }T

TESTING prune-yellow
   w ABCDE to guess
   w YY--- to score
T{ w AAAAA -u prune-yellow -> true  }T
T{ w xxAAA -u prune-yellow -> true  }T
T{ w xxBBB -u prune-yellow -> true  }T
T{ w xxABx -u prune-yellow -> false }T
T{ w BAxxx -u prune-yellow -> false }T
T{ w BABAB -u prune-yellow -> false }T

   w AACDE to guess
   w YY--- to score
T{ w xxAxx -u prune-yellow -> true  }T
T{ w xxAAx -u prune-yellow -> false }T

   w EERIE to guess
   w Y--Y- to score
T{ w VIXEN -u prune-yellow -> false }T

TESTING prune-word
   w EERIE to guess
   w Y--Y- to score
T{ w VIXEN prune-word -> false }T
T{ w xIExx prune-word -> false }T
T{ w Exxxx prune-word -> true  }T
T{ w xxxIx prune-word -> true  }T

[then]
[then]
