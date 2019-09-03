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
    public partial class frmAdicionar : Form
    {
        static string id;
        public frmAdicionar()
        {
            InitializeComponent();
        }

        public frmAdicionar(Pessoa p)
        {
            InitializeComponent();
            txbId.Text = p.Id;
            txbNome.Text = p.Name;
            txbInsta.Text = p.Username;
            txbFace.Text = p.Facebook;
            txbWhats.Text = p.Numero;
            txbSobre.Text = p.Sobre;
            txbRua.Text = p.Local;
            txbCidade.Text = p.Cidade;
            txbEstado.Text = p.Estado;

        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Pessoa p = new Pessoa();
            p.Id = txbId.Text;
            p.Name = txbNome.Text;
            p.Username = txbInsta.Text;
            p.Facebook = txbFace.Text;
            p.Numero = txbWhats.Text;
            p.Sobre = txbSobre.Text;
            p.Local = txbRua.Text;
            p.Cidade = txbCidade.Text;
            p.Estado = txbEstado.Text;

            if (MessageBox.Show
                (p.Name + Environment.NewLine 
                + "Nome: " + p.Username + Environment.NewLine
                + "Face: " + p.Facebook + Environment.NewLine
                + "Whats: " + p.Numero + Environment.NewLine
                + "Rua: " + p.Local + Environment.NewLine
                + "Cidade: " + p.Cidade + Environment.NewLine
                + "Estado: " + p.Estado + Environment.NewLine
                + "Sobre: " + p.Sobre + Environment.NewLine, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Repository r = new Repository();
                _ = r.AddPessoa(p);
            }
            
        }
    }
}
