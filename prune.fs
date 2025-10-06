( Pruning the working set )

\ Pruning is similar to scoring and we re-use the scoring table.
\ Instead of taking a guess and target and producing a score,
\ we take a guess and a score and answer whether a target word
\ could have received that score. If not, we can remove it from
\ the working set.

: prune-green? ( score -- f )
    for-scoring do
        3 /mod swap green = if
            ( prune if guess and target don't match )
            i 2@ - if  drop true  unloop exit  then
            -1 -2 i 2! ( ok, mark letter used )
        then
    2 cells +loop   drop false ;

: find-match ( c -- a true | false ) \ look for match in target
    scoring cell+ #scoring bounds do
        dup i @ = if ( found ) drop i true unloop exit then
    2 cells +loop  drop false ;

: prune-yellow? ( score -- f )
    for-scoring do
        3 /mod swap yellow = if
            i 2@ = if ( prune ) drop true unloop exit then
            i @ find-match not if ( prune ) drop true unloop exit then
            -1 swap ! ( mark )
        then
    2 cells +loop   drop false ;

: prune-grey? ( score -- f )
    for-scoring do
        3 /mod swap 0= if
            i @ find-match if ( prune ) 2drop true  unloop exit then
        then
    2 cells +loop   drop false ;

: prune? ( guess score target -- f )
    rot init-scoring
    dup prune-green?  if true else
    dup prune-yellow? if true else
    dup prune-grey?   then then nip ;



( ===== TESTS ===== )

testing prune-green?
\    guess   score   target
t{ w abcde s g-g-g w axcxe  rot init-scoring  prune-green? -> false }t
t{ w abcde s g-g-g w bbcde  rot init-scoring  prune-green? -> true  }t
t{ w abcde s g-g-g w abbde  rot init-scoring  prune-green? -> true  }t
t{ w abcde s g-g-g w abcdd  rot init-scoring  prune-green? -> true  }t
t{ w abcde s g-g-g w abcde  rot init-scoring  prune-green? -> false }t
t{ 0 2 cells * scoring + @ 0< -> true  }t
t{ 1 2 cells * scoring + @ 0< -> false }t
t{ 2 2 cells * scoring + @ 0< -> true  }t
t{ 3 2 cells * scoring + @ 0< -> false }t
t{ 4 2 cells * scoring + @ 0< -> true  }t

testing find-match
t{ w aaaaa w xxxxx init-scoring  'a' find-match -> scoring cell+ true }t
t{ w xxaxx w xxxxx init-scoring  'a' find-match -> 2 2 cells * scoring + cell+ true }t
t{ w xxxxx w xxxxx init-scoring  'a' find-match     -> false  }t
t{ w abcde w xxxxx init-scoring  'b' find-match nip -> true }t
t{ w abcde w xxxxx init-scoring  'c' find-match nip -> true }t
t{ w abcde w xxxxx init-scoring  'd' find-match nip -> true }t
t{ w abcde w xxxxx init-scoring  'e' find-match nip -> true }t
t{ w abcde w xxxxx init-scoring  'f' find-match     -> false  }t

testing prune-yellow?
\    guess   score   target
t{ w abcde s yy--- w aaaaa  rot init-scoring  prune-yellow? -> true }t
t{ w abcde s yy--- w xxaaa  rot init-scoring  prune-yellow? -> true }t
t{ w abcde s yy--- w xxbbb  rot init-scoring  prune-yellow? -> true }t
t{ w abcde s yy--- w xxabx  rot init-scoring  prune-yellow? -> false }t
t{ w abcde s yy--- w baxxx  rot init-scoring  prune-yellow? -> false }t
t{ w abcde s yy--- w babab  rot init-scoring  prune-yellow? -> false }t

t{ w aacde s yy--- w xxaxx  rot init-scoring  prune-yellow? -> true }t
t{ w aacde s yy--- w xxaax  rot init-scoring  prune-yellow? -> false }t
t{ w eerie s y--y- w vixen  rot init-scoring  prune-yellow? -> false }t

testing prune-grey?
\    guess   score   target
t{ w abcde s ----- w xxxxx  rot init-scoring  prune-grey? -> false }t
t{ w abcde s ----- w xaxxx  rot init-scoring  prune-grey? -> true }t
t{ w abcde s ----- w xxxxa  rot init-scoring  prune-grey? -> true }t
t{ w abcde s ----- w cxxxx  rot init-scoring  prune-grey? -> true }t

testing prune?
\    guess   score   target
t{ w eerie s y--y- w vixen  prune? -> false }t
t{ w eerie s y--y- w xiexx  prune? -> false }t
t{ w eerie s y--y- w exxxx  prune? -> true }t
t{ w eerie s y--y- w xxxix  prune? -> true }t
