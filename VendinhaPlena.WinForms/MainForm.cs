using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using VendinhaPlena.Application.Services;
using VendinhaPlena.Domain.Entities;
using VendinhaPlena.Infrastructure.Data;

namespace VendinhaPlena.WinForms
{
    public class MainForm : Form
    {
        private readonly ClienteService _clienteService;
        private readonly VendinhaDbContext _context;

        // Componentes
        private DataGridView gridClientes = new DataGridView();
        private TextBox txtNome = new TextBox(), txtCpf = new TextBox(), txtEmail = new TextBox();
        private DateTimePicker dtNascimento = new DateTimePicker();
        private TextBox txtBusca = new TextBox();
        private ComboBox cbClientesDivida = new ComboBox();
        private NumericUpDown numValorDivida = new NumericUpDown();
        private DataGridView gridDividas = new DataGridView();

        public MainForm()
        {
            var optionsBuilder = new DbContextOptionsBuilder<VendinhaDbContext>();
            
            
            optionsBuilder.UseSqlite("Data Source=vendinha.db");
            _context = new VendinhaDbContext(optionsBuilder.Options);

            
            _context.Database.EnsureCreated();

            _clienteService = new ClienteService(_context);

            ConfigurarJanela();
            InicializarComponentes();
            CarregarDadosComponentes();
        }
        private void ConfigurarJanela()
        {
            this.Text = "Sistema Vendinha Plena - Controle de Fiado";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);
        }

        private void InicializarComponentes()
        {
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };
            TabPage tabClientes = new TabPage("Gerenciar Clientes");
            TabPage tabDividas = new TabPage("Gerenciar Dívidas/Caixa");

            // --- ABA CLIENTES ---
            Panel panelCadastroCliente = new Panel { Dock = DockStyle.Left, Width = 280, Padding = new Padding(10) };
            
            panelCadastroCliente.Controls.Add(new Label { Text = "Nome Completo:", Location = new Point(10, 15), Width = 200 });
            txtNome.Location = new Point(10, 35); txtNome.Width = 240;
            panelCadastroCliente.Controls.Add(txtNome);

            panelCadastroCliente.Controls.Add(new Label { Text = "CPF (Apenas números):", Location = new Point(10, 70), Width = 200 });
            txtCpf.Location = new Point(10, 90); txtCpf.Width = 240;
            panelCadastroCliente.Controls.Add(txtCpf);

            panelCadastroCliente.Controls.Add(new Label { Text = "Data de Nascimento:", Location = new Point(10, 125), Width = 200 });
            dtNascimento.Location = new Point(10, 145); dtNascimento.Width = 240; dtNascimento.Format = DateTimePickerFormat.Short;
            panelCadastroCliente.Controls.Add(dtNascimento);

            panelCadastroCliente.Controls.Add(new Label { Text = "E-mail:", Location = new Point(10, 180), Width = 200 });
            txtEmail.Location = new Point(10, 200); txtEmail.Width = 240;
            panelCadastroCliente.Controls.Add(txtEmail);

            Button btnSalvarCliente = new Button { Text = "Salvar Cliente", Location = new Point(10, 245), Width = 240, Height = 35, BackColor = Color.LightGreen };
            btnSalvarCliente.Click += BtnSalvarCliente_Click;
            panelCadastroCliente.Controls.Add(btnSalvarCliente);

            Panel panelGridCliente = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            txtBusca.Location = new Point(10, 15); txtBusca.Width = 350; txtBusca.PlaceholderText = " Digite o nome para pesquisar...";
            txtBusca.TextChanged += (s, e) => CarregarGridClientes();
            panelGridCliente.Controls.Add(txtBusca);

            gridClientes.Location = new Point(10, 50); gridClientes.Size = new Size(560, 450); gridClientes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; gridClientes.AutoGenerateColumns = true; gridClientes.ReadOnly = true; gridClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            panelGridCliente.Controls.Add(gridClientes);

            tabClientes.Controls.Add(panelGridCliente);
            tabClientes.Controls.Add(panelCadastroCliente);

