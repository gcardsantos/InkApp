using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkApp.Models
{
    public class Repository
    {
        public async Task<List<Pessoa>> GetPessoas(string city, string estado)
        {
            var Service = new Services.AzureService<Pessoa>();
            var Items = await Service.GetTable();

            return Items.Where(n => n.Estado.Equals(estado) && n.Cidade.Equals(city)).OrderBy(n => Guid.NewGuid()).ToList();
        }

        public async Task<Pessoa> GetPessoa(string nome)
        {
            var Service = new Services.AzureService<Pessoa>();
            var Items = await Service.GetTable();
            return Items.First(n => n.Name.Equals(nome));
        }

        public async Task<List<Pessoa>> GetPessoas()
        {
            var Service = new Services.AzureService<Pessoa>();
            var Items = await Service.GetTable();

            return Items.ToList();
        }

        public async Task<List<Pessoa>> GetShufflePessoas()
        {
            var Service = new Services.AzureService<Pessoa>();
            var Items = await Service.GetTable();

            return Items.OrderBy(n => Guid.NewGuid()).ToList();
        }

        public async Task<int> GetQuantPessoas()
        {
            var Service = new Services.AzureService<Pessoa>();
            var Items = await Service.GetTable();

            return Items.Count();
        }

        public bool Request(Solicitacao solicitacao)
        {
            var Service = new Services.AzureService<Solicitacao>();
            return Service.AddItem(solicitacao);
        }

        public bool AddPessoa(Pessoa p)
        {
            var Service = new Services.AzureService<Pessoa>();
            if (string.IsNullOrEmpty(p.Id))
                return Service.AddItem(p);
            else
                Service.UpdateItemAsync(p.Id, p);

            return true;

        }
    }
}
