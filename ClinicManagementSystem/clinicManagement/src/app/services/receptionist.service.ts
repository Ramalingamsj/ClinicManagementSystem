import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReceptionistService {
  private apiUrl = `${environment.apiUrl}/Receptionist`;

  constructor(private http: HttpClient) {}

  // Patient Management
  getAllPatients(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/patients`);
  }

  searchPatients(searchValue: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/patients/search?searchValue=${searchValue}`);
  }

  getPatientById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/patients/${id}`);
  }

  addPatient(patient: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/patients`, patient);
  }

  updatePatient(id: number, patient: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/patients/${id}`, patient);
  }

  // Appointment Booking
  getDoctors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/doctors`);
  }

  getAvailableSlots(doctorId: number, date: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/slots?doctorId=${doctorId}&date=${date}`);
  }

  bookAppointment(bookingData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/book`, bookingData);
  }

  // Billing
  checkBillExists(appointmentId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/bill/exists/${appointmentId}`);
  }

  getBillDetails(appointmentId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/bill/details/${appointmentId}`);
  }

  generateBill(billData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/bill`, billData);
  }

  getAppointmentHistory(id: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history/${id}`);
  }
}
