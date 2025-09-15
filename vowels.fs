\ Count vowels

1 'A' c>l lshift
1 'E' c>l lshift or
1 'I' c>l lshift or
1 'O' c>l lshift or
1 'U' c>l lshift or constant vowels_

: vowel ( l -- 0/1 )  vowels_ swap rshift 1 and ;

: vowels ( w -- n )  0 ( n ) len 0 do  over i get vowel +  loop nip ;


TESTING VOWELS
T{ 0 vowel -> 0 }T
T{ 'A' c>l vowel -> 1 }T
T{ 'M' c>l vowel -> 0 }T
T{ 'U' c>l vowel -> 1 }T
T{ 'Z' c>l vowel -> 0 }T

T{ w AEIOU vowels -> 5 }T
T{ w EEEEE vowels -> 5 }T
T{ w BMNFG vowels -> 0 }T
T{ w ABCDE vowels -> 2 }T
