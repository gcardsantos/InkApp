using InkApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InkApp.Services
{
    public class InstagramParser
    {
        public InstagramParser()
        {
            
        }

        public async System.Threading.Tasks.Task<bool> GetUserAsync(List<Pessoa> pessoas)
        {
            foreach(var p in pessoas)
            {
                string url = @"https://www.instagram.com/" + p.Username + "/?__a=1";
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
                            p.QtdPosts = o.SelectToken("graphql.user.edge_owner_to_timeline_media.count").Value<int>();
                            p.NextPage = true;
                            
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public async System.Threading.Tasks.Task<Pessoa> GetUserAsync(string username)
        {
            string url = @"https://www.instagram.com/" + username + "/?__a=1";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var o = JObject.Parse(json);

                        Pessoa p = new Pessoa();
                        p.Username = username;
                        p.Image = o.SelectToken("graphql.user.profile_pic_url").Value<string>();
                        p.IdInsta = o.SelectToken("graphql.user.id").Value<string>();
                        p.QtdPosts = o.SelectToken("graphql.user.edge_owner_to_timeline_media.count").Value<int>();
                        p.NextPage = true;
                        return p;
                    }
                }


            }
            catch (Exception)
            {

            }

            return null;
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
                        p.QtdPosts = o.SelectToken("graphql.user.edge_owner_to_timeline_media.count").Value<int>();
                        p.NextPage = true;
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
            try
            {

                while (items.Count < quant)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string s = @"https://www.instagram.com/graphql/query/?query_hash=472f257a40c653c64c666ce877d59d2b&variables={";
                        
                        if (!String.IsNullOrEmpty(p.LastToken))
                        {
                            s += "\"id\"" + ":\"" + p.IdInsta + "\",\"first\":" + "50" + ", \"after\":\"" + p.LastToken + "\"}";
                        }
                        else
                        {
                            s += "\"id\"" + ":\"" + p.IdInsta + "\",\"first\":" + "50" + "}";
                        }

                        var response = await client.GetAsync(s);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            JObject o = JObject.Parse(json);

                            p.NextPage = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.has_next_page").Value<bool>();
                            p.LastToken = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.end_cursor").Value<string>();
                            var l = o.SelectToken("data.user.edge_owner_to_timeline_media.edges").Value<IEnumerable<JToken>>();
                            var list = new List<JToken>(l).FindAll(n => n.ToString().Contains("GraphImage"));
                            if(list.Count > quant)
                            {
                                list.RemoveRange(list.Count - quant - 1, quant);
                            }
                            //p.QtdPosts -= (50 - list.Count);
                            foreach (var x in list)
                            {
                                string t;
                                try
                                {
                                    t = x.SelectToken("node.edge_media_to_caption.edges[0].node.text").Value<string>();
                                }
                                catch (Exception)
                                {
                                    t = "";
                                }


                                InstagramItem i = new InstagramItem()
                                {
                                    ImageLow = x.SelectToken("node.thumbnail_resources[0].src").Value<string>(),
                                    ImageHigh = x.SelectToken("node.display_url").Value<string>(),
                                    Tags = t,
                                    People = p,
                                    Username = p.Username,
                                    Name = p.Name
                                };
                                items.Add(i);
                            }
                            //list.ForEach(n => items.Add(new InstagramItem() { ImageLow = n.SelectToken("node.thumbnail_src").Value<string>(), ImageHigh = n.SelectToken("node.display_url").Value<string>(), People = p, Username = p.Username, Name = p.Name, Tags = n.SelectToken("node.edge_media_to_caption.edges[0].node.text").Value<string>() }));
                        }
                    }
                }
                return items;
            }
            catch (Exception)
            {
                return null;
            }

        }

       
        public async System.Threading.Tasks.Task<List<InstagramItem>> GetAllMediaAsync(Pessoa p)
        {
            List<InstagramItem> items = new List<InstagramItem>();
            try
            {

                while (items.Count < p.QtdPosts && items.Count < 200)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string s = @"https://www.instagram.com/graphql/query/?query_hash=472f257a40c653c64c666ce877d59d2b&variables={";
                        string ss;
                        if (!String.IsNullOrEmpty(p.LastToken))
                        {
                            ss = "\"id\"" + ":\"" + p.IdInsta + "\",\"first\":" + "50" + ", \"after\":\"" + p.LastToken + "\"}";
                        }
                        else
                        {
                            ss = "\"id\"" + ":\"" + p.IdInsta + "\",\"first\":" + "50" + "}";
                        }
                        
                        var response = await client.GetAsync(s + ss);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            JObject o = JObject.Parse(json);

                            p.NextPage = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.has_next_page").Value<bool>();
                            p.LastToken = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.end_cursor").Value<string>();
                            var l = o.SelectToken("data.user.edge_owner_to_timeline_media.edges").Value<IEnumerable<JToken>>();
                            var list = new List<JToken>(l).FindAll(n => n.ToString().Contains("GraphImage"));
                            p.QtdPosts -= (50 - list.Count);

                            foreach (var x in list)
                            {
                                string t;
                                try
                                {
                                    t = x.SelectToken("node.edge_media_to_caption.edges[0].node.text").Value<string>();
                                }
                                catch (Exception)
                                {
                                    t = "";
                                }

                                InstagramItem i = new InstagramItem()
                                {
                                    ImageLow = x.SelectToken("node.thumbnail_src").Value<string>(),
                                    ImageHigh = x.SelectToken("node.display_url").Value<string>(),
                                    Tags = t,
                                    People = p,
                                    Username = p.Username,
                                    Name = p.Name
                                };
                                items.Add(i);
                            }
                        }
                    }
                }
                return items;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void CloseUser(Pessoa _pessoa)
        {
            _pessoa.LastToken = "";
        }
    }
}
