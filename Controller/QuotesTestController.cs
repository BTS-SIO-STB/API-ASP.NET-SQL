using ChuckNorrisApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace ChuckNorrisApi.Controllers
{
    [ApiController]
    [Route("api/Test/[controller]")]
    public class QuotesTestController : ControllerBase
    {
        // POST: api/quotes/serialize
        [HttpPost("serialize")]
        public IActionResult SerializeQuote([FromBody] ChuckNorrisQuote quote)
        {
            if (quote == null)
                return BadRequest("Quote is required.");

            // Serialize the quote to a Base64 string
            string base64SerializedData = SerializeToBase64(quote);

            // Return the Base64 serialized string as a response
            return Ok(new { Base64SerializedData = base64SerializedData });
        }

        // Method to serialize the quote to Base64 using System.Text.Json
        private string SerializeToBase64(ChuckNorrisQuote quote)
        {
            // Serialize the object to a byte array using System.Text.Json
            var jsonData = JsonSerializer.Serialize(quote);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Convert the byte array to a Base64 string
            return Convert.ToBase64String(byteArray);
        }
    }
}
