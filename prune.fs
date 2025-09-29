( Pruning the working set )

\ Pruning is similar to scoring and we re-use the scoring table.
\ Instead of taking a guess and target and producing a score,
\ we take a guess and a score and answer whether a target word
\ could have received that score. If not, we can prune it from the
\ working set.

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


: find-match ( c -- a true | false ) \ look for match in target
    scoring cell+ #scoring bounds do
        dup i @ = if ( found ) drop i true unloop exit then
    2 cells +loop  drop false ;

TESTING find-match
T{ w AAAAA w xxxxx init-scoring  'A' find-match -> scoring cell+ true }T
T{ w xxAxx w xxxxx init-scoring  'A' find-match -> 2 2 cells * scoring + cell+ true }T
T{ w xxxxx w xxxxx init-scoring  'A' find-match     -> false  }T
T{ w ABCDE w xxxxx init-scoring  'B' find-match nip -> true }T
T{ w ABCDE w xxxxx init-scoring  'C' find-match nip -> true }T
T{ w ABCDE w xxxxx init-scoring  'D' find-match nip -> true }T
T{ w ABCDE w xxxxx init-scoring  'E' find-match nip -> true }T
T{ w ABCDE w xxxxx init-scoring  'F' find-match     -> false  }T


: prune-yellow? ( score -- f )
    for-scoring do
        3 /mod swap yellow = if
            i 2@ = if ( prune ) drop true unloop exit then
            i @ find-match not if ( prune ) drop true unloop exit then
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


: prune-grey? ( score -- f )
    for-scoring do
        3 /mod swap 0= if
            i @ find-match if ( prune ) 2drop true  unloop exit then
        then
    2 cells +loop   drop false ;

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
    #words 0 do  i has if
        2dup i ww prune? if  i remove  then
    then loop 2drop ;

: prune ( -- )  latest pruner ;
