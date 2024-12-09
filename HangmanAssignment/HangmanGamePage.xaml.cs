using Microsoft.Maui.Controls;
using System;
using System.Linq;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private string mysteryWord;
        private string displayWord;
        private int incorrectGuesses;
        private string guessedLetters;

        private readonly string[] imageStages =
        {
            "hang1.png", "hang2.png", "hang3.png", "hang4.png",
            "hang5.png", "hang6.png", "hang7.png", "hang8.png"
        };

        private readonly string[] wordBank = {
            "ELEPHANT", "TIGER", "CROCODILE", "GIRAFFE", "HIPPOPOTAMUS",
            "KANGAROO", "RHINOCEROS", "PELICAN", "FLAMINGO", "CHIMPANZEE"
        };

        public HangmanGamePage()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            SelectRandomWord();
            ResetGameVariables();
            UpdateGameUI();
        }

        private void SelectRandomWord()
        {
            Random random = new Random();
            mysteryWord = wordBank[random.Next(wordBank.Length)];
        }

        private void ResetGameVariables()
        {
            incorrectGuesses = 0;
            guessedLetters = string.Empty;
            displayWord = GetInitialDisplayWord(mysteryWord);
        }

        private string GetInitialDisplayWord(string wordToGuess)
        {
            // Create a character array to manipulate
            char[] displayChars = new char[wordToGuess.Length];

            // Reveal first and last letters
            displayChars[0] = wordToGuess[0];
            displayChars[wordToGuess.Length - 1] = wordToGuess[wordToGuess.Length - 1];

            // Fill the rest with underscores
            for (int i = 1; i < wordToGuess.Length - 1; i++)
            {
                displayChars[i] = '_';
            }

            // Join with spaces to make it more readable
            return string.Join(" ", displayChars);
        }

        private void UpdateGameUI()
        {
            try
            {
                // Find labels safely
                var wordLabel = this.FindByName<Label>("DisplayWordLabel");
                var messageLabel = this.FindByName<Label>("MessageLabel");
                var hangmanImage = this.FindByName<Image>("HangmanImage");

                if (wordLabel != null)
                {
                    // Display the current state of the word
                    wordLabel.Text = displayWord;
                }

                if (hangmanImage != null)
                {
                    // Ensure we don't go out of bounds
                    hangmanImage.Source = imageStages[Math.Min(incorrectGuesses, imageStages.Length - 1)];
                }

                if (messageLabel != null)
                {
                    messageLabel.Text = $"Guessed letters: {guessedLetters}";
                }
            }
            catch (Exception ex)
            {
                // Log any UI update errors
                System.Diagnostics.Debug.WriteLine($"UI Update Error: {ex.Message}");
            }
        }

        private void OnGuessClicked(object sender, EventArgs e)
        {
            var inputField = this.FindByName<Entry>("GuessInput");
            if (inputField == null) return;

            string guess = inputField.Text?.ToUpper();
            if (IsValidGuess(guess))
            {
                HandleGuess(guess[0]);
                inputField.Text = string.Empty;
            }
            else
            {
                DisplayMessage("Please enter a valid letter.");
            }
        }

        private bool IsValidGuess(string guess)
        {
            return !string.IsNullOrEmpty(guess) &&
                   guess.Length == 1 &&
                   char.IsLetter(guess[0]);
        }

        private void HandleGuess(char guess)
        {
            // Prevent duplicate guesses
            if (guessedLetters.Contains(guess))
            {
                DisplayMessage("You already guessed that letter.");
                return;
            }

            // Add the guess to guessed letters
            guessedLetters += guess;

            // Process the guess
            bool isCorrect = ProcessGuess(guess);

            if (!isCorrect)
            {
                incorrectGuesses++;
            }

            // Check win/lose conditions
            if (CheckWin())
            {
                DisplayMessage($"Congratulations! You survived! The word was: {mysteryWord}");
                EndGame();
            }
            else if (incorrectGuesses >= imageStages.Length)
            {
                DisplayMessage($"Game Over. You died. The word was: {mysteryWord}");
                EndGame();
            }
            else
            {
                UpdateGameUI();
            }
        }

        private bool ProcessGuess(char guess)
        {
            bool isCorrect = false;
            char[] displayChars = displayWord.Replace(" ", "").ToCharArray();

            for (int i = 0; i < mysteryWord.Length; i++)
            {
                if (mysteryWord[i] == guess && displayChars[i] == '_')
                {
                    displayChars[i] = guess;
                    isCorrect = true;
                }
            }

            // Recreate the display word with spaces
            displayWord = string.Join(" ", displayChars);
            return isCorrect;
        }

        private bool CheckWin()
        {
            // Remove spaces and compare directly
            return displayWord.Replace(" ", "") == mysteryWord;
        }

        private void EndGame()
        {
            var inputField = this.FindByName<Entry>("GuessInput");
            if (inputField != null)
            {
                inputField.IsEnabled = false;
            }
        }

        private void DisplayMessage(string message)
        {
            var messageLabel = this.FindByName<Label>("MessageLabel");
            if (messageLabel != null)
            {
                messageLabel.Text = message;
            }
        }
    }
}