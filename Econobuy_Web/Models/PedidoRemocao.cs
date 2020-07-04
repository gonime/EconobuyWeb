using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class PedidoRemocao
    {
        public int AvaliacaoID { get; set; }
        [MaxLength(300)]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string MsgRequisitante { get; set; }
        [MaxLength(300)]
        public string MsgAdmin { get; set; }
        public string Status { get; set; }
        public decimal NotaAvaliacao { get; set; }
        public string DescricaoAvaliacao { get; set; }
    }
}