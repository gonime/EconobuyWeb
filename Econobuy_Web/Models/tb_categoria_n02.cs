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
    
    public partial class tb_categoria_n02
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_categoria_n02()
        {
            this.tb_categoria_n03 = new HashSet<tb_categoria_n03>();
            this.tb_produto = new HashSet<tb_produto>();
        }
    
        public int cat02_in_codigo { get; set; }
        public string cat02_st_nome { get; set; }
        public int cat01_in_codigo { get; set; }
    
        public virtual tb_categoria_n01 tb_categoria_n01 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_categoria_n03> tb_categoria_n03 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_produto> tb_produto { get; set; }
    }
}