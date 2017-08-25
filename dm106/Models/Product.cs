using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dm106.Models
{
    public class Product
    {

        /*
         Identificação única do produto;         
         Nome do produto(com validação de preenchimento obrigatório);
         Descrição;
         Cor;
         Modelo em formato string (com validação de preenchimento obrigatório);
         Código em formato string (com validação de preenchimento obrigatório);
         Preço;
         Peso;
         Altura;
         Largura;
         Comprimento;
         Diâmetro;
         URL da imagem do produto em formato string.*/

        public int Id { get; set; }
        [Required(ErrorMessage = "O campo nome é obrigatório")]
        public string nome { get; set; }
        public string descricao { get; set; }
        public string cor { get; set; }
        [Required(ErrorMessage = "O campo modelo é obrigatório")]
        public string modelo { get; set; }
        [Required(ErrorMessage = "O campo codigo é obrigatório")]
        public string codigo { get; set; }
        public decimal preco { get; set; }
        public decimal peso { get; set; }
        public decimal altura { get; set; }
        public decimal largura { get; set; }
        public decimal comprimento { get; set; }
        public decimal diametro { get; set; }
        public string url { get; set; }
    }
}