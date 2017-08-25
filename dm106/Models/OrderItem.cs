using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dm106.Models
{

    /*        
        Identificação única do item de pedido;
        
        Quantidade de produtos do item de pedido;
        
        Identificação única do produto presente no item;
        
        Identificação única do pedido a qual o item pertence;
         
        O modelo também deverá considerar a existência de uma referência ao 
        produto em si, para que suas informações possam ser exibidas nas 
        respostas das operações dos pedidos.
    */

    public class OrderItem
    {
        public int Id { get; set; }

        public decimal quantidade { get; set; }

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation property
        public virtual Product Product { get; set; }

        public int OrderId { get; set; }

    }

}