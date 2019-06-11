using InkApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace InkApp.Services
{
    public class InstagramParser
    {
        private WebClient web;

        public InstagramParser()
        {
            web = new WebClient();
        }

        public async System.Threading.Tasks.Task<bool> GetUserAsync(Pessoa p)
        {

            string url = @"https://www.instagram.com/" +p.Username+"/?__a=1";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var o = JObject.Parse(json);

                        p.Image = o.SelectToken("graphql.user.profile_pic_url").Value<string>();
                        p.IdInsta = o.SelectToken("graphql.user.id").Value<string>();
                        return true;
                    }
                }

                
            }catch(Exception)
            {
                
            }

            return false;

        }

        public async System.Threading.Tasks.Task<List<InstagramItem>> GetMediaAsync(Pessoa p, int quant)
        {
            List<InstagramItem> items = new List<InstagramItem>();
            string s = @"https://www.instagram.com/graphql/query/?query_hash=472f257a40c653c64c666ce877d59d2b&variables={";

            string ss = "\"id\"" + ":\"" +p.IdInsta +"\",\"first\":"+ quant +"}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(s + ss);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        JObject o = JObject.Parse(json);

                        for (var i = 0; i < quant; i++)
                        {
                            try
                            {
                                if (o.SelectToken("data.user.edge_owner_to_timeline_media.edges[" + i + "].node.__typename").Value<string>().Equals("GraphImage"))
                                {
                                    var xx = o.SelectToken("data.user.edge_owner_to_timeline_media.edges[" + i + "].node.display_url").Value<string>();
                                    var yy = o.SelectToken("data.user.edge_owner_to_timeline_media.edges[" + i + "].node.thumbnail_src").Value<string>();
                                    items.Add(new InstagramItem() { ImageLow = yy, ImageHigh = xx, People = p, Username = p.Username });
                                }
                            }
                            catch (Exception)
                            {
                                return items;
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return items;
        }
    }
}
