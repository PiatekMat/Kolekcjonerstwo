

namespace Kolekcja
{
    public partial class MainPage : ContentPage
    {
        private string folderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Kolekcje");
        string fileName = "AllColection.txt";

        List<String> Kolekcje = new List<String>();


        public MainPage()
        {
            InitializeComponent();
            LoadCollection();
          
            if (!File.Exists(Path.Combine(folderPath, fileName)))
            {
                File.Create(Path.Combine(folderPath, fileName));
            }

        }
        private void CategoryChoice(object sender, SelectionChangedEventArgs e)
        {
            Navigation.PushAsync(new SingleCollectionPage(CollectionsCollectionView.SelectedItem?.ToString()));
        }
        private void DodajKolekcje_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewCollectionEditor.Text))
            {
                string NewCollection = NewCollectionEditor.Text;
                foreach (string s in Kolekcje)
                {
                    if(s == NewCollection)
                    {
                        DisplayAlert("Błąd", $"Taka kolekcja już isnieje", "OK");
                        return;
                    }
                }
                Kolekcje.Add(NewCollection);
                SaveCollection();
                LoadCollection();
                NewCollectionEditor.Text = null;
            }
            else
            {
                DisplayAlert("Błąd", "Prosze wprowadzić nazwę kolekcji", "OK");

            }
        }

        private void LoadCollection()
        {
            string filePath = Path.Combine(folderPath, fileName);
            if (File.Exists(filePath))
            {
                Kolekcje = File.ReadAllLines(filePath).ToList();
            }
            CollectionsCollectionView.ItemsSource = Kolekcje;
        }
        private void SaveCollection()
        {
            try
            {
                File.WriteAllText(Path.Combine(folderPath, fileName), string.Join(Environment.NewLine, Kolekcje));
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd podczas zapisywania kolekcji: {ex.Message}", "OK");
            }

        }

    }
}