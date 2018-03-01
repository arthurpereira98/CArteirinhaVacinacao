using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteirinhaVacinacao.Models;
using CarteirinhaVacinacao.ViewModel;
using static CarteirinhaVacinacao.Classes.Utilites;

namespace CarteirinhaVacinacao.Controllers
{
    public class CarteirinhaController : Controller
    {
        private VacinacaoContext vacinacaoContext;

        public CarteirinhaController(VacinacaoContext v)
        {

        }

        public IActionResult Index()
        {
            if (!CheckSession()) { return RedirectToAction("Login", "Vacina"); }
            return View();
        }
        [HttpGet]
        public IActionResult CadastrarPessoa()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarPessoa(Pessoa pessoa)
        {
            if (vacinacaoContext.Pessoas.Where(p => p.Login == pessoa.Login).Count() == 0)
            {
                Random random = new Random();
                pessoa.Salt = random.Next(11111111, 99999999).ToString();
                pessoa.Password = HashPass(pessoa.Password, pessoa.Salt);
                vacinacaoContext.Add(pessoa);
                vacinacaoContext.SaveChanges();
            }
            else
            {
                return RedirectToAction("CadastrarUsuario", "Sistema");
            }
            return RedirectToAction("Login", "Sistema");
        }

        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Pessoa pessoa)
        {
            if (pessoa.Login != null && pessoa.Login != "" && pessoa.Login != "")
            {
                Pessoa _pessoa = vacinacaoContext.Pessoas.Where(p => p.Login == pessoa.Login).FirstOrDefault();
                pessoa.Password = HashPass(pessoa.Password, _pessoa.Salt);
                if (pessoa.Password == _pessoa.Password)
                {
                    HttpContext.Session.SetString("HashPass", pessoa.Password);
                    HttpContext.Session.SetString("UserId", _pessoa.IdPessoa.ToString());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Vacinados", "Carteirinha", new { IdPessoa = pessoa.IdPessoa });
        }

        [HttpGet]
        public IActionResult Vacinados()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Vacinados(int idPessoa)
        {
            BoardPessoa _bp = new BoardPessoa();
            
            if (!CheckSession()) { return RedirectToAction("Login", "Sistema"); }
            if(idPessoa == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            _bp.Pessoa = vacinacaoContext.Pessoas.Where(p => p.IdPessoa == idPessoa).FirstOrDefault();
            _bp.PessoasVacinadas = vacinacaoContext.PessoasVacinadas.Where(p => p.IdPessoa == idPessoa).ToList();
            return View(_bp);
        }

            public Boolean CheckSession()
        {
            string UId = HttpContext.Session.GetString("UserId");
            if (!String.IsNullOrEmpty(UId))
            {
                int IdPessoa = Int32.Parse(UId);
                string uPHash = vacinacaoContext.Pessoas.Where(p => p.IdPessoa == IdPessoa).Select(p => p.Password).FirstOrDefault();
                if (uPHash == HttpContext.Session.GetString("HashPass"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}