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

namespace dm106.Controllers
{

    [Authorize]
    public class ProductsController : ApiController
    {

        private dm106Context db = new dm106Context();

        /*
            Operação para listagem de todos os produtos, que poderá ser acessível apenas por usuários cadastrados, 
            inclusive o administrador;
        */
        // GET: api/Products
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }


        /*         
            Operação para recuperar a informação de um produto a partir de sua identificação única, 
            que poderá ser acessível apenas por usuários cadastrados, inclusive o administrador;
        */
        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        /*
            Operação para alterar a informação de um produto a partir de sua identificação única,
            que poderá ser acessível somente pelo usuário administrador.Não deverá permitir a
            alteração do código e/ou do modelo para um valor que já exista na tabela;
        */
        // PUT: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            // recupera o produto do id
            Product tmpProduct = new dm106Context().Products.Find(id);
            // confere se alterou os dados e se ja existe no banco um valor igual
            if ( ( (tmpProduct.codigo != product.codigo) && (ProductExistsByCodigo(product.codigo)) ) ||
                 ( (tmpProduct.modelo != product.modelo) && (ProductExistsByModelo(product.modelo)) ) )
            {                
                return BadRequest("Não foi possivel alterar o produto, codigo e/ou modelo já existente!");
            }
            else
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();              
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /*         
            Operação para inclusão de um novo produto, que poderá ser acessível somente pelo usuário administrador. 
            Não deverá permitir a inclusão de um produto com o código e/ou o modelo com um valor que já exista na tabela;
        */
        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ProductExistsByCodigo(product.codigo) || ProductExistsByModelo(product.modelo))
            {
                return BadRequest("Não foi possivel inserir o produto, codigo e/ou modelo já existente!");
            }
            else
            {
                db.Products.Add(product);
                db.SaveChanges();
            }

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }


        /*
            Operação para apagar um produto a partir de sua identificação única, que poderá ser 
            acessível somente pelo usuário administrador.
        */
        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }

        private bool ProductExistsByModelo(string modelo)
        {
            return db.Products.Count(e => e.modelo == modelo) > 0;
        }

        private bool ProductExistsByCodigo(string codigo)
        {
            return db.Products.Count(e => e.codigo == codigo) > 0;
        }
    }
}
 
 