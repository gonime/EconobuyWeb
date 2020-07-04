using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CarrinhoTempFinal
    {
        const string ItensID = "ItensFinal";

        internal static void ArmazenaItens(CarrinhoTrad item)
        {
            List<CarrinhoTrad> pedidos = RetornaItens() != null ?
                RetornaItens() : new List<CarrinhoTrad>();

            pedidos.Add(item);
            HttpContext.Current.Session[ItensID] = pedidos;
        }

        internal static void RemoveItem(int id)
        {
            List<CarrinhoTrad> pedidos = RetornaItens();
            pedidos.RemoveAt(id);
            HttpContext.Current.Session[ItensID] = pedidos;
        }


        internal static List<CarrinhoTrad> RetornaItens()
        {
            return HttpContext.Current.Session[ItensID] != null ?
                (List<CarrinhoTrad>)HttpContext.Current.Session[ItensID] : null;
        }
    }
}