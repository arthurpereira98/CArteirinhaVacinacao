using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarteirinhaVacinacao.Models
{
    public class Vacina
    {
        public int IdVacina { get; set; }
        [Required]
        public string Nome { get; set; }
    }
}
