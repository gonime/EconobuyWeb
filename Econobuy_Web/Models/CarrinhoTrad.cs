using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CarrinhoTrad
    {
        [Key]
        public int Id { get; set; }
        public int ProdID { get; set; }
        public string Nome { get; set; }
        [Required(ErrorMessage = "Adicione pelo menos uma unidade")]
        [Display(Name = "Quantidade")]
        public int Qtde { get; set; }
        [Display(Name = "Valor Unidade")]
        public decimal Valor { get; set; }
        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }
        [Display(Name = "Departamento")]
        public string Cat01 { get; set; }
        [Display(Name = "Categoria")]
        public string Cat02 { get; set; }
        [Display(Name = "Produto")]
        public string Cat03 { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }
}