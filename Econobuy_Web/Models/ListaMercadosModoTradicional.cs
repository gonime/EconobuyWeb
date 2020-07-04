using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class ListaMercadosModoTradicional
    {

        public string Mercado { get; set; }
        [Display(Name = "Avaliação")]
        public decimal Avaliacao { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        [Display(Name = "Número")]
        public string Numero { get; set; }
        [Display(Name = "Telefone 1")]
        public string Telefone_1 { get; set; }
        [Display(Name = "Telefone 2")]
        public string Telefone_2 { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public int MerID { get; set; }
    }
}