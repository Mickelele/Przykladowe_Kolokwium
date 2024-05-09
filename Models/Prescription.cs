namespace PrzykladoweKolokwium.Models;

public class Prescription
{
    public int idPrescription { get; set; }
    public DateTime date { get; set; }
    public DateTime dueDate { get; set; }
    public int idPatient { get; set; }
    public int idDoctor { get; set; }
}