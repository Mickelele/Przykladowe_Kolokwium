using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using PrzykladoweKolokwium.Models;

namespace PrzykladoweKolokwium.Repositories;

public class PrescriptionRepository
{
    private string _connectionString;

    public PrescriptionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }



    public async Task<IEnumerable<ReturnReceptType>> getReciepts(string? nazwisko)
    {
        List<ReturnReceptType> result = new List<ReturnReceptType>();
        string query = "";
        
        
        if (string.IsNullOrEmpty(nazwisko))
        {
            query = @"SELECT Prescription.IdPrescription, Prescription.[Date], Prescription.[DueDate], 
                          Patient.LastName AS PatientLastName, Doctor.LastName AS DoctorLastName 
                   FROM Prescription
                   JOIN Patient ON Patient.IdPatient = Prescription.IdPatient
                   JOIN Doctor ON Doctor.IdDoctor = Prescription.IdDoctor";
        }
        else
        {
            query = @"SELECT Prescription.IdPrescription, Prescription.[Date], Prescription.[DueDate], 
                          Patient.LastName AS PatientLastName, Doctor.LastName AS DoctorLastName 
                   FROM Prescription 
                   JOIN Patient ON Patient.IdPatient = Prescription.IdPatient 
                   JOIN Doctor ON Doctor.IdDoctor = Prescription.IdDoctor 
                   WHERE Doctor.LastName LIKE @Nazwisko";
        }
        
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);
        if (!string.IsNullOrEmpty(nazwisko))
        {
            command.Parameters.AddWithValue("@Nazwisko", $"%{nazwisko}%");
        }
        
        

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        
        while (await reader.ReadAsync())
        {
            result.Add(new ReturnReceptType
            {
                idPrescription = (int)reader["IdPrescription"],
                date = (DateTime)reader["Date"],
                dueDate = (DateTime)reader["DueDate"],
                patientLastName = reader["PatientLastName"].ToString(),
                doctorLastName = reader["DoctorLastName"].ToString()
            });
        }
        

        return result;
    }


    public async Task<Prescription> AddPrescription(ReceptToInsert receptToInsert)
    {

        string query = "INSERT INTO Prescription VALUES (@date, @dueDate, @idPatient, @idDoctor); SELECT SCOPE_IDENTITY()";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@date",receptToInsert.date);
        command.Parameters.AddWithValue("@dueDate",receptToInsert.dueDate);
        command.Parameters.AddWithValue("@idPatient",receptToInsert.idPatient);
        command.Parameters.AddWithValue("@idDoctor",receptToInsert.idDoctor);
        await connection.OpenAsync();
        
        var newId = await command.ExecuteScalarAsync();
        
        Prescription newPrescription = new Prescription
        {
            idPrescription = Convert.ToInt32(newId),
            date = receptToInsert.date,
            dueDate = receptToInsert.dueDate,
            idPatient = receptToInsert.idPatient,
            idDoctor = receptToInsert.idDoctor
        };

        return newPrescription;
        
    }


    public async Task<bool> czyDataJestPoprawna(ReceptToInsert receptToInsert)
    {
        if (receptToInsert.dueDate < receptToInsert.date)
        {
            return false;
        }

        return true;
    }






}