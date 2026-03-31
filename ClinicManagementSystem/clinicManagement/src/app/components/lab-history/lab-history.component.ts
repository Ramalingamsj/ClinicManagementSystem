import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LabTechnicianService } from '../../services/lab-technician.service';

@Component({
  selector: 'app-lab-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lab-history.component.html',
  styleUrl: './lab-history.component.css'
})
export class LabHistoryComponent implements OnInit {
  bills: any[] = [];
  filteredBills: any[] = [];
  searchTerm: string = '';
  loading: boolean = true;
  error: string | null = null;
  
  // UI State
  toasts: any[] = [];
  sendingSmsMap: { [key: number]: boolean } = {};
  selectedRecord: any = null;
  today: Date = new Date();

  constructor(
    private labService: LabTechnicianService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchBills();
  }

  showToast(message: string, type: 'success' | 'warning' | 'error' = 'success'): void {
    const id = Date.now();
    this.toasts.push({ id, message, type });
    setTimeout(() => {
      this.toasts = this.toasts.filter(t => t.id !== id);
    }, 4000);
  }

  removeToast(id: number): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  fetchBills(): void {
    this.loading = true;
    this.labService.getBills().subscribe({
      next: (data) => {
        this.bills = data || [];
        this.filteredBills = [...this.bills];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to fetch lab bills:', err);
        this.error = 'Failed to load billing history. Please try again later.';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredBills = [...this.bills];
      return;
    }

    this.filteredBills = this.bills.filter(bill => 
      bill.patientName?.toLowerCase().includes(term) ||
      bill.billId.toString().includes(term) ||
      bill.consultationId?.toString().includes(term) ||
      bill.labTests?.some((t: any) => t.testName.toLowerCase().includes(term))
    );
  }

  printBill(record: any): void {
    this.selectedRecord = record;
    this.showToast('Preparing laboratory invoice for download...', 'success');
    
    // Small delay to allow Angular to render the print-section
    setTimeout(() => {
      const printContents = document.getElementById('print-section')?.innerHTML;
      if (!printContents) {
        this.showToast('Failed to generate print layout.', 'error');
        return;
      }

      const originalContents = document.body.innerHTML;
      document.body.innerHTML = printContents;
      
      window.print();
      
      // Reload the page to restore Angular state after printing
      // This is a common workaround for window.print() in SPA
      location.reload(); 
    }, 500);
  }

  sendSms(record: any): void {
    const consultationId = record.consultationId;
    this.sendingSmsMap[consultationId] = true;

    const message = `Hello ${record.patientName}, your Lab Report for Bill #${record.billId} is ready. Total: ₹${record.totalAmount}. Thank you for visiting ClinicSys.`;
    
    // In a real app, we'd get the actual phone number. Using dummy for simulation.
    const dummyPhone = "+91 98765 43210"; 

    this.labService.sendSMS(consultationId, dummyPhone, message).subscribe({
      next: (res) => {
        this.sendingSmsMap[consultationId] = false;
        if (res.success) {
          this.showToast(`Notification dispatched to ${record.patientName} safely.`, 'success');
        } else {
          this.showToast('SMS Service currently unavailable.', 'warning');
        }
      },
      error: (err) => {
        this.sendingSmsMap[consultationId] = false;
        console.error('SMS Error:', err);
        this.showToast('Clinical notification failure.', 'error');
      }
    });
  }

  viewDetails(consultationId: number): void {
    this.router.navigate(['/labtech/test', consultationId]);
  }
}
