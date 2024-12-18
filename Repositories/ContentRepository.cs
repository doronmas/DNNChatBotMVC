using DNNChatBotMVC.Data;
using DNNChatBotMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DNNChatBotMVC.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext _context;

        public ContentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContentEmbedding>> GetAllAsync()
        {
            return await _context.ContentEmbeddings.ToListAsync();
        }

        public async Task<ContentEmbedding> GetByIdAsync(int id)
        {
            return await _context.ContentEmbeddings.FindAsync(id);
        }

        public async Task<IEnumerable<ContentEmbedding>> SearchAsync(float[] queryEmbedding, int topK = 5)
        {
            var allEmbeddings = await _context.ContentEmbeddings.ToListAsync();
            
            return allEmbeddings
                .Select(e => new
                {
                    Embedding = e,
                    Similarity = CosineSimilarity(queryEmbedding, BytesToFloatArray(e.Embedding))
                })
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .Select(x => x.Embedding);
        }

        public async Task AddAsync(ContentEmbedding content)
        {
            await _context.ContentEmbeddings.AddAsync(content);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ContentEmbedding content)
        {
            _context.ContentEmbeddings.Update(content);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var content = await GetByIdAsync(id);
            if (content != null)
            {
                _context.ContentEmbeddings.Remove(content);
                await _context.SaveChangesAsync();
            }
        }

        private float CosineSimilarity(float[] a, float[] b)
        {
            float dotProduct = 0;
            float normA = 0;
            float normB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            return dotProduct / (float)(Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        private float[] BytesToFloatArray(byte[] bytes)
        {
            float[] floats = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
            return floats;
        }
    }
}
