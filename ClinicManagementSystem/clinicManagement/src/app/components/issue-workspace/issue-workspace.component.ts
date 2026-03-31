import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PharmacistService } from '../../services/pharmacist.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-issue-workspace',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './issue-workspace.component.html',
  styleUrl: './issue-workspace.component.css'
})
export class IssueWorkspaceComponent implements OnInit {
  consultationId: number = 0;
  patientName: string = '';
  medicines: any[] = [];
  pharmacyUserId: number = 0;
  loading: boolean = true;
  error: string | null = null;
  allIssued: boolean = false;
  generatedBill: any = null;
  generatingBill: boolean = false;
  sendingSms: boolean = false;
  today: Date = new Date();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pharmacistService: PharmacistService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.pharmacyUserId = user.userId;
    }

    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.consultationId = +idParam;
        this.loadConsultationData();
      }
    });
  }

  loadConsultationData(): void {
    this.loading = true;

    // First get patient name for header
    this.pharmacistService.getPatientName(this.consultationId).subscribe(data => {
      this.patientName = data.patientName;
    });

    // Check if bill already exists for this consultation
    this.pharmacistService.billExists(this.consultationId).subscribe(exists => {
      if (exists) {
        this.pharmacistService.getBillAmount(this.consultationId).subscribe(amount => {
          this.generatedBill = { totalAmount: amount, consultationId: this.consultationId };
          this.allIssued = true;
        });
      }
    });

    // Then get medicines queue to filter for this consultation
    this.pharmacistService.getPendingMedicines().subscribe({
      next: (data) => {
        const currentApt = data.find((d: any) => d.consultationId === this.consultationId);
        if (currentApt && currentApt.medicines) {
          this.medicines = currentApt.medicines;
          this.checkIfAllIssued();
        }
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load medicines from queue.';
        this.loading = false;
      }
    });
  }

  checkIfAllIssued(): void {
    if (this.medicines.length > 0) {
      this.allIssued = this.medicines.every(m => m.statusId === 7);
    }
  }

  issueMedicine(patientMedicineId: number): void {
    const medIndex = this.medicines.findIndex(m => m.patientMedicineId === patientMedicineId);
    if (medIndex !== -1) {
      this.medicines[medIndex]._issuing = true;
    }

    this.pharmacistService.issueMedicine(patientMedicineId, this.pharmacyUserId).subscribe({
      next: (issuedMed) => {
        if (medIndex !== -1) {
          this.medicines[medIndex].statusId = 7;
          this.medicines[medIndex].statusName = 'Issued';
          this.medicines[medIndex]._issuing = false;
          this.medicines[medIndex].quantity = (this.medicines[medIndex].frequency * this.medicines[medIndex].durationDays);
          this.checkIfAllIssued();
        }
      },
      error: (err) => {
        console.error('Failed to issue:', err);
        this.showAlert('Error', err.error || 'Failed to issue medicine. Check stock.');
        if (medIndex !== -1) {
          this.medicines[medIndex]._issuing = false;
        }
      }
    });
  }

  sendSMS(): void {
    if (!this.generatedBill || this.sendingSms) return;

    this.sendingSms = true;
    this.pharmacistService.sendBillSms(this.consultationId).subscribe({
      next: () => {
        this.sendingSms = false;
        this.showAlert('SMS Sent', `A digital invoice link has been transmitted to ${this.patientName}'s registered contact.`);
      },
      error: (err) => {
        console.error('Failed to send SMS:', err);
        // Even if it fails, many SMS services are async, but we'll show error if API rejects
        this.showAlert('Communication Sync Error', 'Primary SMS gateway is currently offline. Please provide a physical receipt.');
        this.sendingSms = false;
      }
    });
  }

  generateBill(): void {
    if (!this.allIssued || this.generatingBill) return;

    this.generatingBill = true;
    this.pharmacistService.generateMedicineBill(this.consultationId).subscribe({
      next: (bill) => {
        this.generatedBill = bill;
        this.generatingBill = false;
        this.showAlert('Success', `Bill generated successfully! Total Amount: $${bill.totalAmount}`);
      },
      error: (err) => {
        console.error('Failed to generate bill:', err);
        this.showAlert('Error', 'Failed to generate bill. Please try again.');
        this.generatingBill = false;
      }
    });
  }

  printBill(): void {
    window.print();
  }

  showAlert(title: string, message: string): void {
    const overlay = document.createElement('div');
    overlay.className = 'custom-modal-overlay animate-fade-in';

    const modal = document.createElement('div');
    modal.className = `custom-modal glass-panel ${title === 'Error' ? 'modal-error' : ''}`;

    modal.innerHTML = `
      <div class="modal-header">
        <h3 style="color: ${title === 'Error' ? '#ef4444' : 'var(--color-primary)'}">${title}</h3>
      </div>
      <div class="modal-body">
        <p>${message}</p>
      </div>
      <div class="modal-footer" style="margin-top: 1.5rem; text-align: right;">
        <button class="btn btn-primary premium-btn" id="closeModalBtn">OK</button>
      </div>
    `;

    overlay.appendChild(modal);
    document.body.appendChild(overlay);

    document.getElementById('closeModalBtn')?.addEventListener('click', () => {
      document.body.removeChild(overlay);
    });
  }

  goBack(): void {
    this.router.navigate(['/pharmacist/pending']);
  }
}
