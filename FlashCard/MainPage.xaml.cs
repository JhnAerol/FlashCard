using System.Collections.Frozen;

namespace FlashCard
{
    public partial class MainPage : ContentPage
    {
        //Initializations
        Random _random = new Random();
        int TotalPokemon = 10;
        bool allRevealed = false;
        bool someRevealed = false;
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
            else if(allRevealed == true)
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
            ShowPokemonAtIndex(NumberCarousel.Position);
        }

        //Navigate Prev Item
        private void OnPrevClicked(object sender, EventArgs e)
        {
            NumberCarousel.Position = (NumberCarousel.Position == 0 ? TotalPokemon - 1 : NumberCarousel.Position - 1);
            ClearAnswerField();
            ShowPokemonAtIndex(NumberCarousel.Position);
        }
        
        //Reset 
        private async void OnResetClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Reset?", "Do you want to reset? ", "Yes", "No");

            if (answer)
            {
                Reset();
            }
            else
            {
                allRevealed = true;
                someRevealed = false;
            }
        }

        //Carousel Posistion Changed
        private void OnCarouselPositionChanged(object sender, PositionChangedEventArgs e)
        {
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
            foreach(var notReveal in Pokedex.pokedex)
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

        //If the image is revealed, show name & disable button
        private void UpdateUIState()
        {
            if (NumberCarousel.CurrentItem is KeyValuePair<string, ImageClass> currentPokemon)
            {
                string imageFile = currentPokemon.Value.ImageFile;
                bool isRevealed = !imageFile.Contains("_black");

                if (isRevealed)
                {
                    txtAnswer.Text = currentPokemon.Key;
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

        //Sumbit Answer
        private async void OnEnterClicked(object sender, EventArgs e)
        {
            //Prevent Null Answer
            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                await DisplayAlert("Empty Answer", "Please enter a Pokémon name!", "OK");
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
                    //Prevent Duplication
                    if (revealedPokemons.ContainsKey(currentPokemon.Key) && !notRevealedPokemon.ContainsKey(currentPokemon.Key))
                    {
                        await DisplayAlert("Duplication", "Duplication Error.", "Okay");
                    }
                    //To prevent shuffling the revealed pokemon
                    else
                    {
                        if(values != null)
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
                    }
                    someRevealed = true;
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

            //Count 1.2s
            await Task.Delay(1200); 

            //Check if all the images are revealed
            allRevealed = Pokedex.pokedex.Values.All(p => p.ImageFile != null && !p.ImageFile.Contains("_black"));

            if (allRevealed)
            {
                //Display alert when all the images are revealed 
                await DisplayAlert("Congratulations!", "All Pokémon have been revealed!", "Okay");

                //Reset
                bool answer = await DisplayAlert("Reset?", "Do you want to reset? ", "Yes", "No");
                if (answer)
                {
                    Reset();
                }
                else
                {
                    allRevealed= true;
                    someRevealed = false;
                }
            }
            else if (NumberCarousel.Position == TotalPokemon - 1)
            {
                //Display alert when answer is corrent and it is the last number
                await DisplayAlert("Correct!", $"That's right! It's {pokemonName}! ", "Okay");
            }
            else
            {
                //Display alert when answer is corrent
                await DisplayAlert("Correct!", $"That's right! It's {pokemonName}! ", "Next Pokémon");
            }

            //Go to perspective pokemon
            if (NumberCarousel.Position <= TotalPokemon - 1)
            {
                if(NumberCarousel.Position == TotalPokemon - 1 || allRevealed)
                {
                    NumberCarousel.Position = 0;
                }
                else
                {
                    NumberCarousel.Position++;
                }
                ShowPokemonAtIndex(NumberCarousel.Position);
            }

            ClearAnswerField();
            UpdateUIState();
        }

        //Method to call if the answer is Incorrect
        private async Task ShowIncorrectAnswer(string correctAnswer)
        {
            //Display alert when answer is Incorrect
            await DisplayAlert("Incorrect", $"Sorry! Wrong amswer :(. Try again!", "Try Again");

            ClearAnswerField();
            txtAnswer.Focus();
        }

        //Clear input fields
        private void ClearAnswerField()
        {
            txtAnswer.Text = string.Empty;
        }
    }
}
