import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReceptionistService } from '../../services/receptionist.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-receptionist-workstation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receptionist-workstation.component.html',
  styleUrls: ['./receptionist-workstation.component.css']
})
export class ReceptionistWorkstationComponent implements OnInit {
  activeTab: 'patients' | 'booking' | 'billing' = 'patients';

  // Patient State
  patients: any[] = [];
  searchQuery: string = '';
  loadingPatients: boolean = false;
  showPatientModal: boolean = false;
  editingPatient: any = null;
  newPatient: any = {
    patientName: '',
    dob: null,
    gender: 'Male',
    contact: '',
    email: '',
    address: ''
  };

  // Booking State
  doctors: any[] = [];
  slots: any[] = [];
  selectedDoctorId: number = 0;
  selectedDate: string = new Date().toISOString().split('T')[0];
  selectedSlotId: number = 0;
  bookingPatientId: number = 0;
  bookingPatientName: string = '';
  isBooking: boolean = false;

  // Custom Searchable Dropdown State
  showDoctorDropdown: boolean = false;
  doctorSearchTerm: string = '';

  get filteredDoctors() {
    if (!this.doctorSearchTerm) return this.doctors;
    return this.doctors.filter(d =>
      (d.fullName || '').toLowerCase().includes(this.doctorSearchTerm.toLowerCase()) ||
      (d.specialization || '').toLowerCase().includes(this.doctorSearchTerm.toLowerCase())
    );
  }

  selectDoctor(doctor: any) {
    this.selectedDoctorId = doctor.doctorId;
    this.showDoctorDropdown = false;
    this.doctorSearchTerm = '';
    this.onDoctorOrDateChange();
  }

  getSelectedDoctorName(): string {
    const d = this.doctors.find(doc => doc.doctorId === this.selectedDoctorId);
    return d ? `Dr. ${d.fullName} | ${d.specialization}` : 'Select Practitioner...';
  }

  // Billing State
  activeDate: Date = new Date();
  billingAppointmentId: string = '';
  billDetails: any = null;
  checkingBill: boolean = false;
  billExists: boolean = false;
  generatingBill: boolean = false;

  // Elite Modal State
  showModal: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  modalType: 'success' | 'warning' | 'error' | '' = '';
  modalIcon: string = '';

  constructor(
    private receptionService: ReceptionistService,
    private authService: AuthService,
    private router: Router
  ) { }

  get currentUser(): any {
    return this.authService.currentUserValue;
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadDoctors();
  }

  // Tab Navigation
  setTab(tab: 'patients' | 'booking' | 'billing'): void {
    this.activeTab = tab;
    if (tab === 'patients') this.loadPatients();
  }

