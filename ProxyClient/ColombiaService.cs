using MovilidadSostenible_YAMAHA.Models;
using Newtonsoft.Json;

namespace MovilidadSostenible_YAMAHA.ProxyClient
{
    public class ColombiaService
    {
        private readonly HttpClient _httpClient;

        public ColombiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DepartamentoViewModel>> GetListDepartamentosAsync()
        {
            var response = await _httpClient.GetStringAsync("https://api-colombia.com/api/v1/Department");
            var departments = JsonConvert.DeserializeObject<List<DepartamentoViewModel>>(response);
            return departments.OrderBy(d => d.name).ToList();
        }

        public async Task<List<CiudadViewModel>> GetListCiudadesAsync(int departamentoId)
        {

            var response = await _httpClient.GetStringAsync($"https://api-colombia.com/api/v1/Department/{departamentoId}/cities");
            var cities = JsonConvert.DeserializeObject<List<CiudadViewModel>>(response);
            return cities.OrderBy(c => c.name).ToList();
        }
    }
}
