using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using MvcExamen2sacg.Models;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Azure.Storage.Files.Shares;/*importante */

namespace MvcExamen2sacg.Services
{
    public class ServiceLogicAppUsuarios
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;
        private ShareDirectoryClient Root;

        public ServiceLogicAppUsuarios(string urlapi, string keys)
        {

            this.UrlApi = urlapi;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                string request = "/api/login/validarusuario";
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    string token = jObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        //METODO CON SEGURIDAD QUE RECIBE EL TOKEN
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        public async Task<List<Ticket>> GetTicketsAsync(string token,string id) 
        {
            string request = "/api/tickets/ticketsusuario/" + id;
            List<Ticket> tickets = await this.CallApiAsync<List<Ticket>>(request, token);
            return tickets;
        }

        public async Task CreateUsuarioAsync(int idusuario, string nombre, string apellidos, string email, string username, string password, string token) 
        {
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/tickets/createalumno";

                UsuarioTicket usuario = new UsuarioTicket { IdUsuario = idusuario,Nombre=nombre,Apellidos=apellidos,Email=email,Username=username,Password=password };

                string json = JsonConvert.SerializeObject(usuario);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task CreateTicketAsync(int idticket,int idusuario, DateTime fecha,string importe,string producto,string filename,string storagepath,string token, string fileName,Stream stream) 
        {
            ShareFileClient file = this.Root.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);//creamos el fichero con un tamaño
            await file.UploadAsync(stream);//subimos el fichero 


            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/tickets/createticket";

                Ticket ticket = new Ticket { IdTicket= idticket,IdUsuario = idusuario, Fecha=fecha,Importe = importe,Producto=producto,Filename=filename,Storagepath=""};

                string json = JsonConvert.SerializeObject(ticket);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

    }
}
