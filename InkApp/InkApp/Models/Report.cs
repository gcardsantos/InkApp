using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace InkApp.Models
{
    [DataTable("Report")]
    public class Report
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Tatuador { get; set; }
        public string Titulo { get; set; }
        public string Motivo { get; set; }
    }
}
