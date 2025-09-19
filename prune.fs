( Pruning the working set )

\ The working set contains the words that could be the solution.
\ We start with all the words then prune the set after each score
\ by removing words that couldn't have got that score.

create working  #words allot  \ one byte per word, 0=absent, 1=present

: all-words  working #words 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

: #working ( -- n )  0 #words 0 do i has + loop ;
: .working  0  #words 0 do i has if i ww w. 1+ then loop  . ." words " ;

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

TESTING #WORKING
T{ all-words #working -> #words }T
T{ working 10 erase  #working -> #words 10 - }T
T{ 0 working #words + 1- c!  #working -> #words 11 - }T
T{ working #words erase  #working -> 0 }T


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
