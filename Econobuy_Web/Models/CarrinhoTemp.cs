using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CarrinhoTemp
    {
        const string ItensID = "Itens";

        internal static void ArmazenaItens(Carrinho item)
        {
            List<Carrinho> pedidos = RetornaItens() != null ?
                RetornaItens() : new List<Carrinho>();

            pedidos.Add(item);
            HttpContext.Current.Session[ItensID] = pedidos;
        }

        internal static void RemoveItem(int id)
        {
            List<Carrinho> pedidos = RetornaItens();
            pedidos.RemoveAt(id);
            HttpContext.Current.Session[ItensID] = pedidos;
        }


        internal static List<Carrinho> RetornaItens()
        {
            return HttpContext.Current.Session[ItensID] != null ?
                (List<Carrinho>)HttpContext.Current.Session[ItensID] : null;
        }
    }
}