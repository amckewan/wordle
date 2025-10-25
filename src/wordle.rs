// Wordle game

use crate::words::{Word, ww};
use crate::score::{Score, score, GREEN};

#[derive(Default)]
struct Wordle {
    secret: Word,           // the secret word
    guesses: Vec<Word>,     // history of guesses
    scores: Vec<Score>,     // scores for each guess
    used: [u8; 32],         // letter usage, 0=unknown, 1=grey, 2=yellow, 3=green
    greens: [char; 5],      // green letter or '-' if not known
}

impl Wordle {
    pub fn new() -> Wordle {
        let secret: Word = 0; // todo: random
        Self::with_secret(secret)
    }

    pub fn with_secret(secret: Word) -> Wordle {
        Wordle {
            secret: secret,
            guesses: Vec::new(),
            scores: Vec::new(),
            used: [0; 32],
            greens: ['-'; 5],
        }
    }

    pub fn reset(&mut self) {
        self.guesses.clear();
        self.scores.clear();
        self.used.fill(0);
        self.greens.fill('-');
    }

    pub fn num_guesses(&self) -> usize {
        self.guesses.len()
    }

    pub fn latest(&self) -> (Word, Score) {
        
        (0,0)
    }

    pub fn num_greens(&self) -> i32 {
        let mut num = 0;
        for c in self.greens {
            if c != '-' {
                num += 1;
            }
        }
        num
    }

    pub fn submit(&mut self, guess: Word) -> Score {
        let score = score(ww(guess), ww(self.secret));

        // update history
        self.guesses.push(guess);
        self.scores.push(score);

        // update used and greens
        let g: Vec<char> = ww(guess).chars().collect();
        let mut mscore = score;
        for i in 0..5 {
            let c = g[i];
            let s = mscore % 3;
            mscore /= 3;

            let l = c as usize & 31;
            if s >= self.used[l] {
                self.used[l] = s + 1;
            }

            if s == GREEN {
                self.greens[i] = c;
            }
        }

        score
    }

}
