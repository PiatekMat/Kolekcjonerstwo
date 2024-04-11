using System.Diagnostics;

namespace Kolekcja;

public partial class SingleCollectionPage : ContentPage
{
    List<string> przedmioty = new List<string>();
    private string fileName;
    private string folderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Kolekcje");
    List<SingleItem> items = new List<SingleItem>();
    List<SingleItem> SelledItems = new List<SingleItem>();
    List<SingleItem> NotSelledItems = new List<SingleItem>();

    //C:\Users\matik\AppData\Local\Packages\430f614c-c8f4-4fac-a664-103e3da1dbd9_9zz4h110yvjzm\LocalState\Kolekcje

    public SingleCollectionPage(string filename)
	{
		InitializeComponent();
        this.fileName = filename + ".txt";
        LoadItems();
        if (!File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Create(Path.Combine(folderPath, fileName));
        }
        Trace.WriteLine(folderPath);
    }
    private async void DodajPrzedmiot_Clicked(object sender, EventArgs e)
    {
        string name = "";
        string price = "";
        string sprzedane = "";
        string zadowolenie = "";
        string przedmioty_row = "";
        if (!string.IsNullOrWhiteSpace(NewItemEditor.Text) && !string.IsNullOrWhiteSpace(NewItemPrice.Text))
        {
            name = NewItemEditor.Text;
            price = NewItemPrice.Text;
            sprzedane = Sell.SelectedItem.ToString();
            zadowolenie = NewZadowolenie.Text;
            przedmioty_row = name + ";" + price + ";" + sprzedane + ";" + zadowolenie;
            foreach (SingleItem s in items)
            {
                if (s.Name == name)
                {
                    if (!await DisplayAlert("Ponowne wprowadzanie", "Czy chcesz wprowadziæ ponownie ten sam przedmiot", "Tak", "Nie"))
                    {
                        return;
                    }
                }
            }
        przedmioty.Add(przedmioty_row);
        SaveItems();
        LoadItems();
        NewItemEditor.Text = null;
        NewItemPrice.Text = null;
        }
        else
        {
            DisplayAlert("B³¹d", "Prosze wprowadziæ przedmiot", "OK");
        }
    }
    private void LoadItems()
    {
        items.Clear();
        SelledItems.Clear();
        NotSelledItems.Clear();
        CollectionsCollectionView.ItemsSource = null;
        string filePath = Path.Combine(folderPath, fileName);
        if (File.Exists(filePath))
        {
            przedmioty = File.ReadAllLines(filePath).ToList();
        }
        foreach (string s in przedmioty)
        {
            string[] sa = s.Split(";");
            SingleItem it = new SingleItem();
            it.Name = sa[0];
            it.Price = sa[1];
            it.selled = sa[2];
            it.zadowolenie = sa[3];
            if (it.selled == "Sprzedane")
            {
                SelledItems.Add(it);
                it.op = 0.5;
            }
            else
            {
                NotSelledItems.Add(it);
                it.op = 1;
            }
        }
        foreach(SingleItem it in NotSelledItems)
        {
            items.Add(it);
        }
        foreach (SingleItem it in SelledItems)
        {
            items.Add(it);
        }
        CollectionsCollectionView.ItemsSource = items;
    }
    private void SaveItems()
    {
        try
        {
            File.WriteAllText(Path.Combine(folderPath, fileName), string.Join(Environment.NewLine, przedmioty));
        }
        catch (Exception ex)
        {
            DisplayAlert("B³¹d", $"B³¹d podczas zapisywania kolekcji: {ex.Message}", "OK");
        }

    }
    private async void Edytuj(object sender, SelectionChangedEventArgs e)
    {
        if (CollectionsCollectionView.SelectedItem != null)
        {
            string[] options = { "Sprzedane", "Nie sprzedane" };
            var selectedItem = (SingleItem)CollectionsCollectionView.SelectedItem;
            var id = 0;

            string newName = await DisplayPromptAsync($"Edytujesz przedmiot: {selectedItem.Name}", "", initialValue: selectedItem.Name);
            string newPrice = await DisplayPromptAsync($"Edytujesz cenê: {selectedItem.Price}", "", initialValue: selectedItem.Price);
            string newSell = await DisplayActionSheet("Wybierz status", "Cancel", null, options);
            string newOpinion = await DisplayPromptAsync($"Zmien ocenê w skali od 1 do 10, obecna ocena: {selectedItem.zadowolenie}", "", initialValue: selectedItem.zadowolenie);
            
            if (!string.IsNullOrWhiteSpace(newName) && !string.IsNullOrWhiteSpace(newPrice))
            {
                id = przedmioty.IndexOf(selectedItem.Name + ";" + selectedItem.Price + ";" + selectedItem.selled + ";" + selectedItem.zadowolenie);
                przedmioty[id] = newName + ";" + newPrice + ";" + newSell + ";" + newOpinion;
                SaveItems();
                LoadItems();
            }
        }
    }
    public class SingleItem
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string selled { get; set; }
        public string zadowolenie { get; set; }
        public double op {  get; set; }
    }
}