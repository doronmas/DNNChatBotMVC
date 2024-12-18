namespace DNNChatBotMVC.Services
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);
    }
}
