using Newtonsoft.Json;

namespace LuanVan.Services
{
    public class GoogleCaptchaService
    {
        public GoogleCaptchaService() { }

        public virtual async Task<GoogleResponse> VerifyreCaptcha(string _token )
        {
            GoogleCaptchaData googleCaptchaData = new GoogleCaptchaData
            {
                response = _token,
                secrect = "6Le5mEwlAAAAADUX8mJxDY1YL-zHg7o6ypKvPVdb"

            };

            HttpClient client= new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={googleCaptchaData.secrect}&response={googleCaptchaData.response}");

            var capreponse= JsonConvert.DeserializeObject<GoogleResponse>(response);

            return capreponse;

        }
    }

    public class GoogleCaptchaData
    {
        public string response { get; set; } // Token

        public string secrect { get; set; }
    }

    public class GoogleResponse
    {
        public bool success { get; set; }

        public double score { get; set; }

        public string action { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }


    }
}