            // --- ABA DÍVIDAS ---
            Panel panelCadastroDivida = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(10) };
            
            panelCadastroDivida.Controls.Add(new Label { Text = "Selecione o Cliente:", Location = new Point(15, 15), Width = 150 });
            cbClientesDivida.Location = new Point(15, 35); cbClientesDivida.Width = 250; cbClientesDivida.DropDownStyle = ComboBoxStyle.DropDownList;
            cbClientesDivida.SelectedIndexChanged += (s, e) => CarregarGridDividas();
            panelCadastroDivida.Controls.Add(cbClientesDivida);

            panelCadastroDivida.Controls.Add(new Label { Text = "Valor da Dívida (R$):", Location = new Point(290, 15), Width = 150 });
            numValorDivida.Location = new Point(290, 35); numValorDivida.Width = 120; numValorDivida.Maximum = 10000; numValorDivida.DecimalPlaces = 2;
            panelCadastroDivida.Controls.Add(numValorDivida);

            Button btnLancarDivida = new Button { Text = "Pendurar Dívida", Location = new Point(430, 30), Width = 140, Height = 30, BackColor = Color.LightCoral };
            btnLancarDivida.Click += BtnLancarDivida_Click;
            panelCadastroDivida.Controls.Add(btnLancarDivida);

            Button btnPagar = new Button { Text = "Dar Baixa / Pagar", Location = new Point(580, 30), Width = 140, Height = 30, BackColor = Color.LightSkyBlue };
            btnPagar.Click += BtnPagar_Click;
            panelCadastroDivida.Controls.Add(btnPagar);

            gridDividas.Dock = DockStyle.Fill; gridDividas.AutoGenerateColumns = true; gridDividas.ReadOnly = true; gridDividas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            tabDividas.Controls.Add(gridDividas);
            tabDividas.Controls.Add(panelCadastroDivida);

            tabControl.Controls.Add(tabClientes);
            tabControl.Controls.Add(tabDividas);
            this.Controls.Add(tabControl);
        }

        private async void CarregarGridClientes()
        {
            try
            {
                var dados = await _clienteService.ObterClientesPaginadosAsync(txtBusca.Text, 1);
                gridClientes.DataSource = dados;
            }
            catch (Exception ex) { MessageBox.Show($"Erro ao carregar clientes: {ex.Message}"); }
        }

        private async void CarregarGridDividas()
        {
            if (cbClientesDivida.SelectedValue is int clienteId)
            {
                var dividas = await _clienteService.ObterDividasPorClienteAsync(clienteId);
                gridDividas.DataSource = dividas.Select(d => new {
                    Código = d.Id,
                    Valor = d.Valor.ToString("C"),
                    Situação = d.EstaPaga ? "PAGA" : "EM ABERTO",
                    Criação = d.DataCriacao.ToLocalTime(),
                    Pagamento = d.DataPagamento.HasValue ? d.DataPagamento.Value.ToLocalTime().ToString() : "-"
                }).ToList();
            }
        }

        private void CarregarDadosComponentes()
        {
            CarregarGridClientes();
            
            var listaClientes = _context.Clientes.Select(c => new { c.Id, c.Nome }).ToList();
            cbClientesDivida.DataSource = listaClientes;
            cbClientesDivida.DisplayMember = "Nome";
            cbClientesDivida.ValueMember = "Id";
        }

        // CORREÇÃO: Adicionado 'object? sender'
        private async void BtnSalvarCliente_Click(object? sender, EventArgs e)
        {
            try
            {
                var novoCliente = new Cliente
                {
                    Nome = txtNome.Text,
                    Cpf = txtCpf.Text,
                    DataNascimento = dtNascimento.Value,
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text
                };

                await _clienteService.CriarClienteAsync(novoCliente);
                MessageBox.Show("Cliente cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                txtNome.Clear(); txtCpf.Clear(); txtEmail.Clear();
                CarregarDadosComponentes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validação Falhou", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // CORREÇÃO: Adicionado 'object? sender'
        private async void BtnLancarDivida_Click(object? sender, EventArgs e)
        {
            if (cbClientesDivida.SelectedValue is int clienteId)
            {
                try
                {
                    await _clienteService.AdicionarDividaAsync(clienteId, numValorDivida.Value);
                    MessageBox.Show("Conta pendurada com sucesso!", "Caixa Atualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    numValorDivida.Value = 0;
                    CarregarGridDividas();
                    CarregarGridClientes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Regra de Negócio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // CORREÇÃO: Adicionado 'object? sender' e proteção contra células Nulas
        private async void BtnPagar_Click(object? sender, EventArgs e)
        {
            if (gridDividas.CurrentRow != null)
            {
                try
                {
                    var valorCodigo = gridDividas.CurrentRow.Cells["Código"].Value;
                    var valorSituacao = gridDividas.CurrentRow.Cells["Situação"].Value;

                    if (valorCodigo == null || valorSituacao == null) return;

                    // Conversão segura evitando unboxing nulo
                    int codigoId = Convert.ToInt32(valorCodigo);
                    string situacao = valorSituacao.ToString() ?? "";

                    if (situacao == "PAGA")
                    {
                        MessageBox.Show("Esta dívida já foi paga anteriormente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    await _clienteService.MarcarDividaComoPagaAsync(codigoId);
                    MessageBox.Show("Dívida liquidada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarGridDividas();
                    CarregarGridClientes();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}