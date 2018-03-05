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
        public List<Pessoa> Pessoa { get; set; }
        [Required]
        public ICollection<Vacina> Vacinas { get; set; }
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataAplicacao { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime DataVencimento { get; set; }
    }
}
