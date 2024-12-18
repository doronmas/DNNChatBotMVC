using DNNChatBotMVC.Models;

namespace DNNChatBotMVC.Repositories
{
    public interface IContentRepository
    {
        Task<IEnumerable<ContentEmbedding>> GetAllAsync();
        Task<ContentEmbedding> GetByIdAsync(int id);
        Task<IEnumerable<ContentEmbedding>> SearchAsync(float[] queryEmbedding, int topK = 5);
        Task AddAsync(ContentEmbedding content);
        Task UpdateAsync(ContentEmbedding content);
        Task DeleteAsync(int id);
    }
}
