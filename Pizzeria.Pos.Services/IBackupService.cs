namespace Pizzeria.Pos.Services;

public interface IBackupService
{
    /// <summary>Tworzy kopię zapasową bazy danych. Zwraca ścieżkę do pliku backupu lub null w razie błędu.</summary>
    string? CreateBackup();
}