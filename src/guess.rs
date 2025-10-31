// Guess using the ideas from "Solving Wordle using information theory"
// https://www.youtube.com/watch?v=v68zYyaEmEA
//
// P = probability that we get a particular score
// I = information bits, I = log2(1/P) or -log2(P)
// e.g. P = 1/16, I = 4
//
// Entropy = sum (P*I) over all possible scores

use crate::score::{score, SCORES};
use crate::words::{from_str, Word};
use crate::game::Game;
use crate::workset::Workset;

fn entropy(word: Word, work: &Workset) -> f64 {
    let mut scored: [u16; SCORES] = [0; SCORES];
    let mut words = 0.;

    let mut target = work.first();
    loop {
        scored[score(word, target) as usize] += 1;
        words += 1.;

        match work.next(target) {
            Some(next) => target = next,
            None => break,
        }
    }

    let mut entropy = 0.;
    for i in 0..SCORES {
        let n = scored[i] as f64;
        if n > 0. {
            let prob = n / words;
            let ibits = -prob.log2();
            entropy += prob * ibits;
        }
    }
    entropy
}

fn max_entropy(work: &Workset) -> Word {
    let mut result = 0;
    let mut max = 0.;

    let mut w = work.first();
    loop {
        let e = entropy(w, &work);
        if e > max {
            result = w;
            max = e;
        }
        match work.next(w) {
            Some(next) => w = next,
            None => break,
        }
    }

    result
}

fn max_entropy_all(workset: &Workset) -> Word {
    workset.first()
}

pub fn guess(wordle: &Game, workset: &Workset) -> Word {
    if wordle.guesses() == 0 {
        return from_str("salet");
    }
    if workset.remaining() == 1 {
        return workset.first();
    }
    if wordle.greens() == 5 {
        return wordle.answer();
    }
    if workset.remaining() < 1 {
        max_entropy_all(workset)
    } else {
        max_entropy(workset)
    }
}
