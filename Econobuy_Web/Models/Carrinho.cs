using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class Carrinho
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Qtde { get; set; }
        [Display(Name = "Valor Unidade")]
        public decimal Valor { get; set; }
        public int Cat03ID { get; set; }

        public int ProdID { get; set; }
    }
}