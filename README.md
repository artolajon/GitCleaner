# GitCleaner

GitCleaner is a Windows console tool to help you manage branches in your local Git repository. It provides a simple menu to check branch status, sync branches, and delete merged branches.

## Features

- **Check actual status of all branches:**  
  See how many commits each branch is ahead or behind the primary branch.
- **Sync all branches:**  
  Automatically checkout and pull updates for all branches.
- **Delete merged branches:**  
  Delete branches that have already been merged into the primary branch, with an option to also delete them from the remote.

## Requirements

- Run in a folder containing a `.git` directory (i.e., inside a local Git repository)
- Git installed and available in your system PATH

## Usage

1. Download the latest release from the [Releases page](https://github.com/artolajon/GitCleaner/releases).
2. Extract and run `GitCleaner.exe` in your repository folder. The `.exe` does **not** require .NET to be installed.
3. Enter the name of your primary branch (default is `main`).
4. Use the menu to select an action:
   - `1` - Check actual status of all branches
   - `2` - Sync all branches
   - `3` - Delete all branches that are already merged
   - `4` - Quit

## How it works

- The tool interacts with Git using command-line calls.
- It analyzes branches by counting commits ahead/behind the primary branch.
- Branch deletion is confirmed before proceeding, with an option to delete on remote.

## License

See `LICENSE.txt` for details.