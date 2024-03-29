﻿using Newtonsoft.Json.Linq;
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


        public JObject ReportToJObject(Report p)
        {
            JObject jo = new JObject();
            jo.Add("id", p.Id);
            jo.Add("Name", p.Name);
            jo.Add("Cpf", p.Cpf);
            jo.Add("Tatuador", p.Tatuador);
            jo.Add("Motivo", p.Motivo);
            jo.Add("Titulo", p.Titulo);
            return jo;
        }


        public JObject PeopleToJObject(Pessoa p)
        {
            JObject jo = new JObject();
            jo.Add("id", p.Id);
            jo.Add("Email", p.Email);
            jo.Add("Username", p.Username);
            jo.Add("Facebook", p.Facebook);
            jo.Add("Numero", p.Numero);
            jo.Add("Sobre", p.Sobre);
            jo.Add("Local", p.Local);
            jo.Add("Cidade", p.Cidade);
            jo.Add("Estado", p.Estado);
            return jo;
        }

        public async Task<List<Solicitacao>> GetAllSolicitacao()
        {
            var Service = new Services.AzureService<Solicitacao>();
            var Items = await Service.GetTable();

            return Items.ToList();
        }

        public bool AddPessoa(Pessoa p)
        {
            var Service = new Services.AzureService<Pessoa>();
            if (string.IsNullOrEmpty(p.Id))
                return Service.AddItem(p);
            else
                Service.UpdateItemAsync(p.Id, PeopleToJObject(p));

            return true;

        }

        public bool AddReport(Report r)
        {
            var Service = new Services.AzureService<Report>();
            if (string.IsNullOrEmpty(r.Id))
                return Service.AddItem(r);
            else
                Service.UpdateItemAsync(r.Id, ReportToJObject(r));

            return true;

        }

        public bool AddPessoas(List<Pessoa> pessoas)
        {
            var Service = new Services.AzureService<Pessoa>();

            try
            {
                foreach (var p in pessoas)
                {
                    if (string.IsNullOrEmpty(p.Id))
                        Service.AddItem(p);
                    else
                        Service.UpdateItemAsync(p.Id, PeopleToJObject(p));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async void RemovePessoasAsync(List<Pessoa> rollback)
        {
            var Service = new Services.AzureService<Pessoa>();

            try
            {
                foreach (var p in rollback)
                {
                    if (!string.IsNullOrEmpty(p.Id))
                        await Service.DeleteItemAsync(p, PeopleToJObject(p));
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
