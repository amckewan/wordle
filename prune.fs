( Pruning the working set )

\ build a mask that will exclude non-green letters found in score
: gmask ( score -- mask )     0 ( mask )
    len 0 do ( score mask )
        swap 3 /mod swap ( m s' color )
        green = cmask and i left ( letter mask )
        rot or ( s' m' )
    loop nip ;

TESTING GMASK
T{ 0 gmask -> 0 }T
T{ s ----- gmask -> 0 }T
T{ s g---- gmask -> cmask }T
T{ s --g-- gmask -> cmask 2 bits * lshift }T
T{ s ggggg gmask -> $1FFFFFF ( 25 bits ) }T


: -green ( guess score w -- gmask f ) \ non-zero if w should be pruned
    rot xor ( 0=green ) swap gmask  swap over and ;

TESTING -GREEN
T{ w AAAAA s G---- w Axxxx -green 0<>     -> cmask false }T
T{ w AAAAA s G---- w Bxxxx -green 0<> nip ->       true  }T

0 [if]
variable pruned     \ letter mask, set if this letter has been used

\ Using 'score' and 'used' from score.fs as local variables
\ it's more convenient using these as arrays
: prune? ( guess score w -- f )
    

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
