﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroProdutos.Models
{
    internal class Produtos
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco {  get; set; }
        public string Foto { get; set; }
        public decimal Custo { get; set; }
    }
}
