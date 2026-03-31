import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PatientMedicine, Medicine, Bill, PendingPharmacistQueue } from '../models/doctor.models';

@Injectable({
  providedIn: 'root'
})
export class PharmacistService {
  private apiUrl = `${environment.apiUrl}/Pharmacist`;

  constructor(private http: HttpClient) { }

  getPendingMedicines(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pendingMedicines`);
  }

  hasPendingMedicines(consultationId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/hasPending/${consultationId}`);
  }

  getMedicines(): Observable<Medicine[]> {
    return this.http.get<Medicine[]>(`${this.apiUrl}/medicines`);
  }

  issueMedicine(patientMedicineId: number, pharmacyUserId: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/issueMedicine/${patientMedicineId}/${pharmacyUserId}`, {});
  }

  updateStock(id: number, stock: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/updateStock/${id}/${stock}`, {});
  }

  generateMedicineBill(consultationId: number): Observable<Bill> {
    return this.http.post<Bill>(`${this.apiUrl}/generateBill/${consultationId}`, {});
  }

  getAllBills(): Observable<Bill[]> {
    return this.http.get<Bill[]>(`${this.apiUrl}/bills`);
  }

  getIssuedMedicinesHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/issuedHistory`);
  }

  getBillAmount(consultationId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/billAmount/${consultationId}`);
  }

  billExists(consultationId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/billExists/${consultationId}`);
  }

  getPatientName(consultationId: number): Observable<{ patientName: string }> {
    return this.http.get<{ patientName: string }>(`${this.apiUrl}/patientName/${consultationId}`);
  }

  getTodaysConsultations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/todaysConsultations`);
  }

  sendBillSms(consultationId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/sendBillSms/${consultationId}`, {});
  }

  addMedicine(medicine: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/addMedicine`, medicine);
  }
}
