using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarteirinhaVacinacao.Models
{
    public class PessoaVacinada
    {
        public int IdPessoaVacinada { get; set; }
        [Required]
        public int IdPessoa { get; set; }
        [Required]
        public List<Vacina> Vacinas { get; set; }
        public DateTime DataAplicacao { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
