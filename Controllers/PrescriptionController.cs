using Microsoft.AspNetCore.Mvc;
using PrzykladoweKolokwium.Models;
using PrzykladoweKolokwium.Repositories;

namespace PrzykladoweKolokwium.Controllers;



[ApiController]
[Route("api/prescriptions")]
public class PrescriptionController : ControllerBase
{
    private PrescriptionRepository _prescriptionRepository;
    
    public PrescriptionController(PrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> getReciepts(string? nazwisko = null)
    {
        var result =  await _prescriptionRepository.getReciepts(nazwisko);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] ReceptToInsert receptToInsert)
    {
        if (!await _prescriptionRepository.czyDataJestPoprawna(receptToInsert))
        {
            return NotFound($"Niepoprawna data");
        }

        var obj = await _prescriptionRepository.AddPrescription(receptToInsert);

        return Ok(obj);
    }


}