using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dm106.Models
{

    /*
        Identificação única do pedido;
        E-mail do usuário dono do pedido;
        Data do pedido;
        Data de entrega do pedido;
        String com o status do pedido (novo, fechado, cancelado, entregue);
        Preço total do pedido;
        Peso total do pedido;
        Preço do frete;
        Lista de itens do pedido.
   */

    public class Order
    {

        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string userEmail { get; set; }

        public string userName { get; set; }

        public DateTime dateOrder { get; set; }

        public DateTime dateOrderDelivery { get; set; }

        public string statusOrder { get; set; }

        public decimal priceOrder { get; set; }

        public decimal weightOrder { get; set; }

        public decimal priceFreight { get; set; }

        public virtual ICollection<OrderItem> OrderItems
        {
            get; set;
        }
    }
}