using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SillageParfumApi.Application.DTOs;
using SillageParfumApi.Application.Interfaces;

namespace SillageParfumApi.Infrastructure.ExternalServices
{
    public class RapidApiPerfumeService : IExternalPerfumeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _host;

        public RapidApiPerfumeService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            var apiKey = _config["RapidApi:Key"];
            _host = _config["RapidApi:Host"];

            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", _host);
        }

        public async Task<PerfumeExternoDto?> BuscarEnInternetAsync(string perfumeName)
        {
            try
            {
                string url = $"https://{_host}/multi-search";
                var requestBody = new { queries = new[] { new { indexUid = "fragrances", q = perfumeName, limit = 1 } } };
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, jsonContent);

                // DETECTIVE 1: Si RapidAPI nos rechaza la llave o hay error, lanzamos el error visible
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error de RapidAPI ({response.StatusCode}): {errorDetails}");
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                // DETECTIVE 2: Si no devuelve nada, lo indicamos
                if (string.IsNullOrWhiteSpace(jsonString))
                    throw new Exception("RapidAPI devolvió una respuesta vacía.");

                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;

                if (root.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                {
                    var firstResult = results[0];
                    if (firstResult.TryGetProperty("hits", out var hits) && hits.GetArrayLength() > 0)
                    {
                        var data = hits[0];

                        // Mejoramos la lectura de la marca por si viene como objeto anidado
                        string brandName = "";
                        if (data.TryGetProperty("brand", out var b))
                        {
                            if (b.ValueKind == JsonValueKind.Object && b.TryGetProperty("name", out var bn))
                                brandName = bn.GetString() ?? "";
                            else if (b.ValueKind == JsonValueKind.String)
                                brandName = b.GetString() ?? "";
                        }
                        // Limpiamos las notas olfativas para que se lean bonito
                        string notasLimpias = "";
                        if (data.TryGetProperty("notes", out var notesArray) && notesArray.ValueKind == JsonValueKind.Array)
                        {
                            var listaNotas = new List<string>();
                            foreach (var nota in notesArray.EnumerateArray())
                            {
                                if (nota.TryGetProperty("name", out var noteName))
                                {
                                    listaNotas.Add(noteName.GetString() ?? "");
                                }
                            }
                            // Unimos todo con comas: "Amber, Basil, Bergamot..."
                            notasLimpias = string.Join(", ", listaNotas);
                        }
                        // Extraemos la imagen Base64 escondida en el objeto 'image'
                        string imageUrl = "";
                        if (data.TryGetProperty("image", out var imageObj) && imageObj.ValueKind == JsonValueKind.Object)
                        {
                            if (imageObj.TryGetProperty("imageData", out var imgDataObj) &&
                                imgDataObj.ValueKind == JsonValueKind.Object &&
                                imgDataObj.TryGetProperty("base64", out var base64Prop))
                            {
                                imageUrl = base64Prop.GetString() ?? "";
                            }
                        }

                        //string jsonCrudo = data.ToString();//Este Json es la respuesta cruda que devuelve RapidAPI, útil para depurar si algo no se mapea bien a nuestro DTO

                        return new PerfumeExternoDto
                        {
                            Name = data.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String ? n.GetString() ?? "" : "",
                            Brand = brandName,
                            Description = "Sin descripción oficial disponible.",
                            ImageUrl = imageUrl,
                            OlfactoryNotes = notasLimpias
                        };
                    }
                }

                // Si no falla, pero literalmente Lacoste no existe en su base de datos
                return null;
            }
            catch (Exception ex)
            {
                // DETECTIVE 3: En lugar de ocultar el fallo, se lo mandamos al Controlador para verlo en Swagger
                throw new Exception($"Fallo interno al procesar la API: {ex.Message}");
            }
        }
    }
}