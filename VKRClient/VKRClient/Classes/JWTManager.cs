using System.IO;
using VKRClient.Models;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VKRClient
{
    public static class JWTManager
    {
        private static string DirectoryPath = "C:\\VKR";
        private static string FileName = "tokens.json";
        private static string FullPath = Path.Combine(DirectoryPath, FileName);

        public static void SaveTokens(string Response)
        {
            var Tokens = JsonSerializer.Deserialize<TokenData>(Response);

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            JsonSerializerOptions Options = new JsonSerializerOptions{ WriteIndented = true };
            File.WriteAllText(FullPath, JsonSerializer.Serialize(Tokens, Options));
        }

        public static async Task<bool> SetBearer()
        {
            try
            {
                string Access = GetJWT("jwtAccess");

                if (Access != null)
                {
                    App.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access);
                    var Response = await App.Client.GetAsync("AuthN/Check");

                    if (Response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    return await JWTUpdate();
                }
            }
            catch {}
            return false;
        }

        public static async Task<bool> JWTUpdate()
        {
            try
            {
                string Refresh = GetJWT("jwtRefresh");

                if (Refresh != null)
                {
                    var Response = await App.Client.PostAsJsonAsync("AuthN/JWTUpdate", Refresh);

                    if (Response.IsSuccessStatusCode)
                    {
                        SaveTokens(await Response.Content.ReadAsStringAsync());
                        return await SetBearer();
                    }
                }
            }
            catch {}
            return false;
        }

        private static string GetJWT(string Type)
        {
            if (File.Exists(FullPath))
            {
                using JsonDocument Doc = JsonDocument.Parse(File.ReadAllText(FullPath));
                string? JWT = Doc.RootElement.GetProperty(Type).GetString();

                if (JWT != null)
                {
                    return JWT;
                }
            }

            return string.Empty;
        }
    }
}
