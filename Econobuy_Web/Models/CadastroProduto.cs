using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CadastroProduto
    {
        [Display(Name = "Nome do Produto")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(255)]
        public string Nome { get; set; }
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(1000)]
        public string Descricao { get; set; }
        [Display(Name = "Código do Produto")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(20)]
        public string Codigo_Mercado { get; set; }
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public decimal Valor { get; set; }
        [Display(Name = "Modo Tradicional")]
        public bool Tradicional { get; set; }
        [Display(Name = "Departamento")]
        [MaxLength(255)]
        public string Cat01 { get; set; }
        [Display(Name = "Categoria")]
        [MaxLength(255)]
        public string Cat02 { get; set; }
        [Display(Name = "Produto")]
        [MaxLength(255)]
        public string Cat03 { get; set; }
        public int Cat01ID { get; set; }
        public int Cat02ID { get; set; }
        public int Cat03ID { get; set; }
        public int ProdID { get; set; }
        public byte[] Imagem { get; set; }

        public int Qtde { get; set; }
    }
}