  // --- Patient Management ---
  loadPatients(): void {
    this.loadingPatients = true;
    this.receptionService.getAllPatients().subscribe({
      next: (data) => {
        this.patients = data;
        this.loadingPatients = false;
      },
      error: () => this.loadingPatients = false
    });
  }


  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.loadPatients();
      return;
    }
    this.receptionService.searchPatients(this.searchQuery).subscribe(data => {
      this.patients = data;
    });
  }

  openAddPatient(): void {
    this.editingPatient = null;
    this.newPatient = { patientName: '', dob: null, gender: 'Male', contact: '', email: '', address: '' };
    this.showPatientModal = true;
  }

  openEditPatient(patient: any): void {
    this.editingPatient = { ...patient };
    this.showPatientModal = true;
  }

  savePatient(form: import('@angular/forms').NgForm): void {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    const patientData = this.editingPatient || this.newPatient;

    // Performance whitespace/trim validation parity with Consultation
    if (!patientData.patientName.trim() || !patientData.contact.trim()) {
      this.showAlert('Invalid Input', 'Patient Name and Contact cannot be empty or contain only spaces.');
      return;
    }

    if (this.editingPatient) {
      this.receptionService.updatePatient(patientData.patientId, patientData).subscribe({
        next: () => {
          this.showAlert('Success', 'Patient profile updated successfully');
          this.showPatientModal = false;
          this.loadPatients();
        },
        error: (err) => this.showAlert('Update Failed', err.error?.message || 'Check connection')
      });
    } else {
      this.receptionService.addPatient(patientData).subscribe({
        next: () => {
          this.showAlert('Success', 'New patient registered successfully');
          this.showPatientModal = false;
          this.loadPatients();
        },
        error: (err) => this.showAlert('Registration Failed', err.error?.message || 'Check connection')
      });
    }
  }

  // --- Appointment Booking ---
  loadDoctors(): void {
    this.receptionService.getDoctors().subscribe(data => {
      this.doctors = data;
    });
  }

  initiateBooking(patient: any): void {
    this.bookingPatientId = patient.patientId;
    this.bookingPatientName = patient.patientName;
    this.setTab('booking');
  }

  onDoctorOrDateChange(): void {
    if (this.selectedDoctorId && this.selectedDate) {
      this.receptionService.getAvailableSlots(this.selectedDoctorId, this.selectedDate).subscribe(data => {
        const today = new Date().toISOString().split('T')[0];
        if (this.selectedDate === today) {
          const now = new Date();
          const currentHour = now.getHours();
          const currentMinute = now.getMinutes();

          this.slots = data.filter(s => {
            const [hour, minute] = s.slotTime.split(':').map(Number);
            if (hour > currentHour) return true;
            if (hour === currentHour && minute > currentMinute) return true;
            return false;
          });
        } else {
          this.slots = data;
        }
      });
    }
  }

  confirmBooking(): void {
    if (!this.bookingPatientId || !this.selectedDoctorId || !this.selectedSlotId) {
      this.showAlert('Warning', 'Please select a patient, doctor, and time slot.');
      return;
    }

    this.isBooking = true;
    const bookingData = {
      patientId: this.bookingPatientId,
      doctorId: this.selectedDoctorId,
      slotId: this.selectedSlotId,
      appointmentDate: this.selectedDate,
      createdBy: this.authService.currentUserValue?.userId || 1
    };

    this.receptionService.bookAppointment(bookingData).subscribe({
      next: (res) => {
        this.showAlert('Appointment Confirmed', `Booking ID: #${res.appointmentId}\n${res.message}`);
        this.isBooking = false;
        this.resetBooking();
        this.router.navigate(['/receptionist/history']); // Navigate to the new audit history module
      },
      error: (err) => {
        this.showAlert('Booking Failed', err.error.message || 'Slot might have been taken.');
        this.isBooking = false;
      }
    });
  }

  resetBooking(): void {
    this.selectedDoctorId = 0;
    this.selectedSlotId = 0;
    this.slots = [];
    this.bookingPatientId = 0;
    this.bookingPatientName = '';
  }

  // --- Billing ---
  searchBill(): void {
    const id = parseInt(this.billingAppointmentId);
    if (isNaN(id)) return;

    this.checkingBill = true;
    this.receptionService.checkBillExists(id).subscribe(existsRes => {
      this.billExists = existsRes.exists;
      this.receptionService.getBillDetails(id).subscribe(details => {
        this.billDetails = details;
        this.checkingBill = false;
        if (!details.patientName) {
          this.showAlert('Not Found', 'No appointment found with this ID.');
        }
      });
    });
  }

  generateBill(): void {
    if (this.billExists) {
      this.showAlert('Already Billed', 'A bill already exists for this appointment.');
      return;
    }

    this.generatingBill = true;
    const billData = {
      appointmentId: this.billDetails.appointmentId,
      totalAmount: this.billDetails.consultationFee,
      statusId: 6 // Billed
    };

    this.receptionService.generateBill(billData).subscribe(() => {
      this.showAlert('Success', 'Consultation bill generated successfully.');
      this.generatingBill = false;
      this.billExists = true;
    });
  }

  printInvoice(): void {
    window.print();
  }

  // --- Elite Modal Logic ---
  showAlert(title: string, message: string): void {
    const isError = title.toLowerCase().includes('error') || title.toLowerCase().includes('failed');
    this.modalTitle = title;
    this.modalMessage = message;
    this.modalType = isError ? 'error' : 'success';
    this.modalIcon = isError ? 'fa-exclamation-triangle' : 'fa-check-circle';
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }
}
