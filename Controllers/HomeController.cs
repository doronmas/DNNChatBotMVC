using Microsoft.AspNetCore.Mvc;
using DNNChatBotMVC.Models;
using DNNChatBotMVC.Services;

namespace DNNChatBotMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmbeddingService _embeddingService;

        public HomeController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                string response;
                if (request.TranslateToEnglish)
                {
                    // First translate to English
                    string translatedQuery = await _embeddingService.TranslateTextAsync(request.Message, "he", "en");
                    
                    // Process the query and get response in English
                    // TODO: Add your chat processing logic here
                    response = $"Your translated message was: {translatedQuery}";
                    
                    // Translate response back to Hebrew
                    response = await _embeddingService.TranslateTextAsync(response, "en", "he");
                }
                else
                {
                    // Process the Hebrew query directly
                    // TODO: Add your chat processing logic here
                    response = $"קיבלתי את ההודעה שלך: {request.Message}";
                }

                return Json(new { success = true, message = response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "אירעה שגיאה בעת עיבוד הבקשה" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
        public bool TranslateToEnglish { get; set; }
    }
}
