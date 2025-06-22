using System.Net.Http;

namespace BulkSyncer.Core
{
    public static class ApiClient
    {
        private static HttpClient _httpClient;
        public static HttpClient GetClient()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }

            return _httpClient;
        }
    }
}
