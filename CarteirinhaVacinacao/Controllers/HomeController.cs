using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteirinhaVacinacao.Models;
using CarteirinhaVacinacao.ViewModel;
using static CarteirinhaVacinacao.Classes.Utilites;

namespace CarteirinhaVacinacao.Controllers
{
    public class HomeController : Controller
    {
        private VacinacaoContext _vacinacaoContext;

        public HomeController(VacinacaoContext vacinacaoContext)
        {
            _vacinacaoContext = vacinacaoContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                if (pessoa.Login != null && pessoa.Login != "" && pessoa.Login != "")
                {
                    Pessoa _pessoa = _vacinacaoContext.Pessoas.Where(p => p.Login == pessoa.Login).FirstOrDefault();
                    if (_pessoa != null)
                    {
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
                    return RedirectToAction("MainPage", "Carteirinha", new { IdPessoa = pessoa.IdPessoa });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Arthur Vaz Pereira";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
