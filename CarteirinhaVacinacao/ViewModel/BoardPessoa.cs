﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarteirinhaVacinacao.Models;

namespace CarteirinhaVacinacao.ViewModel
{
    public class BoardPessoa
    {
        public Pessoa Pessoa { get; set; }
        public List<PessoaVacinada> PessoasVacinadas { get; set; }        
    }
}
