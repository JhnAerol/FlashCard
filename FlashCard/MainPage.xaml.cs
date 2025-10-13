namespace FlashCard
{
    public partial class MainPage : ContentPage
    {
        //Initializations
        Random _random = new Random();
        int TotalPokemon = 10;
        bool allRevealed = false;
        bool someRevealed = true;
        string[] values;
        Dictionary<string, string> revealedPokemons = new Dictionary<string, string>();
        Dictionary<string, string> notRevealedPokemon = new Dictionary<string, string>();

        public MainPage()
        {
            InitializeComponent();
            InitializePokedex();
            NotRevealedPokmon();
            values = notRevealedPokemon.Values.ToArray();
        }

        //To Display Pokedex
        private void InitializePokedex()
        {
            NumberCarousel.ItemsSource = Pokedex.pokedex;
            NumberCarousel.Position = 0;
        }

        //Navigate Shuffle Items
        private void OnShuffleClicked(object sender, EventArgs e)
        {
            int randomIndex = _random.Next(0, TotalPokemon);
            if (someRevealed == true)
            {
                NumberCarousel.Position = GetIndexofNotRevealedPokemon();
            }
            else if (allRevealed == true)
            {
                NumberCarousel.Position = randomIndex;
            }
            ShowPokemonAtIndex(NumberCarousel.Position);
        }

        //Navigate Next Item
        private void OnNextClicked(object sender, EventArgs e)
        {
            NumberCarousel.Position = (NumberCarousel.Position == TotalPokemon - 1 ? 0 : NumberCarousel.Position + 1);
            ClearAnswerField();
            HideFeedback();
            ShowPokemonAtIndex(NumberCarousel.Position);
        }

        //Navigate Prev Item
        private void OnPrevClicked(object sender, EventArgs e)
        {
            NumberCarousel.Position = (NumberCarousel.Position == 0 ? TotalPokemon - 1 : NumberCarousel.Position - 1);
            ClearAnswerField();
            HideFeedback();
            ShowPokemonAtIndex(NumberCarousel.Position);
        }

        //Reset 
        private async void OnResetClicked(object sender, EventArgs e)
        {
            //Show custom reset confirmation
            await ShowResetConfirmation();
        }

        //Carousel Posistion Changed
        private void OnCarouselPositionChanged(object sender, PositionChangedEventArgs e)
        {
            HideFeedback();
            ShowPokemonAtIndex(NumberCarousel.Position);
        }

        //Get the indexes of not revealed pokemos 
        private int GetIndexofNotRevealedPokemon()
        {
            var values = notRevealedPokemon.Values.ToList();

            //Collect indexes that match "_black"
            var blackIndexes = Enumerable.Range(0, values.Count)
                                         .Where(i => values[i].Contains("_black"))
                                         .ToList();

            if (blackIndexes.Count == 0)
                return 0;

            //Randomly pick one of those indexes
            int chosenIndex = blackIndexes[_random.Next(blackIndexes.Count)];

            return chosenIndex;
        }

        //Initialize the not revealed pokemon
        public void NotRevealedPokmon()
        {
            foreach (var notReveal in Pokedex.pokedex)
            {
                notRevealedPokemon.Add(notReveal.Key, notReveal.Value.ImageFile);
            }
        }

        //Reset the program
        public void Reset()
        {
            foreach (var pokemon in Pokedex.pokedex.Values)
            {
                if (!pokemon.ImageFile.Contains("_black"))
                {
                    pokemon.ImageFile = pokemon.ImageFile.Replace(".png", "_black.png");
                }
            }

            revealedPokemons.Clear();
            notRevealedPokemon.Clear();
            if (values != null)
            {
                Array.Clear(values);
            }

            allRevealed = false;
            someRevealed = true;
            NotRevealedPokmon();
            values = notRevealedPokemon.Values.ToArray();

            txtAnswer.Text = string.Empty;
            txtAnswer.IsReadOnly = false;
            btnSubmit.IsEnabled = true;
            btnSubmit.BackgroundColor = Color.FromArgb("#4CAF50");

            NumberCarousel.Position = 0;
            HideFeedback();
            HideResetConfirmation();
            UpdateProgressBar();
        }

        //Check the carousel index then if the image is revealed change the UI State
        public void ShowPokemonAtIndex(int index)
        {
            int currentIndex = index;
            var currentPokemon = Pokedex.pokedex.ElementAt(currentIndex);

            if (currentIndex < 0 || currentIndex >= Pokedex.pokedex.Count)
                return;

            //If revealed, show name & disable button
            if (revealedPokemons.ContainsKey(currentPokemon.Key))
            {
                string current = currentPokemon.Value.ImageFile.Replace(".png", "");
                string output = char.ToUpper(current[0]) + current.Substring(1);
                txtAnswer.Text = output;
                txtAnswer.IsReadOnly = true;
                btnSubmit.IsEnabled = false;
                btnSubmit.BackgroundColor = Colors.Gray;
            }
            else
            {
                txtAnswer.Text = string.Empty;
                txtAnswer.IsReadOnly = false;
                btnSubmit.IsEnabled = true;
                btnSubmit.BackgroundColor = Color.FromArgb("#4CAF50");
            }
        }

        //If the image is revealed, update the UI state
        private void UpdateUIState()
        {
            if (NumberCarousel.CurrentItem is KeyValuePair<string, ImageClass> currentPokemon)
            {
                string imageFile = currentPokemon.Value.ImageFile;
                bool isRevealed = !imageFile.Contains("_black");
                string output = char.ToUpper(imageFile[0]) + imageFile.Substring(1);

                if (isRevealed)
                {
                    txtAnswer.Text = output.Replace(".png", "");
                    txtAnswer.IsReadOnly = true;
                    btnSubmit.IsEnabled = false;
                    btnSubmit.BackgroundColor = Colors.Gray;
                }
                else
                {
                    txtAnswer.Text = string.Empty;
                    txtAnswer.IsReadOnly = false;
                    btnSubmit.IsEnabled = true;
                    btnSubmit.BackgroundColor = Color.FromArgb("#4CAF50");
                }
            }
        }

        //Submit Answer
        private async void OnEnterClicked(object sender, EventArgs e)
        {
            //Prevent Null Answer
            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                await ShowEmptyAnswerFeedback();
                return;
            }

            string userInput = txtAnswer.Text.Trim();

            //Check first if the CurrentItems is a KeyValuePair
            if (NumberCarousel.CurrentItem is KeyValuePair<string, ImageClass> currentPokemon)
            {
                bool isCorrect = string.Equals(currentPokemon.Key, userInput, StringComparison.OrdinalIgnoreCase);

                //Then check if the Answer is Correct or Incorrect
                if (isCorrect)
                {
                    //To prevent shuffling the revealed pokemon
                    if (values != null)
                    {
                        Array.Clear(values);
                    }
                    revealedPokemons.Add(currentPokemon.Key, currentPokemon.Value.ImageFile);
                    if (notRevealedPokemon.ContainsKey(currentPokemon.Key))
                    {
                        string currentImage = notRevealedPokemon[currentPokemon.Key];
                        notRevealedPokemon[currentPokemon.Key] = currentImage.Replace("_black.png", ".png");
                    }
                    values = notRevealedPokemon.Values.ToArray();
                   
                    await ShowCorrectAnswer(currentPokemon.Key);
                }
                else
                {
                    await ShowIncorrectAnswer(currentPokemon.Key);
                }
            }
        }

        //Method to call if the answer is Correct
        private async Task ShowCorrectAnswer(string pokemonName)
        {
            //Update the image to colored version
            if (Pokedex.pokedex.ContainsKey(pokemonName))
            {
                Pokedex.pokedex[pokemonName].ImageFile = $"{pokemonName}.png";
            }

            //FadeIn Effect
            if (NumberCarousel.CurrentItem is KeyValuePair<string, ImageClass>)
            {
                var currentView = NumberCarousel.VisibleViews.FirstOrDefault();
                var image = currentView?.FindByName<Image>("pokemonImg");
                if (image != null)
                {
                    await image.FadeTo(0, 100);
                    await image.FadeTo(1, 300);
                }
            }

            //Show success feedback
            await ShowSuccessFeedback(pokemonName);
            UpdateProgressBar();

            //Check if all the images are revealed
            allRevealed = Pokedex.pokedex.Values.All(p => p.ImageFile != null && !p.ImageFile.Contains("_black"));

            await Task.Delay(1500);

            if (allRevealed)
            {
                await ShowCompletionCelebration();
            }
            else
            {
                //Go to next pokemon
                if (NumberCarousel.Position <= TotalPokemon - 1)
                {
                    if (NumberCarousel.Position == TotalPokemon - 1)
                    {
                        NumberCarousel.Position = 0;
                    }
                    else
                    {
                        NumberCarousel.Position++;
                    }
                    ShowPokemonAtIndex(NumberCarousel.Position);
                }
            }

            ClearAnswerField();
            UpdateUIState();
            HideFeedback();
        }

        //Method to call if the answer is Incorrect
        private async Task ShowIncorrectAnswer(string correctAnswer)
        {
            await ShowErrorFeedback();
            await Task.Delay(2000);
            HideFeedback();
            ClearAnswerField();
            txtAnswer.Focus();
        }

        //Clear input fields
        private void ClearAnswerField()
        {
            txtAnswer.Text = string.Empty;
        }

        //Show empty answer feedback
        private async Task ShowEmptyAnswerFeedback()
        {
            feedbackBorder.BackgroundColor = Color.FromArgb("#FFA726");
            feedbackMessage.Text = "Please enter a Pokémon name!";
            feedbackBorder.IsVisible = true;

            await feedbackBorder.ScaleTo(1.1, 100);
            await feedbackBorder.ScaleTo(1, 100);

            await Task.Delay(2000);
            HideFeedback();
        }

        //Show success feedback
        private async Task ShowSuccessFeedback(string pokemonName)
        {
            feedbackBorder.BackgroundColor = Color.FromArgb("#4CAF50");
            feedbackMessage.Text = $"Correct! It's {char.ToUpper(pokemonName[0]) + pokemonName.Substring(1)}!";
            feedbackBorder.IsVisible = true;

            await feedbackBorder.ScaleTo(1.15, 150);
            await feedbackBorder.ScaleTo(1, 150);
        }

        //Show error feedback
        private async Task ShowErrorFeedback()
        {
            feedbackBorder.BackgroundColor = Color.FromArgb("#F44336");
            feedbackMessage.Text = "Wrong answer! Try again!";
            feedbackBorder.IsVisible = true;

            await feedbackBorder.TranslateTo(-10, 0, 50);
            await feedbackBorder.TranslateTo(10, 0, 50);
            await feedbackBorder.TranslateTo(-10, 0, 50);
            await feedbackBorder.TranslateTo(0, 0, 50);
        }

        //Hide feedback
        private void HideFeedback()
        {
            feedbackBorder.IsVisible = false;
        }

        //Show completion celebration
        private async Task ShowCompletionCelebration()
        {
            completionOverlay.IsVisible = true;
            completionContent.Opacity = 0;
            completionContent.Scale = 0.5;

            await Task.WhenAll(
                completionContent.FadeTo(1, 300),
                completionContent.ScaleTo(1, 300, Easing.BounceOut)
            );
        }

        //Hide completion celebration
        private void HideCompletionCelebration()
        {
            completionOverlay.IsVisible = false;
        }

        //Show reset confirmation
        private async Task ShowResetConfirmation()
        {
            resetOverlay.IsVisible = true;
            resetContent.Opacity = 0;
            resetContent.Scale = 0.8;

            await Task.WhenAll(
                resetContent.FadeTo(1, 200),
                resetContent.ScaleTo(1, 200)
            );
        }

        //Hide reset confirmation
        private void HideResetConfirmation()
        {
            resetOverlay.IsVisible = false;
        }

        //Confirm reset
        private async void OnConfirmResetClicked(object sender, EventArgs e)
        {
            await resetContent.ScaleTo(0.8, 100);
            Reset();
        }

        //Cancel reset
        private async void OnCancelResetClicked(object sender, EventArgs e)
        {
            await resetContent.ScaleTo(0.8, 100);
            HideResetConfirmation();
        }

        //Continue after completion
        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await completionContent.ScaleTo(0.8, 100);
            HideCompletionCelebration();
            allRevealed = true;
            someRevealed = false;
        }

        //Reset after completion
        private async void OnResetAfterCompletionClicked(object sender, EventArgs e)
        {
            await completionContent.ScaleTo(0.8, 100);
            HideCompletionCelebration();
            Reset();
        }

        //Update progress bar
        private void UpdateProgressBar()
        {
            int revealed = revealedPokemons.Count;
            double progress = (double)revealed /TotalPokemon;
            progressBar.Progress = progress;
            progressLabel.Text = $"{revealed}/{TotalPokemon} Pokémon Revealed";
        }
    }
}
