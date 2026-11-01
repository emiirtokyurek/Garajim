namespace Garajim.App.ViewModels;

/// <summary>Henüz yazılmamış modüller için geçici sayfa.</summary>
public sealed class PlaceholderViewModel
{
    public PlaceholderViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; }
    public string Description { get; }
}
