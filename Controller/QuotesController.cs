using ChuckNorrisApi.Data;
using ChuckNorrisApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly ChuckNorrisContext _context;

        public QuotesController(ChuckNorrisContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChuckNorrisQuote>>> GetQuotes()
        {
            return await _context.Quotes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChuckNorrisQuote>> GetQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return NotFound();

            return quote;
        }

        [HttpPost]
        public async Task<ActionResult<ChuckNorrisQuote>> PostQuote(string serializedQuote)
        {
            // Simulating insecure deserialization
            var quote = DeserializeInsecurely(serializedQuote);  // Simulate insecure deserialization

            if (quote == null)
                return BadRequest();

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        // Simulating insecure deserialization method
        private ChuckNorrisQuote DeserializeInsecurely(string serializedData)
        {
            // Unsafe deserialization - using BinaryFormatter, which is vulnerable to attacks
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new System.IO.MemoryStream(System.Convert.FromBase64String(serializedData)))
            {
                return (ChuckNorrisQuote)formatter.Deserialize(stream);  // Insecure deserialization
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuote(int id, ChuckNorrisQuote quote)
        {
            if (id != quote.Id)
                return BadRequest();

            _context.Entry(quote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
                return NotFound();

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuoteExists(int id) => _context.Quotes.Any(e => e.Id == id);
    }
}
