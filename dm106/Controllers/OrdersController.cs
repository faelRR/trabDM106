using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using dm106.Models;
using System.Security.Principal;
using dm106.CRMClient;
using dm106.br.com.correios.ws;

namespace dm106.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private dm106Context db = new dm106Context();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            
            if (order == null)
            {
                return BadRequest("Pedido não encontrado");
            }

            // Se nao for admin acesso nao autorizado e se nao for o dono no pedido nao autorizado
            if (checkUserFromOrder(User, order))
            {
                return Ok(order);
            }
            else
            {
                return BadRequest("Acesso restrito ao pedido " + id);
            }
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrderByEmail(String email)
        {
            IQueryable<Order> orders = db.Orders.Where(p => p.userEmail == email);

            // Se nao for admin acesso nao autorizado e se nao for o dono no pedido nao autorizado
            if (checkUserEmailOrder(User, email))
            {
                return Ok(orders);
            }
            else
            {
                return BadRequest("Acesso não autorizado");
            }            
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("api/closeOrder")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult CloseOrder(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = db.Orders.Find(id);

            if (order == null)
            {
                return BadRequest("Pedido não existe");
            }

            // Se nao for admin acesso nao autorizado e se nao for o dono no pedido nao autorizado
            if (checkUserEmailOrder(User, order.userEmail))
            {

                // se preco do frete ainda nao calculado
                if (order.priceFreight == 0)
                {
                    return BadRequest("Pedido não pode ser fechado, pedido sem o preço do frete calculado");
                }
                else
                {
                    order.statusOrder = "fechado";
                    db.Entry(order).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        return BadRequest("Acesso não autorizado " + e.Message);
                    }
                }

            }
            else
            {
                return BadRequest("Acesso não autorizado");
            }


            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /* Status = “novo”;
               o Peso total do pedido = 0;
               o Preço do frete = 0;
               o Preço total do pedido = 0;
               o Data do pedido = data atual;*/

            order.statusOrder = "novo";
            order.weightOrder = 0;
            order.priceFreight = 0;
            order.priceOrder = 0;
            order.dateOrder = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }



        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {

            Order order = db.Orders.Find(id);
            // Se nao for admin acesso nao autorizado e se nao for o dono no pedido nao autorizado


            if (order == null)
            {
                return BadRequest("Pedido não existe");
            }
            else
            {
                if (checkUserEmailOrder(User, order.userEmail))
                {
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
                else
                {
                    return BadRequest("Acesso não autorizado");
                }
            }
            return Ok(order);
        }

        [Route("api/freteDataEntregaOrder")]
        [HttpPut]
        [ResponseType(typeof(Order))]
        public IHttpActionResult FreteDataEntregaOrder(int id)
        {

            // recupera o pedido pelo ID
            Order order = db.Orders.Find(id);

            // se nao existir o pedido
            if (order == null)
            {
                return BadRequest("Pedido não existe");
            }
            else
            {

                // confere se é o admin ou o dono
                if (checkUserFromOrder(User, order))
                {

                    if (order.statusOrder != "novo")
                    {
                        return BadRequest("Status do pedido diferente de 'novo', não é possivel recalcular o frete.");
                    }


                    if (order.OrderItems.Count == 0)
                    {
                        return BadRequest("Pedido sem itens cadastrados");
                    }
                    

                    CRMRestClient crmClient = new CRMRestClient();
                    Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);

                    // zera as medidas
                    decimal considerarDiametro = 0;
                    decimal considerarAltura = 0;
                    decimal considerarComprimento = 0;
                    decimal largura = 0;

                    // confere se achou o cliente
                    if (customer != null)
                    {
                        order.priceOrder = 0;
                        order.weightOrder = 0;
                        // soma medida dos itens
                        foreach (OrderItem tmpOrder in order.OrderItems)
                        {
                            // somatorio do preço
                            order.priceOrder += tmpOrder.Product.preco * tmpOrder.quantidade;

                            // somatorio do peso
                            order.weightOrder += tmpOrder.Product.peso * tmpOrder.quantidade;


                            // -- considerando que os produtos estaram um do lado do outro, 
                            //               somo a largura e pego a maior altura , comprimento e diametro -- //

                            // somatorio da largura
                            largura += tmpOrder.Product.largura * tmpOrder.quantidade;

                            if (tmpOrder.Product.altura > considerarAltura)
                            {
                                considerarAltura = tmpOrder.Product.altura;
                            }
                            if (tmpOrder.Product.diametro > considerarDiametro)
                            {
                                considerarDiametro = tmpOrder.Product.diametro;
                            }
                            if (tmpOrder.Product.comprimento > considerarComprimento)
                            {
                                considerarComprimento = tmpOrder.Product.comprimento;
                            }

                        }

                        // Considerando que a loja esteja em São Luís - MA => Rua Vila Anselmo - 65040-101

                        CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
                        // (  string nCdEmpresa, string sDsSenha, string nCdServico, string sCepOrigem, 
                        //    string sCepDestino, string nVlPeso, int nCdFormato, decimal nVlComprimento, 
                        //    decimal nVlAltura, decimal nVlLargura, decimal nVlDiametro, string sCdMaoPropria, 
                        //    decimal nVlValorDeclarado, string sCdAvisoRecebimento) 

                        cResultado resultado = correios.CalcPrecoPrazo("", "", "40010", "65040101", customer.zip,
                            order.weightOrder.ToString(), 1, considerarComprimento, considerarAltura, largura, considerarDiametro,
                            "N", order.priceOrder, "S");

                        if (resultado.Servicos[0].Erro.Equals("0"))
                        {
                            // ajusta preço do frete
                            order.priceFreight = Convert.ToDecimal(resultado.Servicos[0].Valor) / 100;
                            // atualiza o preço do pedido , somando o valor do frete
                            order.priceOrder += order.priceFreight;
                            // considerar a data da entrega = data pedido mais prazo entrega
                            order.dateOrderDelivery = order.dateOrder.AddDays(Convert.ToDouble(resultado.Servicos[0].PrazoEntrega));
                            //modificações são persistidas no banco de dados
                            db.SaveChanges();

                            return Ok("Preço do frete: R$ " + resultado.Servicos[0].Valor + " => Entrega em " + resultado.Servicos[0].PrazoEntrega + " dia(s)");
                        }
                        else
                        {
                            return BadRequest("Ouve um erro " + resultado.Servicos[0].Erro + " , " + resultado.Servicos[0].MsgErro);
                        }
                    }
                    else
                    {
                        return BadRequest("Impossibilidade ou erro ao acessar o serviço de CRM ");
                    }
                }
                else
                {
                    return BadRequest("Acesso não autorizado");
                }
            }

        }
    

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("cep")]
        public IHttpActionResult ObtemCEP()
        {
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);

            if (customer != null)
            {
                return Ok(customer.zip);
            }
            else
            {
                return BadRequest("Falha ao consultar o CRM");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool checkUserFromOrder(IPrincipal user, Order order)
        {
            return ((user.Identity.Name.Equals(order.userName)) || (user.IsInRole("ADMIN")));
        }

        private bool checkUserEmailOrder(IPrincipal user, String email)
        {
            return ((user.Identity.Name.Equals(email)) || (user.IsInRole("ADMIN")));
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}