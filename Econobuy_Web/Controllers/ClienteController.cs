using Econobuy_Web.Models;
using Linq.Extras;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Econobuy_Web.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LoginCliente(tb_cliente user)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var userDetail = db.tb_cliente.Where(x => x.cli_st_user == user.cli_st_user && x.cli_st_senha == user.cli_st_senha).FirstOrDefault();
                if (userDetail == null)
                {
                    TempData["Erro"] = "Usuário ou senha inválidos";
                    return View("Index", user);
                }
                else
                {
                    Session["clienteID"] = userDetail.cli_in_codigo;
                    Session["clienteNome"] = userDetail.cli_st_nome;
                    return RedirectToAction("Home", "Cliente");
                }
            }
        }

        public ActionResult RecuperarSenha(tb_cliente cli)
        {
            return View();
        }

        public ActionResult RecuperaSenha(tb_cliente cli)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int cliID = db.tb_cliente.Where(x => x.cli_st_email == cli.cli_st_email).Select(x => x.cli_in_codigo).SingleOrDefault();
                if (cliID > 0)
                {
                    Random rnd = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    string senha = new string(Enumerable.Repeat(chars, 10)
                      .Select(s => s[rnd.Next(s.Length)]).ToArray());
                    tb_cliente cl = db.tb_cliente.Find(cliID);
                    cl.cli_st_senha = senha;
                    db.SaveChanges();
                    EnviaSenhaEmail(cl.cli_st_email, cl.cli_st_user, senha);
                    TempData["Query"] = "Seus dados de acesso foram enviados para seu e-mail";
                    return View("RecuperarSenha", cli);
                }
                else
                {
                    TempData["Erro"] = "E-mail não encontrado no sistema";
                    return View("RecuperarSenha", cli);
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
            finally { }
            return;
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CadastraCliente(CadastroCliente cad)
        {
            var end = new tb_endereco
            {
                end_st_bairro = cad.Bairro,
                end_st_CEP = cad.CEP,
                end_st_cidade = cad.Cidade,
                end_st_compl = cad.Complemento,
                end_st_log = cad.Logradouro,
                end_st_num = cad.Numero,
                end_st_uf = cad.UF,
                end_st_tel1 = cad.Telefone_1,
                end_st_tel2 = cad.Telefone_2
            };
            var cli = new tb_cliente
            {
                cli_st_CPF = cad.CPF,
                cli_st_email = cad.email,
                cli_st_nome = cad.Nome,
                cli_st_senha = cad.Senha,
                cli_st_user = cad.User,
                cli_bit_active = true,
                cli_bit_advert = false,
                cli_bit_conf_email = false
            };
            var av = new tb_avaliacao_cliente
            {
                av_cli_dec_nota = 0
            };
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("Cadastro", cad);
                }
                else if (verificaUsuarioDuplicado(cad.User))
                {
                    TempData["ErroUser"] = "Usuário já cadastrado, insira outro usuário";
                    return View("Cadastro", cad);
                }
                else if (verificaEmailDuplicado(cad.email))
                {
                    TempData["ErroEmail"] = "E-mail já cadastrado, insira outro e-mail";
                    return View("Cadastro", cad);
                }
                else
                {
                    db.tb_endereco.Add(end);
                    cli.end_in_codigo = end.end_in_codigo;
                    db.tb_cliente.Add(cli);
                    av.cli_in_codigo = cli.cli_in_codigo;
                    db.tb_avaliacao_cliente.Add(av);
                    db.SaveChanges();
                    Session["clienteID"] = cli.cli_in_codigo;
                    Session["clienteNome"] = cli.cli_st_nome;
                    EnviaEmailCadastro(cli.cli_st_email, cli.cli_st_user, cli.cli_st_senha);
                    return RedirectToAction("Home", "Cliente");
                }
            };
        }


        public void EnviaEmailCadastro(string email, string usuario, string senha)
        {
            try
            {
                string msg = "Seja bem-vindo a Econobuy! \n\n Seguem seus dados de acesso: \n\n Usuario: " + usuario + " \n Senha: " + senha +
                    " \n\n Caso não tenha feito este cadastro apenas ignore este e-mail. \n\n Atenciosamente, \n Equipe Econobuy.";
                MailMessage mensagemEmail = new MailMessage("sistemaeconobuy@gmail.com", email, "Seja bem-vindo a Econobuy!", msg);
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sistemaeconobuy@gmail.com", "Nmb159nmb!");
                client.Send(mensagemEmail);
            }
            finally { }
            return;
        }

        public ActionResult alterarUsuario()
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var alter = (from cli in db.tb_cliente
                             join end in db.tb_endereco on
                             cli.end_in_codigo equals end.end_in_codigo
                             where cli.cli_in_codigo == Id
                             select new CadastroCliente
                             {
                                 User = cli.cli_st_user,
                                 Senha = cli.cli_st_senha,
                                 email = cli.cli_st_email,
                                 Telefone_1 = end.end_st_tel1,
                                 Telefone_2 = end.end_st_tel2,
                                 EndID = end.end_in_codigo,
                                 CliID = cli.cli_in_codigo,
                                 CEP = end.end_st_CEP,
                                 Cidade = end.end_st_cidade,
                                 Complemento = end.end_st_compl,
                                 CPF = cli.cli_st_CPF,
                                 Bairro = end.end_st_bairro,
                                 Logradouro = end.end_st_log,
                                 Nome = cli.cli_st_nome,
                                 Numero = end.end_st_num,
                                 UF = end.end_st_uf
                             });
                if (alter != null)
                {
                    CadastroCliente alt = alter.First();
                    return View(alt);
                }
                else return View();
            }
        }

        [HttpPost]
        public ActionResult AlteraUsuario(CadastroCliente alt)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("alterarUsuario", alt);
                }
                else
                {
                    tb_endereco end = db.tb_endereco.Find(alt.EndID);
                    tb_cliente cli = db.tb_cliente.Find(alt.CliID);
                    if (alt != null)
                    {
                        if (cli.cli_st_user == alt.User) cli.cli_st_user = alt.User;
                        else if(verificaUsuarioDuplicado(alt.User))
                        {
                            TempData["ErroUser"] = "Usuário já cadastrado, insira outro usuário";
                            return View("alterarUsuario", alt);
                        }
                        else cli.cli_st_user = alt.User;
                        cli.cli_st_senha = alt.Senha;
                        if(cli.cli_st_email == alt.email) cli.cli_st_email = alt.email;
                        else if(verificaEmailDuplicado(alt.email))
                        {
                            TempData["ErroEmail"] = "E-mail já cadastrado, insira outro e-mail";
                            return View("alterarUsuario", alt);
                        }
                        else cli.cli_st_email = alt.email;
                        cli.cli_st_CPF = alt.CPF;
                        cli.cli_st_nome = alt.Nome;
                        end.end_st_uf = alt.UF;
                        end.end_st_cidade = alt.Cidade;
                        end.end_st_compl = alt.Complemento;
                        end.end_st_log = alt.Logradouro;
                        end.end_st_bairro = alt.Bairro;
                        end.end_st_num = alt.Numero;
                        end.end_st_tel1 = alt.Telefone_1;
                        end.end_st_tel2 = alt.Telefone_2;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Home", "Cliente");
                }
            }
        }

        public bool verificaEmailDuplicado(string email)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var check = db.tb_cliente.Where(x => x.cli_st_email == email).FirstOrDefault();
                if (check != null) return true;
                else return false;
            };
        }

        public bool verificaUsuarioDuplicado(string user)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var check = db.tb_cliente.Where(x => x.cli_st_user == user).FirstOrDefault();
                if (check != null) return true;
                else return false;
            };
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }


        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ConsultarPedidos(string coluna, string search)
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from ped in db.tb_pedido
                             join mer in db.tb_mercado on
                             ped.mer_in_codigo equals mer.mer_in_codigo
                             join en in db.tb_endereco on ped.end_in_codigo
                             equals en.end_in_codigo
                             where ped.cli_in_codigo == Id
                             select new ConsultaPedidosCliente
                             {
                                 Id = ped.ped_in_codigo,
                                 Valor = ped.ped_dec_valor,
                                 Status = ped.ped_status,
                                 Data = ped.data_dt_pedido,
                                 Mercado = mer.mer_st_nome,
                                 CEP = en.end_st_CEP,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log
                             }
                             ).OrderBy(u => u.Status).ToList();
                if (!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model;
                    if (coluna == "Status") modelSearch = model.Where(x => x.Status.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Mercado") modelSearch = model.Where(x => x.Mercado.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                else return View(model);
            }
        }

        public ActionResult VisualizarPedido(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from en in db.tb_endereco  join mer
                            in db.tb_mercado on en.end_in_codigo
                            equals mer.end_in_codigo
                            join ped
                            in db.tb_pedido on mer.mer_in_codigo
                            equals ped.mer_in_codigo
                            where
                            ped.ped_in_codigo == id
                             select new VisualizarPedido
                             {
                                 Mercado_Ou_Cliente = mer.mer_st_nome,
                                 Data = ped.data_dt_pedido,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Numero = en.end_st_num,
                                 Email = mer.mer_st_email,
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
                if(avcli != 0) model.AvCli = db.tb_pedido_avaliacao_cliente.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_cli_st_descricao).FirstOrDefault();
                decimal avmer = 0;
                avmer = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_mer_dec_nota).FirstOrDefault();
                model.AvMercado = avmer;
                if (avmer != 0) 
                { 
                    model.AvMer = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_mer_st_descricao).FirstOrDefault(); 
                    model.AvActive = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_bit_active).FirstOrDefault();
                    var pedRemID = 0;
                    int avmerID = 0;
                    avmerID = db.tb_pedido_avaliacao_mercado.Where(x => x.ped_in_codigo == id).Select(x => x.ped_av_mer_in_codigo).FirstOrDefault();
                    if (avmerID != 0) { pedRemID = db.tb_pedido_remocao_avaliacao_cliente.Where(x => x.ped_av_mer_in_codigo == avmerID).Select(x => x.ped_rem_av_cli_in_codigo).FirstOrDefault(); }
                    model.PedRemID = Convert.ToInt32(pedRemID);
                }
                model.Itens = itens;
                return View(model);
            }
        }

        public ActionResult cancelarPedido(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                db.tb_item.RemoveRange(db.tb_item.Where(x => x.ped_in_codigo == id));
                db.tb_pedido.Remove(db.tb_pedido.Where(x => x.ped_in_codigo == id).FirstOrDefault());
                db.SaveChanges();
                return RedirectToAction("Home", "Cliente");
            }
        }

        public ActionResult marcarPedidoEntregue(int id)
        {
            using(EconobuyEntities db = new EconobuyEntities())
            {
                tb_pedido end = db.tb_pedido.Find(id);
                end.ped_status = "Entregue";
                db.SaveChanges();
                return RedirectToAction("avaliaPedidoEntregue", "Cliente", new { id = id });
            }
        }

        public ActionResult avaliaPedidoEntregue(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                TempData["pedID"] = id;
                int merID = db.tb_pedido.Where(x => x.ped_in_codigo == id).Select(x => x.mer_in_codigo).SingleOrDefault();
                int AvMerID = db.tb_avaliacao_mercado.Where(x => x.mer_in_codigo == merID).Select(x => x.av_mer_in_codigo).SingleOrDefault();
                TempData["AvMerID"] = AvMerID;
                return View();
            }
        }
        public ActionResult avaliarPedidoEntregue(tb_pedido_avaliacao_cliente pedav)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var av = new tb_pedido_avaliacao_cliente
                {
                    av_mer_in_codigo = Convert.ToInt32(TempData["AvMerID"]),
                    ped_av_cli_dec_nota = pedav.ped_av_cli_dec_nota,
                    ped_av_cli_st_descricao = pedav.ped_av_cli_st_descricao,
                    ped_in_codigo = Convert.ToInt32(TempData["pedID"]),
                    ped_bit_active = true
                };
                db.tb_pedido_avaliacao_cliente.Add(av);
                db.SaveChanges();
                return RedirectToAction("VisualizarPedido", "Cliente", new { Id = av.ped_in_codigo });
            }
        }

        public ActionResult pedidoRemocaoAvaliacao(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_pedido_avaliacao_mercado
                            where av.ped_in_codigo == id
                            select new PedidoRemocao
                            {
                                AvaliacaoID = av.ped_av_mer_in_codigo,
                                NotaAvaliacao = av.ped_av_mer_dec_nota,
                                DescricaoAvaliacao = av.ped_av_mer_st_descricao
                            }).FirstOrDefault();

                return View(model);
            }
        }

        public ActionResult criaPedidoRemocao(PedidoRemocao ped)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var rem = new tb_pedido_remocao_avaliacao_cliente
                {
                    ped_av_mer_in_codigo = ped.AvaliacaoID,
                    ped_rem_av_cli_st_msg_cli = ped.MsgRequisitante,
                    ped_rem_av_cli_st_msg_adm = "",
                    ped_status = "Em análise"
                };
                db.tb_pedido_remocao_avaliacao_cliente.Add(rem);
                db.SaveChanges();
                return RedirectToAction("acompanharPedidoRemocao", "Cliente", new { Id = rem.ped_rem_av_cli_in_codigo });
            }
        }

        public ActionResult acompanharPedidoRemocao(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_pedido_avaliacao_mercado 
                             join ped in db.tb_pedido_remocao_avaliacao_cliente on
                             av.ped_av_mer_in_codigo equals ped.ped_av_mer_in_codigo
                             where ped.ped_rem_av_cli_in_codigo == id
                             select new PedidoRemocao
                             {
                                 AvaliacaoID = av.ped_av_mer_in_codigo,
                                 NotaAvaliacao = av.ped_av_mer_dec_nota,
                                 DescricaoAvaliacao = av.ped_av_mer_st_descricao,
                                 MsgAdmin = ped.ped_rem_av_cli_st_msg_adm,
                                 MsgRequisitante = ped.ped_rem_av_cli_st_msg_cli,
                                 Status = ped.ped_status
                             }).FirstOrDefault();

                return View(model);
            }
        }


        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Cliente");
        }

        public ActionResult NovoPedido(string search)
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == Id).Select(x => x.end_in_codigo).SingleOrDefault();
                tb_endereco end = db.tb_endereco.Find(end_id);
                var model = (from cat03 in db.tb_categoria_n03
                             join prod in db.tb_produto on cat03.cat03_in_codigo
                             equals prod.cat03_in_codigo join
                             mer in db.tb_mercado on prod.mer_in_codigo
                             equals mer.mer_in_codigo join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo
                             where en.end_st_uf == end.end_st_uf &&
                             en.end_st_cidade == end.end_st_cidade
                             select new ListarProdutosModoEconobuy
                             {
                                 Cat03 = cat03.cat03_st_nome,
                                 Cat03ID = cat03.cat03_in_codigo
                             }
                             ).Distinct().OrderBy(u => u.Cat03).ToList(); 
                if (!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model.Where(x => x.Cat03.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                return View(model);
            }
        }

        public ActionResult MostraProdutoModoEconobuy(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int Id = Convert.ToInt32(Session["clienteID"]);
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == Id).Select(x => x.end_in_codigo).SingleOrDefault();
                tb_endereco end = db.tb_endereco.Find(end_id);
                var model = (from prod in db.tb_produto
                             join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                             equals cat03.cat03_in_codigo
                             where cat03.cat03_in_codigo == id 
                             select new Carrinho
                             {
                                 Nome = cat03.cat03_st_nome,
                                 Cat03ID = cat03.cat03_in_codigo
                             }
                             ).First();
                model.Valor = (from prod in db.tb_produto 
                              join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                               equals cat03.cat03_in_codigo
                               where cat03.cat03_in_codigo == id 
                               select prod.prod_dec_valor_un).Min();
                return View(model);
            }
        }


        public ActionResult MostraImgModoEconobuy(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_produto_img join 
                              prod in db.tb_produto on img.prod_in_codigo 
                              equals prod.prod_in_codigo where prod.cat03_in_codigo == id select img.prod_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        public ActionResult AdicionaAoCarrinho(Carrinho c)
        {
            if (ModelState.IsValid)
            {
                int qtde = 0;
                int id = 0;
                if (Session["Itens"] != null)
                {
                    foreach (var item in CarrinhoTemp.RetornaItens())
                    {
                        if (item.Cat03ID == c.Cat03ID)
                        {
                            qtde = item.Qtde + c.Qtde;
                            id = item.Id;
                            c.Qtde = qtde;
                        }
                    }
                    if (qtde == 0) CarrinhoTemp.ArmazenaItens(c);
                    else
                    {
                        CarrinhoTemp.RemoveItem(id);
                        CarrinhoTemp.ArmazenaItens(c);
                    }
                }
                else
                    CarrinhoTemp.ArmazenaItens(c);
                return RedirectToAction("NovoPedido", "Cliente");
            }
            return View();
        }

        public ActionResult ListaCarrinho()
        {
            var carrinho = CarrinhoTemp.RetornaItens();
            return View(carrinho);
        }

        public ActionResult DeletaItemCarrinho(int id)
        {
            CarrinhoTemp.RemoveItem(id);
            return RedirectToAction("ListaCarrinhoTrad", "Cliente");
        }

        public int selecionaMercado()
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == Id).Select(x => x.end_in_codigo).SingleOrDefault();
                tb_endereco end = db.tb_endereco.Find(end_id);
                var mercados = (from mer in db.tb_mercado join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             en.end_st_uf == end.end_st_uf &&
                             en.end_st_cidade == end.end_st_cidade
                             select mer.mer_in_codigo
                             ).ToList();
                int mer_id = 0;
                decimal valorFinal = 0;
                foreach (var mer in mercados)
                {
                    decimal valor = 0;
                    bool falta = false;
                    foreach (var item in CarrinhoTemp.RetornaItens())
                    {
                        var v = 0M;
                        v = (from merc in db.tb_mercado
                                     join prod in db.tb_produto on
                                     merc.mer_in_codigo equals prod.mer_in_codigo
                                     join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                                     equals cat03.cat03_in_codigo
                                     where cat03.cat03_in_codigo == item.Cat03ID &&
                                     merc.mer_in_codigo == mer
                                     select prod.prod_dec_valor_un).DefaultIfEmpty().Min();
                        if (v != 0M)
                        {
                            v *= item.Qtde;
                            valor += v;
                        }
                        else
                        {
                            falta = true;
                            break;
                        }
                    }
                    if (falta != true)
                    {
                        if (valorFinal == 0 || valor < valorFinal)
                        {
                            valorFinal = valor;
                            mer_id = mer;
                        }
                    }
                }
                return mer_id;
            }
        }

        public ActionResult FinalizaPedidoEconobuy()
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                Session["ItensFinal"] = null;
                decimal valor = 0;
                var carrinho = CarrinhoTemp.RetornaItens().ToList();
                int mer_id = selecionaMercado();
                foreach (var item in carrinho)
                {
                    var produto = (from merc in db.tb_mercado
                                   join prod in db.tb_produto on
                                     merc.mer_in_codigo equals prod.mer_in_codigo
                                   join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                                   equals cat03.cat03_in_codigo
                                   where cat03.cat03_in_codigo == item.Cat03ID &&
                                   merc.mer_in_codigo == mer_id
                                   select new CarrinhoTrad
                                   {
                                       Nome = prod.prod_st_nome,
                                       Descricao = prod.prod_st_descricao,
                                       Valor = prod.prod_dec_valor_un,
                                       ProdID = prod.prod_in_codigo
                                   }).MinBy(x => x.Valor);
                    produto.Qtde = item.Qtde;
                    produto.ValorTotal = produto.Valor * produto.Qtde;
                    CarrinhoTempFinal.ArmazenaItens(produto);
                    valor += produto.ValorTotal;
                }
                var carrinhoFinal = CarrinhoTempFinal.RetornaItens().ToList();
                var model = (from av in db.tb_avaliacao_mercado
                             join mer in db.tb_mercado on av.mer_in_codigo
                             equals mer.mer_in_codigo
                             join en in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             mer.mer_in_codigo == mer_id
                             select new FinalizaPedidoTrad
                             {
                                 Mercado = mer.mer_st_nome,
                                 AvMer = av.av_mer_dec_nota,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Complemento = en.end_st_compl,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Valor = valor,
                                 MerID = mer.mer_in_codigo
                             }
                             ).First();
                model.Carrinho = carrinhoFinal;
                return View(model);
            }
        }

        public ActionResult FinalizarPedidoEconobuy(int merID)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int cli_id = Convert.ToInt32(Session["clienteID"]);
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == cli_id).Select(x => x.end_in_codigo).SingleOrDefault();
                var ped = new tb_pedido
                {
                    cli_in_codigo = cli_id,
                    mer_in_codigo = merID,
                    data_dt_pedido = DateTime.Now,
                    end_in_codigo = end_id,
                    ped_status = "Aguardando",
                    ped_st_msg = "",
                    ped_dec_valor = 0
                };
                db.tb_pedido.Add(ped);
                decimal valor = 0;
                var carrinho = CarrinhoTempFinal.RetornaItens().ToList();
                foreach (var pro in carrinho)
                {
                    var item = new tb_item
                    {
                        prod_in_codigo = pro.ProdID,
                        ped_in_codigo = ped.ped_in_codigo,
                        item_in_qtde = pro.Qtde,
                        item_dec_valor = pro.ValorTotal
                    };
                    valor += pro.ValorTotal;
                    db.tb_item.Add(item);
                }
                ped.ped_dec_valor = valor;
                db.SaveChanges();
                Session["mercadoTradID"] = null;
                Session["Itens"] = null;
                return RedirectToAction("VisualizarPedido", "Cliente", new { Id = ped.ped_in_codigo });
            }
        }

        public ActionResult NovoPedidoTradicional(string coluna, string search)
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == Id).Select(x => x.end_in_codigo).SingleOrDefault();
                tb_endereco end = db.tb_endereco.Find(end_id);
                var model = (from av in db.tb_avaliacao_mercado
                             join mer in db.tb_mercado on av.mer_in_codigo
                             equals mer.mer_in_codigo join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             en.end_st_uf == end.end_st_uf &&
                             en.end_st_cidade == end.end_st_cidade
                             select new ListaMercadosModoTradicional
                             {
                                 Mercado = mer.mer_st_nome,
                                 Avaliacao = av.av_mer_dec_nota,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Numero = en.end_st_num,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Email = mer.mer_st_email,
                                 MerID = mer.mer_in_codigo
                             }
                             ).OrderBy(u => u.Mercado).ToList();
                if (!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model;
                    if (coluna == "Nome") modelSearch = model.Where(x => x.Mercado.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Bairro") modelSearch = model.Where(x => x.Bairro.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Logradouro") modelSearch = model.Where(x => x.Logradouro.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                else return View(model);
            }
        }
        public ActionResult excluirPedidoTrad()
        {
            Session["mercadoTradID"] = null;
            Session["ItensTrad"] = null;
            return RedirectToAction("NovoPedidoTradicional", "Cliente");
        }


        public ActionResult MostraLogoMercado(int Id)
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

        public ActionResult ListarProdutosModoTradicional(int id, string coluna, string search)
        {
            Session["mercadoTradID"] = id;
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto join cat01
                            in db.tb_categoria_n01 on prod.cat01_in_codigo
                            equals cat01.cat01_in_codigo join cat02 in
                            db.tb_categoria_n02 on prod.cat02_in_codigo
                            equals cat02.cat02_in_codigo join cat03 in
                            db.tb_categoria_n03 on prod.cat03_in_codigo
                            equals cat03.cat03_in_codigo where
                            prod.mer_in_codigo == id &&
                            prod.prod_bit_trad_active == true &&
                            prod.prod_bit_active == true
                            select new ConsultaProdutos
                            {
                                Id = prod.prod_in_codigo,
                                Nome = prod.prod_st_nome,
                                Preco = prod.prod_dec_valor_un,
                                Categoria_01 = cat01.cat01_st_nome,
                                Categoria_02 = cat02.cat02_st_nome,
                                Categoria_03 = cat03.cat03_st_nome
                            }
                            ).OrderBy(u => u.Nome).ToList();
                if(!String.IsNullOrEmpty(search))
                {
                    var modelSearch = model;
                    if(coluna == "Nome") modelSearch = model.Where(x => x.Nome.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Departamento") modelSearch = model.Where(x => x.Categoria_01.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Categoria") modelSearch = model.Where(x => x.Categoria_02.ToUpper().Contains(search.ToUpper())).ToList();
                    else if (coluna == "Produto") modelSearch = model.Where(x => x.Categoria_03.ToUpper().Contains(search.ToUpper())).ToList();
                    return View(modelSearch);
                }
                else return View(model);
            }
        }

        public ActionResult MostraImagemProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_produto_img where img.prod_in_codigo == id select img.prod_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        public ActionResult MostraProduto(int id)
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
                             select new CarrinhoTrad
                             {
                                 Nome = prod.prod_st_nome,
                                 Descricao = prod.prod_st_descricao,
                                 Valor = prod.prod_dec_valor_un,
                                 Cat01 = cat01.cat01_st_nome,
                                 Cat02 = cat02.cat02_st_nome,
                                 Cat03 = cat03.cat03_st_nome,
                                 ProdID = prod.prod_in_codigo
                             }
                             ).First();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult AdicionaAoCarrinhoTrad(CarrinhoTrad c)
        {

            if (ModelState.IsValid)
            {
                int qtde = 0;
                int id = 0;
                if (Session["ItensTrad"] != null)
                {
                    foreach (var item in CarrinhoTradTemp.RetornaItens())
                    {
                        if (item.ProdID == c.ProdID)
                        {
                            qtde = item.Qtde + c.Qtde;
                            id = item.Id;
                            c.Qtde = qtde;
                        }
                    }
                    c.ValorTotal = c.Valor * c.Qtde;
                    if (qtde == 0) CarrinhoTradTemp.ArmazenaItens(c);
                    else
                    {
                        CarrinhoTradTemp.RemoveItem(id);
                        CarrinhoTradTemp.ArmazenaItens(c);
                    }
                }
                else
                {
                    c.ValorTotal = c.Valor * c.Qtde;
                    CarrinhoTradTemp.ArmazenaItens(c);
                }
                return RedirectToAction("ListaCarrinhoTrad", "Cliente");
            }
                return View();

        }

        public ActionResult ListaCarrinhoTrad()
        {
            var carrinho = CarrinhoTradTemp.RetornaItens();
            return View(carrinho);
        }

        public ActionResult DeletaItemCarrinhoTrad(int id)
        {
            CarrinhoTradTemp.RemoveItem(id);
            return RedirectToAction("ListaCarrinhoTrad", "Cliente");
        }

        public ActionResult FinalizaPedidoTrad()
        {
            decimal valor = 0;
            var carrinho = CarrinhoTradTemp.RetornaItens().ToList();
            foreach (var item in carrinho)
            {
                valor += item.ValorTotal;
            }
            int Id = Convert.ToInt32(Session["mercadoTradID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_avaliacao_mercado
                             join mer in db.tb_mercado on av.mer_in_codigo
                             equals mer.mer_in_codigo join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             mer.mer_in_codigo == Id
                             select new FinalizaPedidoTrad
                             {
                                 Mercado = mer.mer_st_nome,
                                 AvMer = av.av_mer_dec_nota,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Complemento = en.end_st_compl,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Valor = valor
                             }
                             ).First();
                model.Carrinho = carrinho;
                return View(model);
            }
        }

        public ActionResult FinalizarPedidoTrad(FinalizaPedidoTrad tr)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int cli_id = Convert.ToInt32(Session["clienteID"]);
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == cli_id).Select(x => x.end_in_codigo).SingleOrDefault();
                int mer_id = Convert.ToInt32(Session["mercadoTradID"]);
                var ped = new tb_pedido
                {
                    cli_in_codigo = cli_id,
                    mer_in_codigo = mer_id,
                    data_dt_pedido = DateTime.Now,
                    end_in_codigo = end_id,
                    ped_status = "Aguardando",
                    ped_st_msg = "",
                    ped_dec_valor = 0
                };
                db.tb_pedido.Add(ped);
                decimal valor = 0;
                var carrinho = CarrinhoTradTemp.RetornaItens().ToList();
                foreach (var pro in carrinho)
                {
                    var item = new tb_item
                    {
                        prod_in_codigo = pro.ProdID,
                        ped_in_codigo = ped.ped_in_codigo,
                        item_in_qtde = pro.Qtde,
                        item_dec_valor = pro.ValorTotal
                    };
                    valor += pro.ValorTotal;
                    db.tb_item.Add(item);
                }
                ped.ped_dec_valor = valor;
                db.SaveChanges();
                Session["mercadoTradID"] = null;
                Session["ItensTrad"] = null;
                return RedirectToAction("VisualizarPedido", "Cliente", new { Id = ped.ped_in_codigo });
            }
        }


    }
}