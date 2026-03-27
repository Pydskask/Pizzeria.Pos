using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Pizzeria.Pos.Wpf.ViewModels;

public partial class SettingsPanelViewModel : ObservableObject
{
    private readonly IBackupService backupService;

    [ObservableProperty] private string selectedTheme = "Ciemny";
    [ObservableProperty] private string selectedAccent = "Niebieski";
    [ObservableProperty] private string selectedFontSize = "Normalny";
    [ObservableProperty] private bool isTouchMode = false;
    [ObservableProperty] private bool showSaveConfirmation = true;
    [ObservableProperty] private string statusMessage = "Gotowe.";
    [ObservableProperty] private Brush previewAccentBrush = new SolidColorBrush(Color.FromRgb(37, 99, 235));
    [ObservableProperty] private double previewFontSize = 17;
    [ObservableProperty] private string fontSizeSummary = "Normalny (17px)";

    [ObservableProperty] private ObservableCollection<BackupFileRow> backupFiles = new();
    [ObservableProperty] private string backupFolderPath = string.Empty;
    [ObservableProperty] private string backupStatusMessage = string.Empty;

    public ObservableCollection<string> ThemeOptions { get; } = new() { "Ciemny", "Jasny" };
    public ObservableCollection<string> AccentOptions { get; } = new() { "Niebieski", "Zielony", "Pomarańczowy", "Fioletowy" };
    public ObservableCollection<string> FontSizeOptions { get; } = new() { "Mały", "Normalny", "Duży", "Bardzo duży" };

    public SettingsPanelViewModel(IBackupService backupService)
    {
        this.backupService = backupService;
        RefreshPreview();
        LoadBackupList();
    }

    // ── SETTINGS ──────────────────────────────────────────

    [RelayCommand]
    private void Save()
    {
        RefreshPreview();
        StatusMessage = $"Ustawienia zapisane ({DateTime.Now:HH:mm:ss}).";
    }

    [RelayCommand]
    private void ResetDefaults()
    {
        SelectedTheme = "Ciemny";
        SelectedAccent = "Niebieski";
        SelectedFontSize = "Normalny";
        IsTouchMode = false;
        ShowSaveConfirmation = true;
        RefreshPreview();
        StatusMessage = "Przywrócono ustawienia domyślne.";
    }

    [RelayCommand]
    private void RefreshPreview()
    {
        PreviewAccentBrush = SelectedAccent switch
        {
            "Zielony" => new SolidColorBrush(Color.FromRgb(22, 163, 74)),
            "Pomarańczowy" => new SolidColorBrush(Color.FromRgb(234, 88, 12)),
            "Fioletowy" => new SolidColorBrush(Color.FromRgb(124, 58, 237)),
            _ => new SolidColorBrush(Color.FromRgb(37, 99, 235))
        };

        PreviewFontSize = SelectedFontSize switch
        {
            "Mały" => 14,
            "Duży" => 20,
            "Bardzo duży" => 24,
            _ => 17
        };

        FontSizeSummary = $"{SelectedFontSize} ({PreviewFontSize}px)";
    }

    // ── BACKUP ────────────────────────────────────────────

    [RelayCommand]
    private void CreateBackupNow()
    {
        BackupStatusMessage = "Tworzę backup...";
        var path = backupService.CreateBackup();

        if (path != null)
        {
            BackupStatusMessage = $"✔ Backup gotowy: {Path.GetFileName(path)}";
            StatusMessage = $"Backup wykonany o {DateTime.Now:HH:mm:ss}.";
        }
        else
        {
            BackupStatusMessage = "⚠ Backup nie powiódł się! Sprawdź uprawnienia do folderu.";
            StatusMessage = "Błąd backupu.";
        }

        LoadBackupList();
    }

    [RelayCommand]
    private void OpenBackupFolder()
    {
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        Directory.CreateDirectory(dir);
        Process.Start("explorer.exe", dir);
    }

    [RelayCommand]
    private void RefreshBackupList()
    {
        LoadBackupList();
        BackupStatusMessage = $"Lista odświeżona ({DateTime.Now:HH:mm:ss}).";
    }

    private void LoadBackupList()
    {
        BackupFiles.Clear();
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        if (!Directory.Exists(dir))
        {
            BackupFolderPath = dir;
            return;
        }

        BackupFolderPath = dir;

        var files = Directory.GetFiles(dir, "app_backup_*.db")
            .OrderByDescending(f => File.GetCreationTime(f))
            .Take(15)
            .Select(f => new BackupFileRow
            {
                FileName = Path.GetFileName(f),
                CreatedAt = File.GetCreationTime(f),
                SizeKb = new FileInfo(f).Length / 1024
            });

        foreach (var row in files)
            BackupFiles.Add(row);
    }
}

public class BackupFileRow
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long SizeKb { get; set; }
    public string Display => $"{FileName}   ({CreatedAt:dd.MM.yyyy HH:mm})   {SizeKb} KB";
}