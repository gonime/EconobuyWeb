using Econobuy_Web.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Econobuy_Web.Controllers
{
    public class MercadoController : Controller
    {
        // GET: Mercado
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginMercado(tb_mercado user)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var userDetail = db.tb_mercado.Where(x => x.mer_st_user == user.mer_st_user && x.mer_st_senha == user.mer_st_senha).FirstOrDefault();
                if (userDetail == null)
                {
                    TempData["Erro"] = "Usuário ou senha inválidos";
                    return View("Index", user);
                }
                else
                {
                    Session["mercadoID"] = userDetail.mer_in_codigo;
                    Session["mercadoNome"] = userDetail.mer_st_nome;
                    return RedirectToAction("Home", "Mercado");
                }
            }
        }


        public ActionResult RecuperarSenha(tb_mercado mer)
        {
            return View();
        }

        public ActionResult RecuperaSenha(tb_mercado mer)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int merID = db.tb_mercado.Where(x => x.mer_st_email == mer.mer_st_email).Select(x => x.mer_in_codigo).SingleOrDefault();
                if (merID > 0)
                {
                    Random rnd = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    string senha = new string(Enumerable.Repeat(chars, 10)
                      .Select(s => s[rnd.Next(s.Length)]).ToArray());
                    tb_mercado me = db.tb_mercado.Find(merID);
                    me.mer_st_senha = senha;
                    db.SaveChanges();
                    EnviaSenhaEmail(me.mer_st_email, me.mer_st_user, senha);
                    TempData["Query"] = "Seus dados de acesso foram enviados para seu e-mail";
                    return View("RecuperarSenha", mer);
                }
                else
                {
                    TempData["Erro"] = "E-mail não encontrado no sistema";
                    return View("RecuperarSenha", mer);
                }
            }
        }
        public void EnviaSenhaEmail(string email, string usuario, string senha)
        {
            try
            {
                string msg = "Seguem os dados de acesso solicitados: \n\n Usuario: " + usuario + " \n Senha: " + senha +
                    " \n\n Caso não tenha feito esta solicitação entre em contato conosco respondendo este e-mail. \n\n Atenciosamente, \n Equipe Econobuy.";
                MailMessage mensagemEmail = new MailMessage("sistemaeconobuy@gmail.com", email, "Recuperação de Senha - Econobuy", msg);
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sistemaeconobuy@gmail.com", "Nmb159nmb!");
                client.Send(mensagemEmail);
            }
            catch (Exception ex)
            {

            }
            return;
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult MostraLogo(int Id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_mercado_img where img.mer_in_codigo == Id select img.mer_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        public ActionResult ConsultarPedidos(string coluna, string search)
        {
            int Id = Convert.ToInt32(Session["mercadoID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from ped in db.tb_pedido
                             join cli in db.tb_cliente on
    ped.cli_in_codigo equals cli.cli_in_codigo
                             join en in db.tb_endereco on ped.end_in_codigo
                             equals en.end_in_codigo
                             where ped.mer_in_codigo == Id
                             select new ConsultaPedidosMercado
                             {
                                 Id = ped.ped_in_codigo,
                                 Valor = ped.ped_dec_valor,
                                 Status = ped.ped_status,
                                 Data = ped.data_dt_pedido,
                                 Cliente = cli.cli_st_nome,
                                 CEP = en.end_st_CEP,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log
                             }
                             ).OrderBy(u => u.Status).ToList(); 
                if (!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model;
                    if (coluna == "Status") modelSearch = model.Where(x => x.Status.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Cliente") modelSearch = model.Where(x => x.Cliente.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Bairro") modelSearch = model.Where(x => x.Bairro.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Logradouro") modelSearch = model.Where(x => x.Logradouro.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                return View(model);
            }
        }

        public ActionResult VisualizarPedido(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from en in db.tb_endereco
                             join cli
   in db.tb_cliente on en.end_in_codigo
   equals cli.end_in_codigo
                             join ped
    in db.tb_pedido on cli.cli_in_codigo
    equals ped.cli_in_codigo
                             where
    ped.ped_in_codigo == id
                             select new VisualizarPedido
                             {
                                 Mercado_Ou_Cliente = cli.cli_st_nome,
                                 Data = ped.data_dt_pedido,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Numero = en.end_st_num,
                                 Email = cli.cli_st_email,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Status = ped.ped_status,
                                 Valor = ped.ped_dec_valor,
                                 PedID = ped.ped_in_codigo, 
                                 Msg = ped.ped_st_msg
                             }
                             ).First();
                var itens = (from ped in db.tb_pedido
                             join
                            itn in db.tb_item on ped.ped_in_codigo
                            equals itn.ped_in_codigo
                             join
                            prod in db.tb_produto on itn.prod_in_codigo
                            equals prod.prod_in_codigo
                             where ped.ped_in_codigo == id
                             select new VisualizarItens
                             {
                                 Nome = prod.prod_st_nome,
                                 valor_un = prod.prod_dec_valor_un,
                                 Qtde = itn.item_in_qtde,
                                 valor_total = prod.prod_dec_valor_un * itn.item_in_qtde,
                                 ProdID = prod.prod_in_codigo
                             }
                             ).OrderBy(u => u.Nome).ToList();
                decimal avcli = 0;
                avcli = db.tb_pedido_avaliacao_cliente.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_cli_dec_nota).FirstOrDefault();
                model.AvCliente = avcli;
                if (avcli != 0)
                {
                    model.AvCli = db.tb_pedido_avaliacao_cliente.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_cli_st_descricao).FirstOrDefault();
                    model.AvActive = db.tb_pedido_avaliacao_cliente.Where(x => x.ped_in_codigo == id).Select(x => x.ped_bit_active).FirstOrDefault(); 
                    var pedRemID = 0;
                    int avcliID = 0;
                    avcliID = db.tb_pedido_avaliacao_cliente.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_cli_in_codigo).FirstOrDefault();
                    if (avcliID != 0) { pedRemID = db.tb_pedido_remocao_avaliacao_mercado.Where(x => x.ped_av_cli_in_codigo == avcliID).Select(x => x.ped_rem_av_mer_in_codigo).FirstOrDefault(); }
                    model.PedRemID = Convert.ToInt32(pedRemID);
                }
                decimal avmer = 0;
                avmer = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_mer_dec_nota).FirstOrDefault();
                model.AvMercado = avmer;
                if (avmer != 0) model.AvMer = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_mer_st_descricao).FirstOrDefault();
                model.Itens = itens;
                return View(model);
            }
        }
        public ActionResult aprovaPedido(int id)
        {
            TempData["pedID"] = id;
            return View();
        }

        public ActionResult reprovaPedido(int id)
        {
            TempData["pedID"] = id;
            return View();
        }

        public ActionResult aprovarPedido(tb_pedido ped)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                tb_pedido p = db.tb_pedido.Find(Convert.ToInt32(TempData["pedID"]));
                p.ped_status = "Aprovado";
                p.ped_st_msg = ped.ped_st_msg;
                db.SaveChanges();
                return RedirectToAction("VisualizarPedido", "Mercado", new { id = Convert.ToInt32(TempData["pedID"]) });
            }
        }

        public ActionResult reprovarPedido(tb_pedido ped)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                tb_pedido p = db.tb_pedido.Find(Convert.ToInt32(TempData["pedID"]));
                p.ped_status = "Reprovado";
                p.ped_st_msg = ped.ped_st_msg;
                db.SaveChanges();
                return RedirectToAction("VisualizarPedido", "Mercado", new { id = Convert.ToInt32(TempData["pedID"]) });
            }
        }

        public ActionResult avaliaPedidoEntregue(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                TempData["pedID"] = id;
                int cliID = db.tb_pedido.Where(x => x.ped_in_codigo == id).Select(x => x.cli_in_codigo).SingleOrDefault();
                int AvCliID = db.tb_avaliacao_cliente.Where(x => x.cli_in_codigo == cliID).Select(x => x.av_cli_in_codigo).SingleOrDefault();
                TempData["AvCliID"] = AvCliID;
                return View();
            }
        }

        public ActionResult avaliarPedidoEntregue(tb_pedido_avaliacao_mercado pedav)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var av = new tb_pedido_avaliacao_mercado
                {
                    av_cli_in_codigo = Convert.ToInt32(TempData["AvCliID"]),
                    ped_av_mer_dec_nota = pedav.ped_av_mer_dec_nota,
                    ped_av_mer_st_descricao = pedav.ped_av_mer_st_descricao,
                    ped_in_codigo = Convert.ToInt32(TempData["pedID"]),
                    ped_bit_active = true
                };
                db.tb_pedido_avaliacao_mercado.Add(av);
                db.SaveChanges();
                return RedirectToAction("VisualizarPedido", "Mercado", new { Id = av.ped_in_codigo });
            }
        }

        public ActionResult pedidoRemocaoAvaliacao(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_pedido_avaliacao_cliente
                             where av.ped_in_codigo == id
                             select new PedidoRemocao
                             {
                                 AvaliacaoID = av.ped_av_cli_in_codigo,
                                 NotaAvaliacao = av.ped_av_cli_dec_nota,
                                 DescricaoAvaliacao = av.ped_av_cli_st_descricao
                             }).FirstOrDefault();

                return View(model);
            }
        }

        public ActionResult criaPedidoRemocao(PedidoRemocao ped)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var rem = new tb_pedido_remocao_avaliacao_mercado
                {
                    ped_av_cli_in_codigo = ped.AvaliacaoID,
                    ped_rem_av_mer_st_msg_mer = ped.MsgRequisitante,
                    ped_rem_av_mer_st_msg_adm = "",
                    ped_status = "Em análise"
                };
                db.tb_pedido_remocao_avaliacao_mercado.Add(rem);
                db.SaveChanges();
                return RedirectToAction("acompanharPedidoRemocao", "Mercado", new { Id = rem.ped_rem_av_mer_in_codigo });
            }
        }

        public ActionResult acompanharPedidoRemocao(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_pedido_avaliacao_cliente
                             join ped in db.tb_pedido_remocao_avaliacao_mercado on
                             av.ped_av_cli_in_codigo equals ped.ped_av_cli_in_codigo
                             where ped.ped_rem_av_mer_in_codigo == id
                             select new PedidoRemocao
                             {
                                 AvaliacaoID = av.ped_av_cli_in_codigo,
                                 NotaAvaliacao = av.ped_av_cli_dec_nota,
                                 DescricaoAvaliacao = av.ped_av_cli_st_descricao,
                                 MsgAdmin = ped.ped_rem_av_mer_st_msg_adm,
                                 MsgRequisitante = ped.ped_rem_av_mer_st_msg_mer,
                                 Status = ped.ped_status
                             }).FirstOrDefault();

                return View(model);
            }
        }

        public ActionResult AlterarUsuario()
        {
            int Id = Convert.ToInt32(Session["mercadoID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var alter = (from mer in db.tb_mercado
                             join end in db.tb_endereco on
                             mer.end_in_codigo equals end.end_in_codigo
                             join merImg
                             in db.tb_mercado_img on mer.mer_in_codigo
                             equals merImg.mer_in_codigo
                             where mer.mer_in_codigo == Id
                             select new AlteraMercado
                             {
                                 User = mer.mer_st_user,
                                 Senha = mer.mer_st_senha,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = end.end_st_tel1,
                                 Telefone_2 = end.end_st_tel2,
                                 EndID = end.end_in_codigo,
                                 MerID = mer.mer_in_codigo,
                                 ImgID = merImg.mer_img_in_codigo
                             });
                if (alter != null)
                {
                    AlteraMercado alt = alter.First();
                    return View(alt);
                }
                else return View();
            }
        }

        [HttpPost]
        public ActionResult AlteraUsuario(AlteraMercado alt, HttpPostedFileBase imgMercado)
        {
            HttpPostedFileBase file = Request.Files["img"];
            if (file.ContentLength > 0) alt.imgMercado = ConvertToBytes(file);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("AlterarUsuario", alt);
                }
                else
                {
                    tb_endereco end = db.tb_endereco.Find(alt.EndID);
                    tb_mercado mer = db.tb_mercado.Find(alt.MerID);
                    tb_mercado_img img = db.tb_mercado_img.Find(alt.ImgID);
                    if (alt != null)
                    {
                        mer.mer_st_user = alt.User;
                        mer.mer_st_senha = alt.Senha;
                        mer.mer_st_email = alt.Email;
                        end.end_st_tel1 = alt.Telefone_1;
                        end.end_st_tel2 = alt.Telefone_2;
                        if (alt.imgMercado != null) img.mer_img = alt.imgMercado;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Home", "Mercado");
                }
            }
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        public ActionResult ConsultarAvaliacoes()
        {
            return View();
        }

        public ActionResult ConsultarProdutos(string coluna, string search)
        {
            int Id = Convert.ToInt32(Session["mercadoID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto join cat01
                            in db.tb_categoria_n01 on prod.cat01_in_codigo
                            equals cat01.cat01_in_codigo join cat02 in
                            db.tb_categoria_n02 on prod.cat02_in_codigo
                            equals cat02.cat02_in_codigo join cat03 in
                            db.tb_categoria_n03 on prod.cat03_in_codigo
                            equals cat03.cat03_in_codigo where
                            prod.mer_in_codigo == Id
                             select new ConsultaProdutos
                             {
                                 Id = prod.prod_in_codigo,
                                 Nome = prod.prod_st_nome,
                                 Preco = prod.prod_dec_valor_un,
                                 Codigo = prod.prod_st_cod_mer,
                                 Tradicional = prod.prod_bit_trad_active,
                                 Ativo = prod.prod_bit_active,
                                 Categoria_01 = cat01.cat01_st_nome,
                                 Categoria_02 = cat02.cat02_st_nome,
                                 Categoria_03 = cat03.cat03_st_nome
                             }
                             ).OrderBy(u => u.Nome).ToList();
                if (!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model;
                    if (coluna == "Nome") modelSearch = model.Where(x => x.Nome.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Departamento") modelSearch = model.Where(x => x.Categoria_01.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Categoria") modelSearch = model.Where(x => x.Categoria_02.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Produto") modelSearch = model.Where(x => x.Categoria_03.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Código") modelSearch = model.Where(x => x.Codigo.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                return View(model);
            }
        }


        public ActionResult CadastrarProdutoDepartamento()
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var cat = (from cat01 in db.tb_categoria_n01
                           select new CadastroProduto
                           {
                               Cat01ID = cat01.cat01_in_codigo,
                               Cat01 = cat01.cat01_st_nome
                           }).ToList();
                return View(cat);
            }
        }

        public ActionResult CadastrarNovoDepartamento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CadastraCat01(tb_categoria_n01 cat01)
        {
            if (ModelState.IsValid)
            {
                using (EconobuyEntities db = new EconobuyEntities())
                {
                    var Cat = db.tb_categoria_n01.Add(cat01);
                    db.SaveChanges();
                    return RedirectToAction("CadastrarNovaCategoria", new { id = Cat.cat01_in_codigo, nome = Cat.cat01_st_nome });
                }
            }
            else
                return View(cat01);
        }
        public ActionResult CadastrarProdutoCategoria(int id, string nome)
        {
            Session["Cat01ID"] = id;
            Session["Cat01"] = nome;
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var cat = (from cat02 in db.tb_categoria_n02
                           where
cat02.cat01_in_codigo == id
                           select new CadastroProduto
                           {
                               Cat02ID = cat02.cat02_in_codigo,
                               Cat02 = cat02.cat02_st_nome
                           }).ToList();
                return View(cat);
            }
        }
        public ActionResult CadastrarNovaCategoria(int id, string nome)
        {
            Session["Cat01ID"] = id;
            Session["Cat01"] = nome;
            return View();
        }
        [HttpPost]
        public ActionResult CadastraCat02(tb_categoria_n02 cat02)
        {
            if (ModelState.IsValid)
            {
                using (EconobuyEntities db = new EconobuyEntities())
                {
                    cat02.cat01_in_codigo = Convert.ToInt32(Session["Cat01ID"]);
                    var Cat = db.tb_categoria_n02.Add(cat02);
                    db.SaveChanges();
                    return RedirectToAction("CadastrarNovoTipoProduto", new { id = Cat.cat02_in_codigo, nome = Cat.cat02_st_nome });
                }
            }
            else
                return View(cat02);
        }
        public ActionResult CadastrarProdutoTipoProduto(int id, string nome)
        {
            Session["Cat02ID"] = id;
            Session["Cat02"] = nome;
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var cat = (from cat03 in db.tb_categoria_n03
                           where
                           cat03.cat02_in_codigo == id
                           select new CadastroProduto
                           {
                               Cat03ID = cat03.cat02_in_codigo,
                               Cat03 = cat03.cat03_st_nome
                           }).ToList();
                return View(cat);
            }
        }
        public ActionResult CadastrarNovoTipoProduto(int id, string nome)
        {
            Session["Cat02ID"] = id;
            Session["Cat02"] = nome;
            return View();
        }
        [HttpPost]
        public ActionResult CadastraCat03(tb_categoria_n03 cat03)
        {
            if (ModelState.IsValid)
            {
                using (EconobuyEntities db = new EconobuyEntities())
                {
                    cat03.cat02_in_codigo = Convert.ToInt32(Session["Cat02ID"]);
                    var Cat = db.tb_categoria_n03.Add(cat03);
                    db.SaveChanges();
                    return RedirectToAction("CadastrarProduto", new { id = Cat.cat03_in_codigo, nome = Cat.cat03_st_nome });
                }
            }
            else
                return View(cat03);
        }
        public ActionResult CadastrarProduto(int id, string nome)
        {
            Session["Cat03ID"] = id;
            Session["Cat03"] = nome;
            return View();
        }

        [HttpPost]
        public ActionResult CadastraProduto(CadastroProduto prod, HttpPostedFileBase imgProduto)
        {
            HttpPostedFileBase file = Request.Files["img"];
            if (file.ContentLength > 0) prod.Imagem = ConvertToBytes(file);
            else prod.Imagem = System.IO.File.ReadAllBytes(Server.MapPath(@"\Repository\Images\NoImg.png"));
            var pro = new tb_produto
            {
                prod_bit_active = true,
                prod_bit_trad_active = prod.Tradicional,
                prod_dec_valor_un = prod.Valor,
                prod_st_cod_mer = prod.Codigo_Mercado,
                prod_st_descricao = prod.Descricao,
                prod_st_nome = prod.Nome,
                cat01_in_codigo = Convert.ToInt32(Session["Cat01ID"]),
                cat02_in_codigo = Convert.ToInt32(Session["Cat02ID"]),
                cat03_in_codigo = Convert.ToInt32(Session["Cat03ID"]),
                mer_in_codigo = Convert.ToInt32(Session["mercadoID"])
            };
            var img = new tb_produto_img
            {
                prod_img = prod.Imagem
            };
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("CadastrarProduto", prod);
                }
                else
                {
                    db.tb_produto.Add(pro);
                    if (prod.Imagem != null)
                    {
                        img.prod_in_codigo = pro.prod_in_codigo;
                        db.tb_produto_img.Add(img);
                    }
                    db.SaveChanges();
                    Session["Cat01ID"] = null;
                    Session["Cat02ID"] = null;
                    Session["Cat03ID"] = null;
                    Session["Cat01"] = null;
                    Session["Cat02"] = null;
                    Session["Cat03"] = null;
                    return RedirectToAction("ConsultarProdutos", "Mercado");
                }
            }
        }

        public ActionResult EditarProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto
                             join cat01 in db.tb_categoria_n01 on prod.cat01_in_codigo
                             equals cat01.cat01_in_codigo
                             join cat02 in db.tb_categoria_n02 on prod.cat02_in_codigo
                             equals cat02.cat02_in_codigo
                             join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                             equals cat03.cat03_in_codigo
                             where prod.prod_in_codigo == id
                             select new CadastroProduto
                             {
                                 Nome = prod.prod_st_nome,
                                 Descricao = prod.prod_st_descricao,
                                 Valor = prod.prod_dec_valor_un,
                                 Codigo_Mercado = prod.prod_st_cod_mer,
                                 Tradicional = prod.prod_bit_trad_active,
                                 Cat01 = cat01.cat01_st_nome,
                                 Cat02 = cat02.cat02_st_nome,
                                 Cat03 = cat03.cat03_st_nome,
                                 ProdID = prod.prod_in_codigo
                             }
                             ).First();
                return View(model);
            }
        }

        public ActionResult MostraImgProduto(int Id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_produto_img where img.prod_in_codigo == Id select img.prod_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        [HttpPost]
        public ActionResult EditaProduto(CadastroProduto cad, HttpPostedFileBase imgMercado)
        {
            HttpPostedFileBase file = Request.Files["img"];
            if (file.ContentLength > 0) cad.Imagem = ConvertToBytes(file);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_produto prod = db.tb_produto.Find(cad.ProdID);
                    int img_id = db.tb_produto_img.Where(x => x.prod_in_codigo == cad.ProdID).Select(x => x.prod_img_in_codigo).SingleOrDefault();
                    tb_produto_img img = db.tb_produto_img.Find(img_id);
                    if (prod != null)
                    {
                        prod.prod_st_nome = cad.Nome;
                        prod.prod_st_descricao = cad.Descricao;
                        prod.prod_dec_valor_un = cad.Valor;
                        prod.prod_st_cod_mer = cad.Codigo_Mercado;
                        prod.prod_bit_trad_active = cad.Tradicional;
                        if (cad.Imagem != null) img.prod_img = cad.Imagem;
                        db.SaveChanges();
                    }
                    return RedirectToAction("ConsultarProdutos", "Mercado");
                }
                else return View(cad);
            }
        }
        public ActionResult AtivaProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                tb_produto prod = db.tb_produto.Find(id);
                prod.prod_bit_active = !prod.prod_bit_active;
                db.SaveChanges();
                return RedirectToAction("ConsultarProdutos", "Mercado");
            }
        }
        public ActionResult AtivaTradProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                tb_produto prod = db.tb_produto.Find(id);
                prod.prod_bit_trad_active = !prod.prod_bit_trad_active;
                db.SaveChanges();
                return RedirectToAction("ConsultarProdutos", "Mercado");
            }
        }
        public ActionResult DeletaProduto(bool confirm, int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                tb_produto prod = db.tb_produto.Find(id);
                int img_id = db.tb_produto_img.Where(x => x.prod_in_codigo == id).Select(x => x.prod_img_in_codigo).SingleOrDefault();
                tb_produto_img img = db.tb_produto_img.Find(img_id);
                if (img != null) db.tb_produto_img.Remove(img);
                db.tb_produto.Remove(prod);
                db.SaveChanges();
                return RedirectToAction("ConsultarProdutos", "Mercado");
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Mercado");
        }
    }
}