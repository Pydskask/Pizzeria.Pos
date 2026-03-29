using System;
using System.IO;

namespace Pizzeria.Pos.Services;

public class BackupService : IBackupService
{
    private const string DbFileName = "app.db";

    public string? CreateBackup()
    {
        try
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var sourceDb = Path.Combine(appDir, DbFileName);

            if (!File.Exists(sourceDb))
                return null;

            var backupDir = Path.Combine(appDir, "Backups");
            Directory.CreateDirectory(backupDir);

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFileName = $"app_backup_{timestamp}.db";
            var destPath = Path.Combine(backupDir, backupFileName);

            File.Copy(sourceDb, destPath, overwrite: false);
            CleanOldBackups(backupDir, keepDays: 30);

            return destPath;
        }
        catch
        {
            return null;
        }
    }

    private static void CleanOldBackups(string backupDir, int keepDays)
    {
        try
        {
            var cutoff = DateTime.Now.AddDays(-keepDays);
            foreach (var file in Directory.GetFiles(backupDir, "app_backup_*.db"))
            {
                if (File.GetCreationTime(file) < cutoff)
                    File.Delete(file);
            }
        }
        catch { }


    }
}