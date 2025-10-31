// Wordle game

use crate::words::{Word, ww, find};
use crate::score::{Score, score, GREEN, YELLOW};

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

        let score = score(ww(guess), ww(self.secret));
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

// : esc  27 emit ." [" ;
// : color ( color -- )  dup yellow = if 2 + then 100 +  esc 0 .r ." ;30m" ;
// : normal  esc ." 0m" ;
// : .colored ( guess score -- )
//     swap ww len bounds do
//         3 /mod swap color
//         i c@ bl xor ( upc ) emit
//     loop drop normal space ;
// : .history  guesses 0 ?do  cr i 1+ .  i history 2@ .colored  loop ;

    // println!("\x1B[100;30m Grey   \x1B[0m");
    // println!("\x1B[103;30m Yellow \x1B[0m");
    // println!("\x1B[102;30m Green  \x1B[0m");
    fn color(color: u8) -> &'static str {
        match color {
            GREEN  => "\x1B[102;30m",
            YELLOW => "\x1B[103;30m",
            _      => "\x1B[100;30m",
        }
    }

    fn print_char(c: char, color: u8) {
        let fg = 30;
        let bg = 102;
        // print!("\x1B[{};{}m{}")
    }

    fn format_colored(word: Word, score: Score) -> String {

        format!("{}", ww(word))
    }

    pub fn print_history(&self) {
        for i in 0..self.guesses {
            let (word, score) = self.history[i];
            println!("{} {}", i+1, Self::format_colored(word, score));
        }
    }
}
