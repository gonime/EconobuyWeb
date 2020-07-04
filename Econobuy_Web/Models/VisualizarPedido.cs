using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class VisualizarPedido
    {
        public string Mercado_Ou_Cliente { get; set; }
        [Display(Name = "Data")]
        public DateTime Data { get; set; }
        [Display(Name = "CEP")]
        public string CEP { get; set; }
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }
        [Display(Name = "Número")]
        public string Numero { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Telefone 1")]
        public string Telefone_1 { get; set; }
        [Display(Name = "Telefone 2")]
        public string Telefone_2 { get; set; }
        [Display(Name = "Status do Pedido")]
        public string Status { get; set; }
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        public string Msg { get; set; }
        public int PedID { get; set; }

        public decimal AvMercado { get; set; }
        public string AvMer { get; set; }
        public decimal AvCliente { get; set; }
        public string AvCli { get; set; }
        public bool AvActive { get; set; }

        public int PedRemID { get; set; }

        public List<VisualizarItens> Itens { get; set; }
    }
}