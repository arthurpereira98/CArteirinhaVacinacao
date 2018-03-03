using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteirinhaVacinacao.Models;
using CarteirinhaVacinacao.ViewModel;
using static CarteirinhaVacinacao.Classes.Utilites;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace CarteirinhaVacinacao.Controllers
{
    public class CarteirinhaController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private VacinacaoContext vacinacaoContext;

        public CarteirinhaController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CadastrarPessoa()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarPessoa([Bind("IdPessoa,Login,Password,Salt,Nome,Nascimento,CPF")]Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        var uploads = Path.Combine(webRootPath, "uploads\\img\\pessoas");

                        if (file.Length > 0)
                        {
                            var fileName = ContentDispositionHeaderValue.Parse
                                (file.ContentDisposition).FileName.Trim('"');

                            System.Console.WriteLine(fileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                pessoa.ImageName = file.FileName;
                            }
                            var imageUrl = Path.Combine(uploads + file.FileName);

                        }
                    }
                }
                if (vacinacaoContext.Pessoas.Where(p => p.Login == pessoa.Login).Count() == 0)
                {
                    Random random = new Random();
                    pessoa.Salt = random.Next(11111111, 99999999).ToString();
                    pessoa.Password = HashPass(pessoa.Password, pessoa.Salt);
                    vacinacaoContext.Add(pessoa);
                    await  vacinacaoContext.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("CadastrarUsuario", "Sistema");
                }                
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MainPage(int idPessoa)
        {
            BoardPessoa _bp = new BoardPessoa();
            if (!CheckSession() || idPessoa == 0) { return RedirectToAction("Index", "Home"); }           
            if (idPessoa == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            _bp.Pessoa = vacinacaoContext.Pessoas.Where(p => p.IdPessoa == idPessoa).FirstOrDefault();
            _bp.PessoasVacinadas = vacinacaoContext.PessoasVacinadas.Where(p => p.IdPessoa == idPessoa).ToList();
            return View(_bp);
        }           

        [HttpGet]
        public IActionResult NovaPessoaVacinada(int IdPessoa)
        {
            BoardPessoaVacinada _bpv = new BoardPessoaVacinada();
            _bpv.Vacinas.ToList();
            _bpv.Pessoa = vacinacaoContext.Pessoas.Where(p => p.IdPessoa == IdPessoa).FirstOrDefault();
            return View(_bpv);
        }

        [HttpPost]
        public IActionResult NovaPessoaVacinada([FromBody]PessoaVacinada pv)
        {
            if (!CheckSession()) { return RedirectToAction("Index", "Home"); }
            if (pv.IdPessoaVacinada == 0)
            {
                vacinacaoContext.PessoasVacinadas.Add(pv);
                vacinacaoContext.SaveChanges();
            }
            vacinacaoContext.PessoasVacinadas.Add(pv);
            vacinacaoContext.SaveChanges();
            return RedirectToAction("MainPage", "Carteirinha");
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