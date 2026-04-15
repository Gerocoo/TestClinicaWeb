namespace TestClinicaWeb.Models
{
    public class Ambulatorio { public int Id { get; set; } public string Nome { get; set; } }
    public class ParteCorpo { public int Id { get; set; } public string Nome { get; set; } }
    public class Esame { public int Id { get; set; } public string CodiceMinisteriale { get; set; } public string CodiceInterno { get; set; } public string DescrizioneEsame { get; set; } public int ParteCorpoId { get; set; } }
    public class EsameAmbulatorio { public int EsameId { get; set; } public int AmbulatorioId { get; set; } }

    public class HomeViewModel
    {
        public List<Ambulatorio> Ambulatori { get; set; }
        public List<Esame> EsamiScelti { get; set; } = new List<Esame>();
        public string CampoRicercaPredefinito { get; set; }
        public string TestoRicercaPredefinito { get; set; }
    }
}