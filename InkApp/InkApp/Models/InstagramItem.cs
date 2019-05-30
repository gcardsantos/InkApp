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
        public Pessoa People { get; set; }
        public string ImageLow { get; set; }
        public string ImageHigh { get; set; }
    }
}
