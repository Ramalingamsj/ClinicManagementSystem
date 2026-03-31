import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PharmacistService } from '../../services/pharmacist.service';
import { Bill } from '../../models/doctor.models';

@Component({
  selector: 'app-pharmacist-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pharmacist-history.component.html',
  styleUrl: './pharmacist-history.component.css'
})
export class PharmacistHistoryComponent implements OnInit {
  medicineHistory: any[] = [];
  loadingHistory: boolean = true;
  historyError: string | null = null;
  searchTerm: string = '';

  constructor(private pharmacistService: PharmacistService) {}

  ngOnInit(): void {
    this.fetchHistory();
  }

  fetchHistory(): void {
    this.loadingHistory = true;
    this.pharmacistService.getIssuedMedicinesHistory().subscribe({
      next: (data) => {
        // Sort consultations by consultationId descending
        this.medicineHistory = (data || []).sort((a: any, b: any) => (b.consultationId || 0) - (a.consultationId || 0));
        this.loadingHistory = false;
      },
      error: (err) => {
        console.error('Failed to fetch medicine history:', err);
        this.historyError = 'Unable to load issuance history from server.';
        this.loadingHistory = false;
      }
    });
  }

  get filteredMedicineHistory(): any[] {
    if (!this.searchTerm.trim()) return this.medicineHistory;
    const term = this.searchTerm.toLowerCase();
    return this.medicineHistory.filter(c => 
      c.patientName?.toLowerCase().includes(term) || 
      c.consultationId?.toString().includes(term) ||
      c.medicines?.some((m: any) => m.medicineName?.toLowerCase().includes(term))
    );
  }

  selectedRecord: any = null;
  today: Date = new Date();
  sendingSmsMap: { [key: number]: boolean } = {};

  printBill(record: any): void {
    this.selectedRecord = record;
    // Increased timeout to ensure Angular updates the sibling print template before printing
    setTimeout(() => {
      window.print();
    }, 400);
  }

  sendSms(record: any): void {
    if (this.sendingSmsMap[record.consultationId]) return;

    this.sendingSmsMap[record.consultationId] = true;
    this.pharmacistService.sendBillSms(record.consultationId).subscribe({
      next: (res) => {
        this.sendingSmsMap[record.consultationId] = false;
        alert(`SMS Sent Successfully! Bill details dispatched to the patient's registered contact.`);
      },
      error: (err) => {
        this.sendingSmsMap[record.consultationId] = false;
        console.error('SMS Failed:', err);
        const msg = err.error?.message || 'Failed to send SMS. Please check if the patient has a valid contact number.';
        alert(msg);
      }
    });
  }
}
