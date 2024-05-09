namespace PrzykladoweKolokwium.Models;

public class ReturnReceptType
{
    public int idPrescription { get; set; }
    public DateTime date { get; set; }
    public DateTime dueDate { get; set; }
    public string patientLastName { get; set; }
    public string doctorLastName { get; set; }
}