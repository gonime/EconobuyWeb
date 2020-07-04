using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CadastroCliente
    {
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(255)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Usuário")]
        [MaxLength(20)]
        public string User { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [MinLength(8)]
        [MaxLength(20)]
        public string Senha { get; set; }
        [Display(Name = "CPF")]
        [MaxLength(14)]

        public string CPF { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "E-mail")]
        [MaxLength(100)]
        public string email { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "CEP")]
        [MaxLength(10)]
        public string CEP { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Logradouro")]
        [MaxLength(150)]
        public string Logradouro { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Número")]
        [MaxLength(10)]
        public string Numero { get; set; }
        [Display(Name = "Complemento")]
        [MaxLength(150)]
        public string Complemento { get; set; }
        [Display(Name = "Bairro")]
        [MaxLength(50)]
        public string Bairro { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Cidade")]
        [MaxLength(50)]
        public string Cidade { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "UF")]
        [MaxLength(3)]
        public string UF { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Telefone 1")]
        [MaxLength(15)]
        public string Telefone_1 { get; set; }
        [Display(Name = "Telefone 2")]
        [MaxLength(15)]
        public string Telefone_2 { get; set; }

        public int EndID { get; set; }
        public int CliID { get; set; }
    }
}