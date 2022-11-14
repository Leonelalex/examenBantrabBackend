using examenApiBackend.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace examenApiBackend.Controllers
{
    [ApiController]
    [Route("imagenes")]
    public class ImagenesController : ControllerBase
    {
        public bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }

        [HttpGet]
        public async Task<ActionResult> getImagenes()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("user", "User123");
                client.DefaultRequestHeaders.Add("password", "Password123");

                var result = await client.GetAsync("https://apitest-bt.herokuapp.com/api/v1/imagenes");

                if (result.IsSuccessStatusCode)
                {
                    string respuestastring = await result.Content.ReadAsStringAsync();

                    IEnumerable<ImagenDTO> imagenes = JsonSerializer.Deserialize<IEnumerable<ImagenDTO>>(respuestastring, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    IEnumerable<ImagenDTO> imagenesValidas = Enumerable.Empty<ImagenDTO>();

                    foreach(ImagenDTO img in imagenes)
                    {
                        if (IsBase64String(img.base64))
                            imagenesValidas.Append(img);
                    }

                    return Ok(imagenesValidas);


                }

                return StatusCode(500);
            }
            catch (WebException)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult> postImagen([FromQuery] string fileName)
        {
            var file = Request.Form.Files[0];

            string extencion = file.FileName.Split(".")[1].ToLower();

            if(extencion != "png" && extencion != "jpg" && extencion != "jpeg")
            {
                return StatusCode(400, "el archivo deje ser de algun formato de imagen");
            }

            var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string base64file = Convert.ToBase64String(fileBytes);

            Imagen nuevaImagen = new Imagen { nombre = fileName, base64 = base64file };

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("user", "User123");
                client.DefaultRequestHeaders.Add("password", "Password123");
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                string test = JsonSerializer.Serialize(nuevaImagen);

                var content = new StringContent(JsonSerializer.Serialize(nuevaImagen), Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://apitest-bt.herokuapp.com/api/v1/imagenes", content);

                if (result.IsSuccessStatusCode)
                {

                    return Ok("imagen guardada con exito");

                }

                return StatusCode(500);
            }
            catch (WebException)
            {
                return StatusCode(500);
            }
        }
    }
}
