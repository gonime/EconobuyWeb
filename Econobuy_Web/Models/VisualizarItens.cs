using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class VisualizarItens
    {
        [Display(Name = "Produto")]
        public string Nome { get; set; }
        [Display(Name = "Valor Unidade")]
        public decimal valor_un { get; set; }
        [Display(Name = "Quantidade")]
        public int Qtde { get; set; }
        [Display(Name = "Valor Total")]
        public decimal valor_total { get; set; }
        [Display(Name = "Código")]
        public string codigo { get; set; }
        public int ProdID { get; set; }
    }
}