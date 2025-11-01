// Wordle game

use crate::words::{Word, ww, find};
use crate::score::{Score, score, GREEN};

pub const GUESSES: usize = 6;     // max guesses

#[derive(Default)]
pub struct Game {
    secret: Word,           // the secret word
    guesses: usize,         // number of guesses, 0-6
    history: [(Word, Score); GUESSES],   // history of guesses
    used: [u8; 32],         // letter usage, 0=?, 1=grey, 2=yellow, 3=green
    greens: [char; 5],      // green letter or '-' if not known
}

impl Game {
    pub fn new() -> Game {
        let secret: Word = 1234; // todo: random
        Self::with_secret(secret)
    }

    pub fn with_secret(secret: Word) -> Game {
        Game {
            secret: secret,
            guesses: 0,
            history: [(0,0); GUESSES],
            used: [0; 32],
            greens: ['-'; 5],
        }
    }

    pub fn secret(&mut self, w: Word) {
        self.secret = w;
    }

    pub fn reset(&mut self) {
        self.guesses = 0;
        self.used.fill(0);
        self.greens.fill('-');
    }

    pub fn guesses(&self) -> usize {
        self.guesses
    }

    pub fn latest(&self) -> Option<(Word, Score)> {
        if self.guesses > 0 {
            Some(self.history[self.guesses-1])
        } else {
            None
        }
    }

    pub fn greens(&self) -> i32 {
        let mut greens = 0;
        for c in self.greens {
            if c != '-' {
                greens += 1;
            }
        }
        greens
    }

    pub fn answer(&self) -> Word {
        let s: String = self.greens.iter().collect();
        find(s.as_str()).unwrap()
    }

    pub fn submit(&mut self, guess: Word) -> bool {
        if self.guesses >= GUESSES {
            panic!("no more guesses");
        }

        let score = score(guess, self.secret);
        let solved = score == 242;

        self.history[self.guesses] = (guess, score);
        self.guesses += 1;

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

        solved
    }
}
