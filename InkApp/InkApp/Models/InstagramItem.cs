using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace InkApp.Models
{
    public class InstagramItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        public string ImageLow { get; set; }
        public string ImageHigh { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        [Ignore]
        public Pessoa People { get; set; }
        
    }
}
