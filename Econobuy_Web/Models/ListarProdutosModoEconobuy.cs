using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class ListarProdutosModoEconobuy
    {
        public int Cat03ID { get; set; }
        [Display(Name = "Produtos")]
        public string Cat03 { get; set; }
    }
}