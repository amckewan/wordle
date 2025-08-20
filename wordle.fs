( Wordle solver )

include words.fs
include set.fs

\ current answer, char if green else 0
create answer len allot
: clear-answer  answer len erase ; clear-answer

: green? ( pos -- f ) answer + c@ ;

: .answer  len 0 do  i green?  ?dup 0= if [char] ? then emit  loop space ;

include prune.fs
