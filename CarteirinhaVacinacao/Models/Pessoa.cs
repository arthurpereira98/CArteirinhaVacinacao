using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarteirinhaVacinacao.Models
{
    public class Pessoa
    {        
        public int IdPessoa { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Login cannot be empty")]
        public string Login { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
        public string Salt { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        public string Nome { get; set; }
        [DataType(DataType.Date)]
        public DateTime Nascimento { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Cpf cannot be empty")]
        [StringLength(11)]
        public string CPF { get; set; }
        [Display(Name = "Upload Product Image")]
        [FileExtensions(Extensions = "jpg")]
        public string ImageName { get; set; }
    }
}
