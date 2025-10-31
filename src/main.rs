mod words;
mod score;
mod game;
mod workset;
mod guess;
mod solver;
mod prune;

use crate::{game::{Game, GUESSES}, guess::guess, prune::prune, words::{ww, HIDDEN}, workset::Workset};

fn main() {
    let mut game = Game::new();
    let mut work = Workset::new();
    let mut solved = 0;
    let n = HIDDEN;//HIDDEN
    for w in 0..n {
        game.secret(w);
        if solve(&mut game, &mut work) {
            solved += 1;
        }        
    }
    println!("Solved {}, failed {}", solved, n-solved);
}

fn solve(game: &mut Game, work: &mut Workset) -> bool {
    game.reset();
    work.fill();
    loop {
        if game.submit(guess(game, &work)) {
            // println!("Solved in {}", game.guesses());
            return true;
        }
        if game.guesses() == GUESSES {
            // println!("Failed");
            return false;
        }
        work.prune(|w| prune(w, game.latest().unwrap()));
        let latest = game.latest().unwrap();
        // println!("Guess {} {} {} remaining {}", game.guesses(), ww(latest.0), score::to_string(latest.1), work.remaining());
    }
}
