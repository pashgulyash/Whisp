using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

public sealed partial class MainPage : Page
{
    private ObservableCollection<Note> notes = new ObservableCollection<Note>();
    private const string FileName = "diary_notes.json";

    public MainPage()
{
    this.InitializeComponent();
    NotesList.ItemsSource = notes;
    LoadNotes();
}

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
{
    if (!string.IsNullOrEmpty(NoteTextBox.Text))
    {
        var note = new Note
        {
            Id = Guid.NewGuid().ToString(),
            Text = NoteTextBox.Text,
            Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
        };
        
        notes.Add(note); // ListView обновится сам!
        NoteTextBox.Text = "";
        await SaveToFile();
    }
}

    // Удаление заметки
    private async void DeleteNote_Click(object sender, RoutedEventArgs e)
{
    var button = (Button)sender;
    string id = button.Tag.ToString();
    
    var noteToRemove = notes.FirstOrDefault(n => n.Id == id);
    if (noteToRemove != null)
    {
        notes.Remove(noteToRemove); // ListView обновится сам!
        await SaveToFile();
    }
}

    // Сохранение в файл
    private async System.Threading.Tasks.Task SaveToFile()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(notes);
        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteTextAsync(file, json);
    }

private async void LoadNotes()
{
    try
    {
        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
        var json = await FileIO.ReadTextAsync(file);
        var loadedNotes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Note>>(json);
        if (loadedNotes != null)
        {
        notes.Clear();
        foreach (var note in loadedNotes) notes.Add(note);
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
    }
}
}