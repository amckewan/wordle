( guess a word )

(
    We keep track of these:
    'answer' has the letters we alredy know about
    'guess' has our current guess 
    'result' has what wordle gives us to score our guess
)


\ current answer, char if green else 0
create answer len allot
: clear-answer  answer len erase ; clear-answer

: green? ( pos -- f ) answer + c@ ;

: .answer  len 0 do  i green?  ?dup 0= if [char] ? then emit  loop space ;

variable guess  ( current guess )

\ to start, we'll just guess the first word we have left
: guess  ( -- w )  first 0= abort" no words left to guess!" ;


