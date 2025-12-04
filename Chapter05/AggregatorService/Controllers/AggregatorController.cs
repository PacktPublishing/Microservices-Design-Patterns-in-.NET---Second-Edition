using System;
using AggregatorService.Models;
using AggregatorService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AggregatorController(
    PatientServiceClient _patientService,
    MedicalHistoryServiceClient _medicalHistoryService,
    AppointmentsServiceClient _appointmentService,
    BillingServiceClient _billingService) : ControllerBase
{
    [HttpGet("patient-dashboard/{patientId}")]
    public async Task<ActionResult<PatientDashboard>> GetPatientDashboard(int patientId)
    {
        var patientTask = _patientService.GetPatientAsync(patientId);
        var medicalHistoryTask = _medicalHistoryService.GetMedicalHistoryAsync(patientId);
        var appointmentsTask = _appointmentService.GetAppointmentsAsync(patientId);
        var billingTask = _billingService.GetBillingDetailsAsync(patientId);

        await Task.WhenAll(patientTask, medicalHistoryTask, appointmentsTask, billingTask);
        var patient = await patientTask;
        if(patient is null) return NotFound();
               
        var appointments = await appointmentsTask;

        var billingDetails = await billingTask;
        var medicalHistory = await medicalHistoryTask;
        var dashboardData  = new PatientDashboard
        {
            Patient = patient,
            MedicalHistory = medicalHistory,
            Appointments = appointments,
            BillingDetails = billingDetails
        };

        return Ok(dashboardData );
    }
}