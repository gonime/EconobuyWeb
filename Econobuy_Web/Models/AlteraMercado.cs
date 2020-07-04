using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class AlteraMercado
    {
        [Display(Name = "Usuário")]
        [Required(ErrorMessage = "Este campo não pode ser deixado em branco")]
        [MaxLength(30)]
        public string User { get; set; }
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Este campo não pode ser deixado em branco")]
        [MaxLength(30)]
        public string Senha { get; set; }
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Este campo não pode ser deixado em branco")]
        [MaxLength(100)]
        public string Email { get; set; }
        [Display(Name = "Telefone 1")]
        [Required(ErrorMessage = "Este campo não pode ser deixado em branco")]
        [MaxLength(15)]
        public string Telefone_1 { get; set; }
        [Display(Name = "Telefone 2")]
        [Required(ErrorMessage = "Este campo não pode ser deixado em branco")]
        [MaxLength(15)]
        public string Telefone_2 { get; set; }
        [Display(Name = "Logo")]
        public byte[] imgMercado { get; set; }
        public int MerID { get; set; }
        public int EndID { get; set; }
        public int ImgID { get; set; }
    }
}