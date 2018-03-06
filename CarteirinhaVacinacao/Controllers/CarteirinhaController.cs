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

        public List<PessoaVacinada> ObtemPessoaVacinadaPorIdPessoa(int idPessoa)
        {
            List<PessoaVacinada> pvs = _vacinacaoContext.PessoasVacinadas.Where(x => x.IdPessoa == idPessoa).ToList();
            return pvs;
        }

        public Pessoa ObtemPessoaPorId(int idPessoa)
        {
            Pessoa pessoa = _vacinacaoContext.Pessoas.Where(x => x.IdPessoa == idPessoa).FirstOrDefault();
            return pessoa;
        }

        public List<Vacina> ObtemVacinasPorIdPessoa(int idPessoa)
        {
            List<PessoaVacinada> pvs = ObtemPessoaVacinadaPorIdPessoa(idPessoa);
            List<Vacina> vacinas = new List<Vacina>();
            foreach (PessoaVacinada pv in pvs)
            {
                vacinas = _vacinacaoContext.Vacinas.Where(x => x.IdVacina == pv.IdVacina).ToList();
            }
            return vacinas;
        }

        [HttpGet]
        public IActionResult MainPage(int idPessoa)
        {
            ViewModelPessoa _vmp = new ViewModelPessoa();
            if (!CheckSession() && idPessoa == 0) { return RedirectToAction("Index", "Home"); }
            Pessoa pessoa = ObtemPessoaPorId(idPessoa);
            List<PessoaVacinada> pvs = ObtemPessoaVacinadaPorIdPessoa(idPessoa);
            _vmp.Pessoa = pessoa;
            _vmp.PessoasVacinadas = pvs;
            _vmp.Vacinas = ObtemVacinasPorIdPessoa(idPessoa);           
            return View(_vmp);
        }

        [HttpGet]
        public IActionResult NovaVacina()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NovaVacina(Vacina vacina)
        {
            if (ModelState.IsValid)
            {
                if(vacina.IdVacina == 0)
                {
                    _vacinacaoContext.Vacinas.Add(vacina);
                    _vacinacaoContext.SaveChanges();
                }                
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult NovaPessoaVacinada(int idPessoa)
        {
            ViewModelPessoaVacinada _vmpv = new ViewModelPessoaVacinada();
            _vmpv.Vacinas = _vacinacaoContext.Vacinas.ToList();
            _vmpv.Pessoa = ObtemPessoaPorId(idPessoa);
            return View(_vmpv);
        }

        [HttpPost]
        public IActionResult NovaPessoaVacinada(PessoaVacinada pv)
        {
            if (!CheckSession()) { return RedirectToAction("Index", "Home"); }
            if (ModelState.IsValid)
            {
                if (pv.IdPessoa != 0 && pv.IdVacina != 0)
                {
                    pv.Pessoa = ObtemPessoaPorId(pv.IdPessoa);
                    pv.Vacina = _vacinacaoContext.Vacinas.Where(v => v.IdVacina == pv.IdVacina).FirstOrDefault();
                    pv.DataAplicacao = DateTime.Now;
                    pv.DataVencimento = pv.DataAplicacao.AddYears(10);
                    if (pv.IdPessoaVacinada == 0)
                    {
                        _vacinacaoContext.PessoasVacinadas.Add(pv);
                        _vacinacaoContext.SaveChangesAsync();
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