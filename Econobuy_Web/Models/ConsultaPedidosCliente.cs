using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class ConsultaPedidosCliente
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime Data { get; set; }
        public string Mercado { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
    }
}