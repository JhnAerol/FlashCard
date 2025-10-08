namespace FlashCard
{
    public partial class MainPage : ContentPage
    {
        Random random = new();

        public MainPage()
        {
            InitializeComponent();

            NumberCarousel.ItemsSource = new Dictionary<int, string>
            {
                {1, "How are you" },
                {2, "How Old are you?" },
                {3, "What is your name" },

            };
        }

        private void OnShuffleClicked(object sender, EventArgs e)
        {
            // Pick random index
            int index = random.Next(0, 3);

            // Animate transition
            NumberCarousel.Position = index;
        }
    }
}
