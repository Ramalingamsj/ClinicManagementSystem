import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Bill } from '../models/doctor.models';

export interface ResultDto {
  result: string;
}

@Injectable({
  providedIn: 'root'
})
export class LabTechnicianService {
  private apiUrl = `${environment.apiUrl}/LabTechnician`;

  constructor(private http: HttpClient) { }

  getPendingTests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pendingTests`);
  }

  updateResult(id: number, userId: number, resultDto: ResultDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/updateResult/${id}/${userId}`, resultDto);
  }

  generateLabBill(consultationId: number): Observable<Bill> {
    return this.http.post<Bill>(`${this.apiUrl}/generateBill/${consultationId}`, {});
  }

  getConsultationIdFromLabTest(id: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/consultation/${id}`);
  }

  getBills(): Observable<Bill[]> {
    return this.http.get<Bill[]>(`${this.apiUrl}/bills`);
  }

  billExists(consultationId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/billExists/${consultationId}`);
  }

  getBillAmount(consultationId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/billAmount/${consultationId}`);
  }

  sendSMS(consultationId: number, phoneNumber: string, message: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/sendSMS`, {
      consultationId,
      phoneNumber,
      message
    });
  }

  // Lab Test Management
  getLabTests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/labTests`);
  }

  addLabTest(test: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/addLabTest`, test);
  }

  updateLabPrice(id: number, price: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/updatePrice/${id}/${price}`, {});
  }
}
