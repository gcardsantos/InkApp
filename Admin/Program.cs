using InkApp;
using InkApp.Models;
using InkApp.Services;
using System;
using System.Collections.Generic;

namespace Admin
{
    class Program
    {
        static List<Pessoa> pessoas;


        static void ShowPeople(Pessoa p)
        {
            Console.WriteLine("Nome: " + p.Name);
            Console.WriteLine("Numero: " + p.Numero);
            Console.WriteLine("Cidade: " + p.Cidade);
            Console.WriteLine("Estado: " + p.Estado);
            Console.WriteLine("Username: " + p.Username);
            Console.WriteLine("Facebook: " + p.Facebook);
            Console.WriteLine("Local: " + p.Local);
            Console.WriteLine("Sobre: " + p.Sobre);
        }

        static Pessoa CreatePessoa()
        {
            Pessoa p = new Pessoa();
            Console.WriteLine("Nome:");
            p.Name = Console.ReadLine();

            Console.WriteLine("Numero:");
            p.Numero = Console.ReadLine();

            Console.WriteLine("Cidade:");
            p.Cidade = Console.ReadLine();

            Console.WriteLine("Estado:");
            p.Estado = Console.ReadLine();

            Console.WriteLine("Username:");
            p.Username = Console.ReadLine();

            Console.WriteLine("Facebook:");
            p.Facebook = Console.ReadLine();

            Console.WriteLine("Local:");
            p.Local = Console.ReadLine();

            Console.WriteLine("Sobre:");
            p.Sobre = Console.ReadLine();

            return p;
        }
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Repository repository = new Repository();

            int readKey = 1;

            while(readKey != 0)
            {
                Console.WriteLine(" -------- Tatuadores");
                Console.WriteLine("1 - Adicionar");
                Console.WriteLine("2 - Editar");
                Console.WriteLine("3 - Excluir");
                Console.WriteLine("0 - Sair");

                readKey = Convert.ToInt32(Console.ReadLine());

                switch (readKey)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine(" ---- Adicionar");

                        int close = 0;
                        
                        while (close != 1 && close != 0)
                        {
                            Pessoa p = CreatePessoa();

                            ShowPeople(p);

                            Console.WriteLine(" Aperte 1 para Salvar" + Environment.NewLine + "Aperte 0 para sair");
                            close = Convert.ToInt32(Console.ReadLine());

                            if (close == 1)
                            {
                                repository.AddPessoa(p);
                            }
                        }                      
                        
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine(" ---- Editar");
                        pessoas = await repository.GetPessoas();

                        int i = 1;
                        foreach(Pessoa people in pessoas)
                        {
                            Console.WriteLine("Tatuador " + i);
                            ShowPeople(people);
                            i++;
                        }

                        Console.WriteLine("Selecione o tatuador (0 para sair): ");
                        int ind = Convert.ToInt32(Console.ReadLine());

                        if(ind != 0)
                        {
                            Pessoa p = pessoas[ind];
                            CreatePessoa();
                        }

                        break;
                    case 3:

                        Console.Clear();
                        Console.WriteLine(" ---- Excluir");
                        pessoas = await repository.GetPessoas();

                        i = 1;
                        foreach (Pessoa people in pessoas)
                        {
                            Console.WriteLine("Tatuador " + i);
                            ShowPeople(people);
                            i++;
                        }


                        break;
                }
            }
            Console.WriteLine("Saindo...");
            Console.Read();
        }
    }
}
