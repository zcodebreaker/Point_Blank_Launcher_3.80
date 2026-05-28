using Launcher.PointBlank.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Launcher.PointBlank.Services
{
    public class FileCheck
    {
        private const int MaxBarWidth = 463;
        private const int ChunkSize  = 81920;
        private readonly string _startupPath;

        public FileCheck(string startupPath)
        {
            _startupPath = startupPath;
        }
        public async Task<FileCheckResult> CheckFilesAsync(
            Dictionary<string, string> userFiles,
            IProgress<FileCheckProgress> progress = null)
        {
            FileCheckResult result = new FileCheckResult();

            if (userFiles == null || userFiles.Count == 0)
            {
                result.Success = false;
                result.Message = "COMMAND_FILE_CHECK_START: Ocorreu um erro ao ler os arquivos da lista.";
                return result;
            }

            int count      = 0;
            int totalFiles = userFiles.Count;

            foreach (var item in userFiles)
            {
                count++;

                string relativePath = item.Key;
                string expectedHash = item.Value;

                string localPath = Path.Combine(
                    _startupPath,
                    relativePath.TrimStart('\\', '/')
                );
                Report(progress, relativePath, count, totalFiles, 0, "Verificando arquivo...");

                if (!File.Exists(localPath))
                {
                    result.InvalidFiles.Add(relativePath);
                    Logger.Log($"Arquivo não encontrado: {relativePath}");

                    Report(progress, relativePath, count, totalFiles, MaxBarWidth, "Arquivo não encontrado.");
                    continue;
                }
                long fileSize = new FileInfo(localPath).Length;
                string localHash;
                try
                {
                    localHash = await ComputeHashAsync(localPath, fileSize, expectedHash, bytesRead =>
                    {
                        int filePct = fileSize > 0 ? (int)(bytesRead * MaxBarWidth / fileSize) : 0;
                        Report(progress, relativePath, count, totalFiles, filePct, "Verificando arquivo...");
                    });
                }
                catch (InvalidOperationException ex)
                {
                    result.InvalidFiles.Add(relativePath);
                    Logger.Log($"{relativePath}: {ex.Message}");

                    Report(progress, relativePath, count, totalFiles, MaxBarWidth, "Hash inválido.");
                    continue;
                }

                bool valid = string.Equals(localHash, expectedHash, StringComparison.OrdinalIgnoreCase);
                if (!valid)
                    result.InvalidFiles.Add(relativePath);

                Report(progress, relativePath, count, totalFiles, MaxBarWidth,
                    valid ? "Arquivo verificado." : "Hash inválido.");
            }
            result.Success = result.InvalidFiles.Count == 0;
            result.Message = result.Success ? "Fim da verificação. Você já pode jogar.": $"Foi detectado [{result.InvalidFiles.Count}] arquivos inválidos.";

            return result;
        }
        private void Report(
            IProgress<FileCheckProgress> progress,
            string file, int count, int totalFiles,
            int fileBarWidth, string status)
        {
            if (progress == null) return;
            int totalBarWidth = (int)(MaxBarWidth * (count - 1 + (double)fileBarWidth / MaxBarWidth) / totalFiles);

            progress.Report(new FileCheckProgress
            {
                CurrentFile   = file,
                CurrentIndex  = count,
                TotalFiles    = totalFiles,
                FileBarWidth  = fileBarWidth,
                TotalBarWidth = totalBarWidth,
                StatusMessage = status
            });
        }
        private async Task<string> ComputeHashAsync(string filePath, long fileSize, string expectedHash, Action<long> onProgress)
        {
            using (HashAlgorithm hashAlgorithm = CreateHashAlgorithm(expectedHash))
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, ChunkSize, useAsync: true))
            {
                byte[] buffer = new byte[ChunkSize];
                long bytesRead = 0;
                long lastReported = 0;
                long reportInterval = Math.Max(fileSize / 50, ChunkSize);
                int read;

                while ((read = await stream.ReadAsync(buffer, 0, ChunkSize).ConfigureAwait(false)) > 0)
                {
                    await Task.Run(() => hashAlgorithm.TransformBlock(buffer, 0, read, null, 0));
                    bytesRead += read;

                    if (bytesRead - lastReported >= reportInterval)
                    {
                        lastReported = bytesRead;
                        onProgress?.Invoke(bytesRead);
                        await Task.Delay(5); // não diminuir de mais para não travar a UI
                    }
                }
                hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);
                onProgress?.Invoke(bytesRead);
                return BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "").ToUpperInvariant();
            }
        }
        private HashAlgorithm CreateHashAlgorithm(string expectedHash)
        {
            if (string.IsNullOrWhiteSpace(expectedHash))
                throw new InvalidOperationException("Hash vazio na lista.");

            switch (expectedHash.Trim().Length)
            {
                case 32:
                    return MD5.Create();
                case 128:
                    return SHA512.Create();
                default:
                    throw new InvalidOperationException($"Formato de hash não suportado ({expectedHash.Length} caracteres).");
            }
        }
    }
}
