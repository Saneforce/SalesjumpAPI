using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokencreateController : ControllerBase
    {
        private readonly HttpClient _httpClient;


        public object JSON { get; private set; }

        public TokencreateController()
        {

            _httpClient = new HttpClient();
        }

        [HttpPost]
        [Route("GetTokencreate")]
        public async Task<string> GenerateTokenAsync()
        {
            try{
                var requestUrl = "https://iamhcdev.ril.com/oauth2/token";


                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "V0xRdWZSRXR6M3E4UnNWemVncGFkVXUwdnRJYTpTQUZIdk9tUlVLY3BlTzdQNzcyckpVNUpScmth");


                var response = await _httpClient.PostAsync(requestUrl, requestData);


                if (response.IsSuccessStatusCode)
                {
                    var responsesContent = await response.Content.ReadAsStringAsync();
                    return responsesContent;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("GetTokencreatelive")]
        public async Task<string> GenerateTokenliveAsync()
        {
            try
            {
                var requestUrl = "https://apimhc.ril.com/oauth2/token";


                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");


                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "UjhneENlc2J2QVNleU5ScGhvR19DRmVoa1BFYTp0RFp1VEJleVZqeG9nY0U4SGRBb3h1TmV5Zm9h");


                var response = await _httpClient.PostAsync(requestUrl, requestData);


                if (response.IsSuccessStatusCode)
                {
                    var responsesContent = await response.Content.ReadAsStringAsync();
                    return responsesContent;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
