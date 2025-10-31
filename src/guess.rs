// Guess using the ideas from "Solving Wordle using information theory"
// https://www.youtube.com/watch?v=v68zYyaEmEA
//
// P = probability that we get a particular score
// I = information bits, I = log2(1/P) or -log2(P)
// e.g. P = 1/16, I = 4
//
// Entropy = sum (P*I) over all possible scores

use crate::words::{Word, find};
use crate::game::Game;
use crate::workset::Workset;

fn max_entropy(workset: &Workset) -> Word {
    workset.first()
}

fn max_entropy_all(workset: &Workset) -> Word {
    workset.first()
}

pub fn guess(wordle: &Game, workset: &Workset) -> Word {
    if wordle.guesses() == 0 {
        return find("salet").unwrap();
    }
    if workset.remaining() == 1 {
        return workset.first();
    }
    if wordle.greens() == 5 {
        return wordle.answer();
    }
    if workset.remaining() < 100 {
        max_entropy_all(workset)
    } else {
        max_entropy(workset)
    }
}


// : flog2 ( F: f -- log2_f )  flog [ 2e flog ] fliteral f/ ;

// : probability ( n total -- ) ( F: -- prob )  swap s>f s>f f/ ;
// : ibits ( F: prob -- ibits )  flog2 fnegate ;

// for each score, how many words from the working set got that score
// create scored   #scores cells allot

// : .scored  0 0  #scores 0 do i cells scored + @ ?dup if  
//       over 6 mod 0= if cr then swap 1+ swap
//       i s.  dup 3 .r 3 spaces  +
//     then loop cr ." Total: " . drop ;

// update scored for each word in the working set
// : score-working  ( w -- #words )
//     scored #scores cells erase   0 ( #words )
//     working @ begin >r
//         over r@ swap score  cells scored +  1 swap +!  1+
//     r> next? until nip ;

// calculate the entropy for a guess against all the words in the working set
// : entropy ( w -- ) ( F: -- entropy )
//     score-working ( #words ) 0e ( entropy ) 
//     #scores 0 do
//         i cells scored + @ ( words with this score )
//         ?dup if  over probability  fdup ibits f*  f+  then
//     loop drop ;

// find the word from the working set with the highest entropy
// : max-entropy ( -- w )    
//     0e ( entropy ) working @ ( default ) dup begin
//         dup entropy  fover fover f< if  nip dup  fswap  then  fdrop
//     next? until fdrop ;

// find the word with the highest entropy from all words
// : max-entropy-all ( -- w )
//     0e ( entropy ) 0 ( default ) #words 0 do
//         i entropy  fover fover f< if  drop i  fswap  then  fdrop
//     loop fdrop ;

// It takes time for the first guess which is always the same
//   all-words              time max-entropy w.  85.293 sec   tares
//   all-words prune-hidden time max-entropy w.   2.601 sec   raise
// When we consider all words, "tares" has higher entropy
//   w raise entropy f. 5.91973684251619
//   w tares entropy f. 6.19405254437545
// The proven best first word is "salet", which we don't prize quite as much:
//   w salet entropy f. 6.01684287539826

// w tares value first-guess

//  cr .( calculating first entropy guess... )
//  all-words max-entropy to first-guess

// variable allon2 ( consider all words for 2nd guess - slow )
// : second-guess  allon2 @ if max-entropy-all else max-entropy then ;

// guess the word with the highest entropy
// variable fence ( working set size below which we guess with all words )
// : entropy-guess ( -- w )
//     remaining 2 < if ( can't use entropy ) simple-guess exit then
//     guesses 0=  if first-guess  exit then
//     guesses 1 = if second-guess exit then
//     \ entropy is ineffective when the working set gets too small
//     \ but it doesn't make sense for the last guess
//     remaining fence @ u<  guesses 5 < and if max-entropy-all exit then
//     max-entropy ;

