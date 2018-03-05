using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarteirinhaVacinacao.Models;

namespace CarteirinhaVacinacao.ViewModel
{
    public class ViewModelPessoaVacinada
    {
        public Pessoa Pessoa { get; set; }
        public List<Vacina> Vacinas { get; set; }
        public PessoaVacinada PessoaVacinada { get; set; }
    }
}
