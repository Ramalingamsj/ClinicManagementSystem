import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Appointment, Consultation, Doctor, PatientLabTest, PatientMedicine, Medicine, LabTest } from '../models/doctor.models';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private apiUrl = `${environment.apiUrl}/Doctor`;

  constructor(private http: HttpClient) { }

  getTodayPendingPatients(doctorId: number): Observable<Appointment[]> {
    return this.http.post<Appointment[]>(`${this.apiUrl}/GetTodayPendingPatients/${doctorId}`, {});
  }

  getDoctorProfile(doctorId: number): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.apiUrl}/profile/${doctorId}`);
  }

  getDoctorHistory(doctorId: number): Observable<Consultation[]> {
    return this.http.get<Consultation[]>(`${this.apiUrl}/getDoctorHistory/${doctorId}`);
  }


  getConsultationDetails(consultationId: number): Observable<Consultation> {
    return this.http.get<Consultation>(`${this.apiUrl}/consultationDetails/${consultationId}`);
  }

  getConsultationByAppointment(appointmentId: number): Observable<Consultation> {
    return this.http.get<Consultation>(`${this.apiUrl}/GetConsultationByAppointment/${appointmentId}`);
  }

  getPatientHistory(patientId: number, doctorId: number): Observable<Consultation[]> {
    return this.http.get<Consultation[]>(`${this.apiUrl}/patientHistory/${patientId}/${doctorId}`);
  }

  getAppointmentById(appointmentId: number): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.apiUrl}/GetAppointmentById/${appointmentId}`);
  }

  addConsultation(consultation: Consultation): Observable<Consultation> {
    return this.http.post<Consultation>(`${this.apiUrl}/AddConsultation`, consultation);
  }

  addMedicine(consultationId: number, medicine: PatientMedicine): Observable<PatientMedicine> {
    return this.http.post<PatientMedicine>(`${this.apiUrl}/AddMedicine/${consultationId}`, medicine);
  }

  editPrescription(patientMedicineId: number, medicine: PatientMedicine): Observable<PatientMedicine> {
    return this.http.put<PatientMedicine>(`${this.apiUrl}/editPrescription/${patientMedicineId}`, medicine);
  }

  deletePrescription(patientMedicineId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/deletePrescription/${patientMedicineId}`);
  }

  addLabTest(consultationId: number, labTest: PatientLabTest): Observable<PatientLabTest> {
    return this.http.post<PatientLabTest>(`${this.apiUrl}/AddLabTest/${consultationId}`, labTest);
  }

  editLabTest(patientLabtestId: number, labTest: PatientLabTest): Observable<PatientLabTest> {
    return this.http.put<PatientLabTest>(`${this.apiUrl}/editLabTest/${patientLabtestId}`, labTest);
  }

  deleteLabTest(patientLabtestId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/deleteLabTest/${patientLabtestId}`);
  }

  getAllMedicines(): Observable<Medicine[]> {
    return this.http.get<Medicine[]>(`${this.apiUrl}/GetAllMedicines`);
  }

  getAllLabTests(): Observable<LabTest[]> {
    return this.http.get<LabTest[]>(`${this.apiUrl}/GetAllLabTests`);
  }
}
