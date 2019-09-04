using InkApp;
using InkApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Administrador
{
    public partial class frmPrincipal : Form
    {
        static List<Pessoa> Pessoas;
        static Repository repository;
        public frmPrincipal()
        {
            InitializeComponent();
            StartValueAsync();
        }

        private async void StartValueAsync()
        {
            Pessoas = new List<Pessoa>();
            repository = new Repository();
            await Task.WhenAll(GetPeopleAsync());
            Pessoas.ForEach(n => lbPessoas.Items.Add(n.Name + "       | " + n.Cidade + "      | " + n.Estado));
        }

        static public async Task GetPeopleAsync() => Pessoas = await repository.GetPessoas();

        private void BtnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            frmAdicionar frm = new frmAdicionar();
            frm.Visible = true;
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {

        }
    }
}
