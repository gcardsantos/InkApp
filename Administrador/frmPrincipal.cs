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
        static List<Pessoa> Rollback;
        static List<Solicitacao> Solicitacaos;
        static Repository repository;

        public frmPrincipal()
        {
            InitializeComponent();
            StartValueAsync();
        }

        private async void StartValueAsync()
        {
            Pessoas = new List<Pessoa>();
            Rollback = new List<Pessoa>();
            Solicitacaos = new List<Solicitacao>();

            repository = new Repository();

            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("Nome", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Instagram", typeof(string));
            table.Columns.Add("WhatsApp", typeof(string));
            table.Columns.Add("Facebook", typeof(string));
            table.Columns.Add("Rua", typeof(string));
            table.Columns.Add("Cidade", typeof(string));
            table.Columns.Add("Estado", typeof(string));
            table.Columns.Add("Sobre", typeof(string));
            table.Columns.Add("Criado em", typeof(string));
            table.Columns.Add("Editado em", typeof(string));
            table.Columns.Add("         ", typeof(string));

            this.Cursor = Cursors.WaitCursor;
            await Task.WhenAll(GetPeopleAsync(), GetSolicitaoAsync());
            this.Cursor = Cursors.Default;

            foreach (Pessoa p in Pessoas)
                table.Rows.Add(p.Id, p.Name, p.Email, p.Username, p.Numero, p.Facebook, p.Local, p.Cidade, p.Estado, p.Sobre, p.createdAt.ToString(), p.updatedAt.ToString());

            dgvPessoas.DataSource = table;

            table = new DataTable();

            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("Nome", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Criado em", typeof(string));
            table.Columns.Add("Editado em", typeof(string));

            foreach (Solicitacao s in Solicitacaos)
                table.Rows.Add(s.Id, s.Nome, s.Email, s.createdAt.ToString(), s.updatedAt.ToString());

            dgvSolicitacao.DataSource = table;
        }

        static public async Task GetSolicitaoAsync() => Solicitacaos = await repository.GetAllSolicitacao();


        static public async Task GetPeopleAsync() => Pessoas = await repository.GetPessoas();

        private void BtnBuscar_Click(object sender, EventArgs e)
        {

        }

        public Pessoa RowToPeople(DataRow row)
        {
            return (new Pessoa()
            {
                Id = row.ItemArray[0] as string,
                Name = row.ItemArray[1] as string,
                Email = row.ItemArray[2] as string,
                Username = row.ItemArray[3] as string,
                Numero = row.ItemArray[4] as string,
                Facebook = row.ItemArray[5] as string,
                Local = row.ItemArray[6] as string,
                Cidade = row.ItemArray[7] as string,
                Estado = row.ItemArray[8] as string,
                Sobre = row.ItemArray[9] as string
            });
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            DataTable table = dgvPessoas.DataSource as DataTable;

            List<Pessoa> pessoas = new List<Pessoa>();

            foreach (var row in table.Rows)
            {
                pessoas.Add(RowToPeople(row as DataRow));
            }


            if (MessageBox.Show("Deseja salvar todas alterações ? ", "Excluir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Concluir(pessoas);
        }

        private void Concluir(List<Pessoa> pessoas)
        {
            repository.AddPessoas(pessoas);
            repository.RemovePessoasAsync(Rollback);
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvPessoas.CurrentRow.Selected)
            {
                if(MessageBox.Show("Deseja remover ? ", "Excluir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataRow row = ((DataRowView)dgvPessoas.SelectedRows[0].DataBoundItem).Row;
                    Rollback.Add(RowToPeople(row));
                    dgvPessoas.Rows.Remove(dgvPessoas.CurrentRow);
                }
                    
            }
        }

        private void BtnAtt_Click(object sender, EventArgs e)
        {
            DataTable table = dgvPessoas.DataSource as DataTable;

            if(table != null)
                table.Clear();

            StartValueAsync();
        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
