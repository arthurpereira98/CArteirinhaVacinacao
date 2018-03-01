using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarteirinhaVacinacao.Models
{
    public class Pessoa
    {
        public int IdPessoa { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username cannot be empty")]
        public string Login { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
        public string Salt { get; set; }
        [Required]
        public string Nome { get; set; }
        public DateTime Nascimento { get; set; }
        [Required]
        public string CPF { get; set; }
    }
}
