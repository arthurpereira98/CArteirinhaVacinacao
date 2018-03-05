using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteirinhaVacinacao.Models;
using Microsoft.AspNetCore.Session;
using CarteirinhaVacinacao.ViewModel;
using static CarteirinhaVacinacao.Utilites.Utilites;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace CarteirinhaVacinacao.Controllers
{
    public class CarteirinhaController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private VacinacaoContext _vacinacaoContext;        

        public CarteirinhaController(IHostingEnvironment hostingEnvironment, VacinacaoContext vacinacaoContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _vacinacaoContext = vacinacaoContext;
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
                        var uploads = Path.Combine(webRootPath, "Uploads\\Pessoas");

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
                if (_vacinacaoContext.Pessoas.Where(p => p.Login == pessoa.Login).Count() == 0)
                {
                    Random random = new Random();
                    pessoa.Salt = random.Next(11111111, 99999999).ToString();
                    pessoa.Password = HashPass(pessoa.Password, pessoa.Salt);
                    _vacinacaoContext.Add(pessoa);
                    await _vacinacaoContext.SaveChangesAsync();
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
            ViewModelPessoa _bp = new ViewModelPessoa();
            if (!CheckSession() && idPessoa == 0) { return RedirectToAction("Index", "Home"); }
            Pessoa pessoa = _vacinacaoContext.Pessoas.Where(x => x.IdPessoa == idPessoa).FirstOrDefault();
            _bp.Pessoa = _vacinacaoContext.Pessoas.Where(p => p.IdPessoa == idPessoa).FirstOrDefault();
            _bp.PessoasVacinadas = _vacinacaoContext.PessoasVacinadas.Where(x => x.IdPessoa == idPessoa).ToList();
            return View(_bp);
        }

        [HttpGet]
        public IActionResult NovaPessoaVacinada(int IdPessoa)
        {
            ViewModelPessoaVacinada _vmpv = new ViewModelPessoaVacinada();
            _vmpv.Vacinas = _vacinacaoContext.Vacinas.ToList();
            _vmpv.Pessoa = _vacinacaoContext.Pessoas.Where(p => p.IdPessoa == IdPessoa).FirstOrDefault();
            return View(_vmpv);
        }

        [HttpPost]
        public async Task<IActionResult> NovaPessoaVacinada([FromBody][Bind("IdPessoaVacina,IdPessoa,IdVacina,DataAplicacao,DataVencimento")]PessoaVacinada pv)
        {
            if (!CheckSession()) { return RedirectToAction("Index", "Home"); }
            if (ModelState.IsValid)
            {
                if (pv.IdPessoa != 0 && pv.IdVacina != 0)
                {
                    if (pv.IdPessoaVacinada == 0)
                    {
                        _vacinacaoContext.PessoasVacinadas.Add(pv);
                        await _vacinacaoContext.SaveChangesAsync();
                    }
                    return RedirectToAction("MainPage", "Carteirinha", new { IdPessoa = pv.IdPessoa });
                }

            }            
            return RedirectToAction("Index", "Home");
        }

        public Boolean CheckSession()
        {
            string UId = HttpContext.Session.GetString("UserId");
            if (!String.IsNullOrEmpty(UId))
            {
                int IdPessoa = Int32.Parse(UId);
                string uPHash = _vacinacaoContext.Pessoas.Where(p => p.IdPessoa == IdPessoa).Select(p => p.Password).FirstOrDefault();
                if (uPHash == HttpContext.Session.GetString("HashPass"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}