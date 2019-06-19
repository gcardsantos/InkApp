﻿using InkApp.Models;
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
        private string last_token = "";
        private bool has_next_page = true;
        public InstagramParser()
        {
            
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
            try
            {
                while (items.Count < (quant * 2) && has_next_page)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string s = @"https://www.instagram.com/graphql/query/?query_hash=472f257a40c653c64c666ce877d59d2b&variables={";

                        string ss = "\"id\"" + ":\"" + p.IdInsta + "\",\"first\":" + "50" + ", \"after\":\"" + last_token + "\"}";
                        var response = await client.GetAsync(s + ss);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            JObject o = JObject.Parse(json);

                            has_next_page = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.has_next_page").Value<bool>();
                            last_token = o.SelectToken("data.user.edge_owner_to_timeline_media.page_info.end_cursor").Value<string>();
                            var l = o.SelectToken("data.user.edge_owner_to_timeline_media.edges").Value<IEnumerable<JToken>>();
                            var list = new List<JToken>(l).FindAll(n => n.ToString().Contains("GraphImage"));
                            list = list.Count > quant ? list.GetRange(0, quant) : list;
                            list.ForEach(n => items.Add(new InstagramItem() { ImageLow =  n.SelectToken("node.thumbnail_src").Value<string>(), ImageHigh = n.SelectToken("node.display_url").Value<string>() }));
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

        public void CloseUser()
        {
            last_token = "";
        }
    }
}
