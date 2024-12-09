using Microsoft.Maui.Controls;
using System;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private string mysteryWord;
        private string displayWord;
        private int incorrectGuesses;
        private string guessedLetters;

        private readonly string[] imageStages =
           { "hang1.png", "hang2.png", "hang3.png", "hang4.png", "hang5.png", "hang6.png", "hang7.png", "hang8.png" };

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
            displayWord = new string('_', mysteryWord.Length);
        }

        private void UpdateGameUI()
        {
            var wordLabel = this.FindByName<Label>("DisplayWordLabel");
            wordLabel.Text = GetDisplayWord(mysteryWord, guessedLetters);

            var hangmanImage = this.FindByName<Image>("HangmanImage");
            hangmanImage.Source = imageStages[incorrectGuesses];

            var messageLabel = this.FindByName<Label>("MessageLabel");
            messageLabel.Text = $"Guessed letters: {guessedLetters}";
        }

        private void OnGuessClicked(object sender, EventArgs e)
        {
            var inputField = this.FindByName<Entry>("GuessInput");
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
            return !string.IsNullOrEmpty(guess) && guess.Length == 1 && char.IsLetter(guess[0]);
        }

        private void HandleGuess(char guess)
        {
            if (guessedLetters.Contains(guess))
            {
                DisplayMessage("You already guessed that letter.");
                return;
            }

            guessedLetters += guess;
            bool isCorrect = ProcessGuess(guess);

            if (!isCorrect)
            {
                incorrectGuesses++;
            }

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
            char[] displayChars = displayWord.ToCharArray();

            for (int i = 0; i < mysteryWord.Length; i++)
            {
                if (mysteryWord[i] == guess && displayChars[i] == '_')
                {
                    displayChars[i] = guess;
                    isCorrect = true;
                }
            }

            displayWord = new string(displayChars);
            return isCorrect;
        }

        private string GetDisplayWord(string wordToGuess, string guessedLetters)
        {
            char[] displayWord = new char[wordToGuess.Length];

            for (int i = 0; i < wordToGuess.Length; i++)
            {
                if (guessedLetters.Contains(wordToGuess[i]))
                {
                    displayWord[i] = wordToGuess[i];
                }
                else
                {
                    displayWord[i] = '_';
                }
            }

            // Join the characters with a space to make it readable, e.g., "k _ k _ t _ o"
            return string.Join(" ", displayWord);
        }

        private bool CheckWin()
        {
            return displayWord.Replace(" ", "") == mysteryWord;
        }

        private void EndGame()
        {
            var inputField = this.FindByName<Entry>("GuessInput");
            inputField.IsEnabled = false;
        }

        private void DisplayMessage(string message)
        {
            var messageLabel = this.FindByName<Label>("MessageLabel");
            messageLabel.Text = message;
        }
    }
}