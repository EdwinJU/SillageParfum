using System.Net;
using System.Text.Json;

namespace SillageParfumApi.Middlewares
{
    // Por convención, las clases middleware llevan el sufijo "Middleware"
    public class GlobalExceptionMiddleware
    {
        // El RequestDelegate es el "siguiente empleado" en el pasillo
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Este es el método mágico que se ejecuta con CADA petición que llega a tu API
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Le decimos a la petición: "Sigue caminando por el pasillo hacia el Controlador"
                await _next(context);
            }
            catch (Exception ex)
            {
                // ¡BUM! Si en cualquier parte (Controlador, Servicio o Repositorio) 
                // ocurre un error y nadie lo atrapa, el error rebota hacia arriba y cae aquí.

                // Llamamos a nuestro propio método para manejar el desastre elegantemente
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. Configuramos la respuesta HTTP como un 500 (Error Interno del Servidor)
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 2. Si el error es de negocio (como el precio negativo que programamos en la entidad), 
            // lo convertimos a un 400 Bad Request
            if (exception is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            // 3. Empacamos el error en un JSON bonito en lugar de que la pantalla se ponga roja
            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocurrió un error en el servidor al procesar la petición.",
                // En producción, Detail debería estar oculto. Lo dejamos aquí para que veas qué falló en tus pruebas.
                Detail = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}