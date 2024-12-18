using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.Json;
using Azure.AI.OpenAI;

namespace DNNChatBotMVC.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IConfiguration _configuration;
        private readonly OpenAIClient _openAIClient;
        private readonly string? _pythonPath;
        private const string ModelName = "text-embedding-3-small";

        public EmbeddingService(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKey = _configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not found in configuration.");
            _openAIClient = new OpenAIClient(apiKey);
            _pythonPath = _configuration["PythonPath"];
            
            if (string.IsNullOrEmpty(_pythonPath))
            {
                throw new InvalidOperationException("Python path not found in configuration.");
            }
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var embeddingOptions = new EmbeddingsOptions(text, new[] { text });
            var response = await _openAIClient.GetEmbeddingsAsync(embeddingOptions);
            
            if (response.Value.Data.Count == 0)
            {
                throw new InvalidOperationException("Failed to generate embeddings.");
            }

            return response.Value.Data[0].Embedding.ToArray();
        }

        public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrEmpty(_pythonPath))
            {
                throw new InvalidOperationException("Python path not configured.");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = $"PythonServices/translate.py \"{text}\" {sourceLanguage} {targetLanguage}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start Python process.");
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Translation failed: {error}");
            }

            var result = JsonSerializer.Deserialize<TranslationResult>(output);
            if (result == null || !result.Success)
            {
                throw new Exception($"Translation failed: {result?.Error ?? "Unknown error"}");
            }

            return result.TranslatedText ?? throw new Exception("Translation result is null");
        }

        private class TranslationResult
        {
            public bool Success { get; set; }
            public string? TranslatedText { get; set; }
            public string? Error { get; set; }
        }
    }
}
