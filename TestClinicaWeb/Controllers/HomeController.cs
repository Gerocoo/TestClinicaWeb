using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TestClinicaWeb.Models;

namespace TestClinicaWeb.Controllers
{
    public class HomeController : Controller
    {
        // Stringa con backslash singolo (corretto)
        private string cs = @"Server=localhost\SQLEXPRESS;Database=TestClinica;Integrated Security=True;TrustServerCertificate=True;";
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        private List<T> LeggiDB<T>(string query, Func<SqlDataReader, T> map)
        {
            var list = new List<T>();
            using var conn = new SqlConnection(cs);
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(map(r));
            return list;
        }

        public IActionResult Index()
        {
            // Leggiamo i valori dal file appsettings.json
            string campoDef = _config["Ricerca:CampoRicerca"] ?? "DescrizioneEsame";
            string testoDef = _config["Ricerca:TestoPredefinito"] ?? "";

            try
            {
                var ambulatori = LeggiDB("SELECT * FROM Ambulatori", r => new Ambulatorio { Id = (int)r["Id"], Nome = r["Nome"].ToString() });

                return View(new HomeViewModel
                {
                    Ambulatori = ambulatori,
                    CampoRicercaPredefinito = campoDef,
                    TestoRicercaPredefinito = testoDef
                });
            }
            catch (Exception ex)
            {
                ViewBag.ErroreDB = $"Impossibile connettersi al database. Verifica che SQL Server sia avviato e il database 'TestClinica' esista.\n\nDettaglio: {ex.Message}";

                return View(new HomeViewModel
                {
                    Ambulatori = new List<Ambulatorio>(),
                    CampoRicercaPredefinito = campoDef,
                    TestoRicercaPredefinito = testoDef
                });
            }
        }

        [HttpGet]
        public JsonResult GetPartiCorpo(int ambulatorioId, string ricerca = "", string campo = "DescrizioneEsame")
        {
            var collegamenti = LeggiDB("SELECT * FROM EsamiAmbulatori", r => new EsameAmbulatorio { EsameId = (int)r["EsameId"], AmbulatorioId = (int)r["AmbulatorioId"] });
            var esami = LeggiDB("SELECT * FROM Esami", r => new Esame { Id = (int)r["Id"], CodiceMinisteriale = r["CodiceMinisteriale"].ToString(), CodiceInterno = r["CodiceInterno"].ToString(), DescrizioneEsame = r["DescrizioneEsame"].ToString(), ParteCorpoId = (int)r["ParteCorpoId"] });
            var partiCorpo = LeggiDB("SELECT * FROM PartiCorpo", r => new ParteCorpo { Id = (int)r["Id"], Nome = r["Nome"].ToString() });

            var idEsamiInAmbulatorio = collegamenti.Where(c => c.AmbulatorioId == ambulatorioId).Select(c => c.EsameId).ToList();
            var esamiFiltrati = FiltroRicerca(esami.Where(e => idEsamiInAmbulatorio.Contains(e.Id)).ToList(), ricerca, campo);
            var idPartiCorpo = esamiFiltrati.Select(e => e.ParteCorpoId).Distinct().ToList();

            return Json(partiCorpo.Where(p => idPartiCorpo.Contains(p.Id)).ToList());
        }

        [HttpGet]
        public JsonResult GetEsami(int ambulatorioId, int parteCorpoId, string ricerca = "", string campo = "DescrizioneEsame")
        {
            var collegamenti = LeggiDB("SELECT * FROM EsamiAmbulatori", r => new EsameAmbulatorio { EsameId = (int)r["EsameId"], AmbulatorioId = (int)r["AmbulatorioId"] });
            var esami = LeggiDB("SELECT * FROM Esami", r => new Esame { Id = (int)r["Id"], CodiceMinisteriale = r["CodiceMinisteriale"].ToString(), CodiceInterno = r["CodiceInterno"].ToString(), DescrizioneEsame = r["DescrizioneEsame"].ToString(), ParteCorpoId = (int)r["ParteCorpoId"] });

            var idEsamiInAmbulatorio = collegamenti.Where(c => c.AmbulatorioId == ambulatorioId).Select(c => c.EsameId).ToList();
            var risultato = FiltroRicerca(esami.Where(e => e.ParteCorpoId == parteCorpoId && idEsamiInAmbulatorio.Contains(e.Id)).ToList(), ricerca, campo);

            return Json(risultato);
        }

        [HttpGet]
        public JsonResult GetAmbulatori(string ricerca = "", string campo = "DescrizioneEsame")
        {
            var collegamenti = LeggiDB("SELECT * FROM EsamiAmbulatori", r => new EsameAmbulatorio { EsameId = (int)r["EsameId"], AmbulatorioId = (int)r["AmbulatorioId"] });
            var esami = LeggiDB("SELECT * FROM Esami", r => new Esame { Id = (int)r["Id"], CodiceMinisteriale = r["CodiceMinisteriale"].ToString(), CodiceInterno = r["CodiceInterno"].ToString(), DescrizioneEsame = r["DescrizioneEsame"].ToString(), ParteCorpoId = (int)r["ParteCorpoId"] });
            var ambulatori = LeggiDB("SELECT * FROM Ambulatori", r => new Ambulatorio { Id = (int)r["Id"], Nome = r["Nome"].ToString() });

            var esamiFiltrati = FiltroRicerca(esami, ricerca, campo);
            var idAmbulatori = collegamenti.Where(c => esamiFiltrati.Select(e => e.Id).Contains(c.EsameId)).Select(c => c.AmbulatorioId).Distinct().ToList();

            return Json(ambulatori.Where(a => idAmbulatori.Contains(a.Id)).ToList());
        }

        private List<Esame> FiltroRicerca(List<Esame> esami, string ricerca, string campo)
        {
            if (string.IsNullOrEmpty(ricerca)) return esami;
            ricerca = ricerca.ToLower();
            return esami.Where(e =>
                (campo == "CodiceMinisteriale" && e.CodiceMinisteriale.ToLower().Contains(ricerca)) ||
                (campo == "CodiceInterno" && e.CodiceInterno.ToLower().Contains(ricerca)) ||
                (campo == "DescrizioneEsame" && e.DescrizioneEsame.ToLower().Contains(ricerca))
            ).ToList();
        }
    }
}