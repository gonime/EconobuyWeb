using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Econobuy_Web.Models
{
    public class ConsultaProdutos
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        public bool Tradicional { get; set; }
        public bool Ativo { get; set; }
        [Display(Name = "Departamento")]
        public string Categoria_01 { get; set; }
        [Display(Name = "Categoria Primária")]
        public string Categoria_02 { get; set; }
        [Display(Name = "Categoria Secundária")]
        public string Categoria_03 { get; set; }
    }
}