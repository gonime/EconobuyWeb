using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class FinalizaPedidoTrad
    {
        public string Mercado { get; set; }
        public decimal AvMer { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public decimal Valor { get; set; }
        public string Email { get; set; }
        public string Telefone_1 { get; set; }
        public string Telefone_2 { get; set; }
        public int MerID { get; set; }

        public List<CarrinhoTrad> Carrinho { get; set; }
    }
}