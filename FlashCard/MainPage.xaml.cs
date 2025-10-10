namespace FlashCard
{
    public partial class MainPage : ContentPage
    {
        //Initializations
        Random _random = new Random();
        int TotalPokemon = 10;

        public MainPage()
        {
            InitializeComponent();
            InitializePokedex();

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
            NumberCarousel.Position = randomIndex;
            ClearAnswerField();
        }

        //Navigate Next Item
        private void OnNextClicked(object sender, EventArgs e)
        {
            NumberCarousel.Position = (NumberCarousel.Position == TotalPokemon - 1 ? 0 : NumberCarousel.Position++);
            ClearAnswerField();
        }

        //Navigate Prev Item
        private void OnPrevClicked(object sender, EventArgs e)
        {
            NumberCarousel.Position = (NumberCarousel.Position == 0 ? TotalPokemon - 1 : NumberCarousel.Position--);
            ClearAnswerField();
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
            Thread.Sleep(1200);

            //Check if all the images are revealed
            bool allRevealed = Pokedex.pokedex.Values.All(p => p.ImageFile != null && !p.ImageFile.Contains("_black"));
            if (allRevealed)
            {
                //Display alert when all the images are revealed
                await DisplayAlert("Congratulations!", "All Pokémon have been revealed!", "Okay");
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
            }

            ClearAnswerField();
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
