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
