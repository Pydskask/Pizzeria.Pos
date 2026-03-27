using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Pizzeria.Pos.Wpf.ViewModels
{
    public partial class SettingsPanelViewModel : ObservableObject
    {
        public ObservableCollection<string> ThemeOptions { get; } = new()
        {
            "Ciemny",
            "Jasny"
        };

        public ObservableCollection<string> AccentOptions { get; } = new()
        {
            "Niebieski",
            "Zielony",
            "Pomarańczowy",
            "Czerwony"
        };

        public ObservableCollection<string> FontSizeOptions { get; } = new()
        {
            "Mała",
            "Średnia",
            "Duża"
        };

        [ObservableProperty]
        private string selectedTheme = "Ciemny";

        [ObservableProperty]
        private string selectedAccent = "Niebieski";

        [ObservableProperty]
        private string selectedFontSize = "Średnia";

        [ObservableProperty]
        private bool isTouchMode = true;

        [ObservableProperty]
        private bool showSaveConfirmation = true;

        [ObservableProperty]
        private string statusMessage = "Gotowe do konfiguracji.";

        public Brush PreviewAccentBrush => SelectedAccent switch
        {
            "Zielony" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
            "Pomarańczowy" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA580C")),
            "Czerwony" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"))
        };

        public double PreviewFontSize => SelectedFontSize switch
        {
            "Mała" => 16,
            "Duża" => 24,
            _ => 20
        };

        public string FontSizeSummary => SelectedFontSize switch
        {
            "Mała" => "Mała czcionka",
            "Duża" => "Duża czcionka",
            _ => "Średnia czcionka"
        };

        partial void OnSelectedAccentChanged(string value)
        {
            OnPropertyChanged(nameof(PreviewAccentBrush));
            StatusMessage = $"Wybrano akcent: {value}.";
        }

        partial void OnSelectedFontSizeChanged(string value)
        {
            OnPropertyChanged(nameof(PreviewFontSize));
            OnPropertyChanged(nameof(FontSizeSummary));
            StatusMessage = $"Wybrano rozmiar czcionki: {value}.";
        }

        partial void OnSelectedThemeChanged(string value)
        {
            StatusMessage = $"Wybrano motyw: {value}.";
        }

        [RelayCommand]
        private void Save()
        {
            StatusMessage = "Ustawienia zostały zapisane.";
        }

        [RelayCommand]
        private void ResetDefaults()
        {
            SelectedTheme = "Ciemny";
            SelectedAccent = "Niebieski";
            SelectedFontSize = "Średnia";
            IsTouchMode = true;
            ShowSaveConfirmation = true;
            StatusMessage = "Przywrócono ustawienia domyślne.";
        }

        [RelayCommand]
        private void RefreshPreview()
        {
            OnPropertyChanged(nameof(PreviewAccentBrush));
            OnPropertyChanged(nameof(PreviewFontSize));
            OnPropertyChanged(nameof(FontSizeSummary));
            StatusMessage = "Odświeżono podgląd ustawień.";
        }
    }
}