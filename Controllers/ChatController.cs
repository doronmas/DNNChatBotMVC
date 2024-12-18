using Microsoft.AspNetCore.Mvc;
using DNNChatBotMVC.Services;
using DNNChatBotMVC.Repositories;
using DNNChatBotMVC.Models;

namespace DNNChatBotMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IContentRepository _contentRepository;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IEmbeddingService embeddingService, 
            IContentRepository contentRepository,
            ILogger<ChatController> logger)
        {
            _embeddingService = embeddingService;
            _contentRepository = contentRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessMessage([FromBody] ChatMessage message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message))
                {
                    return BadRequest("Message cannot be empty");
                }

                // Translate to English if the message is in Hebrew
                string processedText = message.Message;
                if (ContainsHebrew(message.Message) || message.TranslateToEnglish)
                {
                    processedText = await _embeddingService.TranslateTextAsync(message.Message, "he", "en");
                }

                // Generate embedding for the query
                var embedding = await _embeddingService.GenerateEmbeddingAsync(processedText);

                // Search for relevant content
                var relevantContent = await _contentRepository.SearchAsync(embedding);

                // Prepare response based on the most relevant content
                var response = PrepareResponse(relevantContent);

                // Translate response back to Hebrew if the original message was in Hebrew
                if (ContainsHebrew(message.Message) && !message.TranslateToEnglish)
                {
                    response = await _embeddingService.TranslateTextAsync(response, "en", "he");
                }

                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        private bool ContainsHebrew(string text)
        {
            return text.Any(c => c >= 0x0590 && c <= 0x05FF);
        }

        private string PrepareResponse(IEnumerable<ContentEmbedding> relevantContent)
        {
            if (!relevantContent.Any())
            {
                return "I'm sorry, I couldn't find any relevant information for your query.";
            }

            var mostRelevant = relevantContent.First();
            return $"{mostRelevant.Content}\n\nSource: {mostRelevant.Title}";
        }
    }

    public class ChatMessage
    {
        public required string Message { get; set; }
        public bool TranslateToEnglish { get; set; }
    }
}
