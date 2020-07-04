using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class ConsultaPedidosMercado
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime Data { get; set; }
        public string Cliente { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
    }
}