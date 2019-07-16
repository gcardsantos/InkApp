using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace InkApp.Models
{
    [DataTable("Solicitacoes")]
    public class Solicitacao
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Facebook { get; set; }
        public string Telefone { get; set; }
        public string Estilos { get; set; }
        public string Email { get; set; }
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Sobre { get; set; }
    }
}
