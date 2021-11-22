using Microsoft.AspNetCore.JsonPatch; 
using RequesterTool.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RequesterTool
{
    public class Requester : IRequester
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly JsonSerializerOptions _options;
        public Requester(string BaseUri)
        {
            _httpClient.BaseAddress = new Uri(BaseUri);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);

            //Permet d'éviter le problème des LowerCamelCase vs UpperCamelCase
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

       
        public async Task<List<T>> Get<T>(string route)
        {
            /*Définir les type de média supportés*/
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            /*****************************************/

            HttpResponseMessage response = await _httpClient.GetAsync(route);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(content, _options);
        }
        public async Task<bool> Post<T>(string route, T element)
        {
            return await  SendToApi<T>(route, element, HttpMethod.Post);
        }

        public async Task<bool> Put<T>(string route, T element)
        {
            return await  SendToApi<T>(route, element, HttpMethod.Put);
        }

        public async Task<bool> Delete<T>(string route, T identifiant)
        {
            try
            {
                var uri = Path.Combine(route , identifiant.ToString());
                var response = await _httpClient.DeleteAsync(uri);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        private async Task<bool> SendToApi<T>(string route, T element, HttpMethod method)
        {
            try
            {
                string elementAsJson = JsonSerializer.Serialize(element);
                HttpRequestMessage request = new HttpRequestMessage(method, route);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(elementAsJson, Encoding.UTF8);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

      

        public async Task<bool> Patch<T,U>(string route, U identifiant, JsonPatchDocument<T> patchDoc)
            where T:class
        {
            try
            {
                var uri = Path.Combine(route + @"\", identifiant.ToString());
                var serializedDoc = JsonSerializer.Serialize(patchDoc);
                var request = new HttpRequestMessage(HttpMethod.Patch, uri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(serializedDoc, Encoding.UTF8);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }





    }
}
