mod words;
mod score;
mod game;
mod workset;
mod guess;
mod solver;
mod prune;

use crate::{game::{Game, GUESSES}, guess::guess, words::HIDDEN, workset::Workset};

fn main() {
    let mut game = Game::new();
    let mut work = Workset::new();
    let mut solved = 0;
    for w in 0..HIDDEN {
        game.secret(w);
        if solve(&mut game, &mut work) {
            solved += 1;
        }        
    }
    println!("Solved {}, failed {}", solved, HIDDEN-solved);
}

fn solve(game: &mut Game, work: &mut Workset) -> bool {
    game.reset();
    work.fill();
    loop {
        if game.submit(guess(game, &work)) {
            return true;
        }
        if game.guesses() == GUESSES {
            return false;
        }
        // prune(work, |w| {} );
    }
}
