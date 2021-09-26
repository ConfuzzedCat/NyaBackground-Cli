using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace NyaBackgroundTest
{
    public class NekosDotLife
    {
        public string url { get; set; }
        private static readonly HttpClient client = new HttpClient();


        public static async Task NekoImage()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "ConfuzzedCat");

            //var stringTask = client.GetStringAsync("https://nekos.life/api/v2/img/neko");

            using var streamTask = client.GetStreamAsync("https://nekos.life/api/v2/img/neko");

            NekosDotLife nekosDotLifeUrl = await JsonSerializer.DeserializeAsync<NekosDotLife>(await streamTask);
            Console.WriteLine(nekosDotLifeUrl.url);
            ImageHandler.WallpaperSetter(nekosDotLifeUrl.url);
        }
                
    }
}
