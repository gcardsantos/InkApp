using System;
using Microsoft.WindowsAzure.MobileServices;

namespace InkApp
{
    [DataTable("Pessoas")]
    public class Pessoa
    {
        [Version]
        public string AzureVersion { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Local { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
        public string Numero { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Sobre { get; set; }
        public string Facebook { get; set; }
        public string IdInsta { get; set; }
        public string LastToken { get; set; }
        public string Estilos { get; set; }
        public bool NextPage { get; set; }
        public int QtdPosts { get; set; }
    }
}
