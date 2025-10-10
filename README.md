# This is a fully functional minesweeper game built with C# and WinForms, featuring multiple difficulties and scaling board resizing, restart support, and the usual first-click safety.

## Features:
    Three difficulties:
        Easy -- 8x8 grid, 10 bombs
        Medium -- 15x15 grid, 30 bombs
        Hard -- 20x20 grid, 60 bombs
    First click safety:
        Your first click (and the surrounding 8 cells) are guaranteed to be bomb-free.
    Restart Anytime:
        Click the restart button to instantly restart and begin a new round.
    Automatic Win/Loss detection:
        The game will end automatically when all safe tiles are revealed, or if you click on a bomb.

## Installation & Setup:
    Simply install the .msi file and begin clicking away!


## How to play:
    Select a difficulty from the dropdown menu.
    Click Restart to generate a new board.
    Left-click to reveal a tile.
    If you reveal a bomb 💣 → you lose!
    Empty tiles auto-expand surrounding safe areas.
    Right-click to place or remove a flag ⚑.
    Clear all safe tiles to win the game 🎉.

## Project structure:
    /WinFormsApp1
    │
    ├── Form1.cs         # Main game logic and UI
    ├── Program.cs       # Application entry point
    └── README.md        # You are here
