using ApiProyectoBimestral.Describe;
using ApiProyectoBimestral.Entidades;
using ApiProyectoBimestral.Ocr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ApiProyectoBimestral
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller : ControllerBase
    {

        // POST: api/Imagen
        [HttpPost]
        public async Task<ActionResult<Imagen>> PostImagen(string ruta)
        {
            //API DE RECONOCIMIENTO DE TEXTO
            var api = new System.Net.WebClient();
            api.Headers.Add("Content-type", "application/octet-stream");
            api.Headers.Add("Ocp-Apim-Subscription-Key", "2269f3ef68244285a2e7267feef9cd8e");
            var qs = "language=es&language=true&model-version=latest";
            var url = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/ocr";


            var resp = api.UploadFile(url + "?" + qs, "POST", ruta);
            var json = System.Text.Encoding.UTF8.GetString(resp);
            var texto = Newtonsoft.Json.JsonConvert.DeserializeObject<ocr_response>(json);


            //API DE DESCRIPCION DE IMAGEN
            var apiO = new System.Net.WebClient();
            apiO.Headers.Add("Content-type", "application/octet-stream");
            apiO.Headers.Add("Ocp-Apim-Subscription-Key", "2269f3ef68244285a2e7267feef9cd8e");
            var qsO = "maxCandidates=1&language=es&model-version=latest";
            var urlO = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/describe";


            var respO = apiO.UploadFile(urlO + "?" + qsO, "POST", ruta);
            var jsonO = Encoding.UTF8.GetString(respO);
            var description = Newtonsoft.Json.JsonConvert.DeserializeObject<describe_response>(jsonO);

            return Ok("TEXTO: " + htmlOcr(texto, "center") + DescriptionHtml(description));
        }

        private static string textoOcr(ocr_response resp)
        {
            return htmlOcr(resp).Replace("<p>", "").Replace("/p>", "").Replace("<br/>", "\r\n");
        }

        private static string htmlOcr(ocr_response resp)
        {
            var txt = "<br/>";

            foreach (var region in resp.regions)
            {
                txt += "<p>";
                foreach (var line in region.lines)
                {
                    foreach (var word in line.words)
                    {
                        txt += word.text + " ";
                    }
                    txt += "<br/>";
                }
                txt += "</p>";
            }

            return txt;
        }

        private static string DescriptionHtml(describe_response resp)
        {
            var text = "<p> DESCRIPCIÓN: <br/>";

            foreach (var Caution in resp.description.captions)
            {
                text += "► " + Caution.text + "<br/>";
            }
            text += "</p>";
            return text;
        }

        private static string DescriptionTexto(describe_response resp)
        {
            return DescriptionHtml(resp).Replace("<p>", "").Replace("/p>", "").Replace("<br/>", "\r\n");
        }

        private static string htmlOcr(ocr_response resp, string justificacion)
        {
            return htmlOcr(resp).Replace("<p>", $"<p style='text-align:{justificacion}'>");
        }

    }
}
