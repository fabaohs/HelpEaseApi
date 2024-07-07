using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HelpEaseApi.Middlewares
{
    public class DecryptRequestBody
    {
        private readonly RequestDelegate _next;
        private readonly string _encryptionKey;

        public DecryptRequestBody(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            var secret = configuration["Secret"];

            if (secret == null)
            {
                throw new ArgumentNullException("Secret key is missing in appsettings.json");
            }

            _encryptionKey = secret;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post && context.Request.ContentType == "application/json")
            {
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var encryptedContent = await reader.ReadToEndAsync();
                    var decryptedContent = DecryptString(encryptedContent, _encryptionKey);
                    var byteArray = Encoding.UTF8.GetBytes(decryptedContent);
                    context.Request.Body = new MemoryStream(byteArray);
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }

            await _next(context);
        }

        private string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var aes = Aes.Create();
            var key = Encoding.UTF8.GetBytes(keyString);
            aes.Key = key.Take(32).ToArray();
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(cipher);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}