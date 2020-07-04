//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econobuy_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_pedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_pedido()
        {
            this.tb_item = new HashSet<tb_item>();
            this.tb_pedido_avaliacao_cliente = new HashSet<tb_pedido_avaliacao_cliente>();
            this.tb_pedido_avaliacao_mercado = new HashSet<tb_pedido_avaliacao_mercado>();
        }
    
        public int ped_in_codigo { get; set; }
        public int cli_in_codigo { get; set; }
        public int mer_in_codigo { get; set; }
        public System.DateTime data_dt_pedido { get; set; }
        public int end_in_codigo { get; set; }
        public decimal ped_dec_valor { get; set; }
        public string ped_status { get; set; }
        public string ped_st_msg { get; set; }
    
        public virtual tb_cliente tb_cliente { get; set; }
        public virtual tb_endereco tb_endereco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_item> tb_item { get; set; }
        public virtual tb_mercado tb_mercado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_pedido_avaliacao_cliente> tb_pedido_avaliacao_cliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_pedido_avaliacao_mercado> tb_pedido_avaliacao_mercado { get; set; }
    }
}