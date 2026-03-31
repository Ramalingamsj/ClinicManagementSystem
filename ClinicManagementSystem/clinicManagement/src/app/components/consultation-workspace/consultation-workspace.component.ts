import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { DoctorService } from '../../services/doctor.service';
import { AuthService } from '../../services/auth.service';
import { Consultation, PatientMedicine, PatientLabTest, Medicine, LabTest } from '../../models/doctor.models';

@Component({
  selector: 'app-consultation-workspace',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './consultation-workspace.component.html',
  styleUrl: './consultation-workspace.component.css'
})
export class ConsultationWorkspaceComponent implements OnInit {
  @ViewChild('medicineForm') medicineForm!: NgForm;
  @ViewChild('labTestForm') labTestForm!: NgForm;

  appointmentId: number = 0;
  consultationId: number | null = null;
  consultationData: Consultation | null = null;

  medicines: Medicine[] = [];
  labTests: LabTest[] = [];
  patientHistory: Consultation[] = [];
  patientId: number | null = null;
  doctorId: number | null = null;
  patientName: string = '';
  patientAge: string = '';
  patientGender: string = '';
  loadingHistory: boolean = false;

  // Template Form Data Objects
  noteData = {
    symptoms: '',
    diagnosis: '',
    notes: ''
  };

  medicineData = {
    medicineId: null as any,
    dosage: '' as any,
    duration: null as any,
    notes: ''
  };

  labTestData = {
    labTestId: null as any,
    testNotes: ''
  };

  // Dropdown States
  showMedDropdown = false;
  medSearchTerm = '';
  showLabDropdown = false;
  labSearchTerm = '';

  get filteredMedicines() {
    if (!this.medSearchTerm) return this.medicines;
    return this.medicines.filter(m =>
      ((m.medicineName || m.name) ?? '').toLowerCase().includes(this.medSearchTerm.toLowerCase())
    );
  }

  get filteredLabTests() {
    if (!this.labSearchTerm) return this.labTests;
    return this.labTests.filter(t =>
      ((t.testName || t.name) ?? '').toLowerCase().includes(this.labSearchTerm.toLowerCase())
    );
  }

  getSelectedMedicineName(): string {
    const med = this.medicines.find(m => (m.medicineId || (m as any).medicineid || m.id) === this.medicineData.medicineId);
    return med ? (med.medicineName || med.name || 'Unknown') : 'Select Medicine';
  }

  getSelectedLabTestName(): string {
    const test = this.labTests.find(t => (t.labTestId || t.labtestId || (t as any).labTestid || t.id) === this.labTestData.labTestId);
    return test ? (test.testName || test.name || 'Unknown') : 'Select Lab Test';
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.custom-dropdown')) {
      this.showMedDropdown = false;
      this.showLabDropdown = false;
    }
  }

  selectMedicine(med: any) {
    this.medicineData.medicineId = med.medicineId || (med as any).medicineid || med.id;
    this.showMedDropdown = false;
    this.medSearchTerm = '';
  }

  selectLabTest(test: any) {
    this.labTestData.labTestId = test.labTestId || test.labtestId || (test as any).labTestid || test.id;
    this.showLabDropdown = false;
    this.labSearchTerm = '';
  }

  loading: boolean = true;
  saving: boolean = false;
  activeTab: 'notes' | 'medicines' | 'labtests' | 'history' = 'notes';

  // Custom Modal State
  showModal: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  modalType: 'success' | 'error' | 'warning' | '' = '';
  modalIcon: string = 'fa-info-circle';
  isConfirmMode: boolean = false;
  confirmCallback: (() => void) | null = null;

  constructor(
    private route: ActivatedRoute,
    private doctorService: DoctorService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.doctorId = user.userId;
    }

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.appointmentId = parseInt(id, 10);
        this.loadConsultationState();
      }
    });

    this.doctorService.getAllMedicines().subscribe({
      next: (res) => { this.medicines = res; },
      error: (err) => { console.error('Failed to load medicines', err); }
    });
    this.doctorService.getAllLabTests().subscribe({
      next: (res) => { this.labTests = res; },
      error: (err) => { console.error('Failed to load lab tests', err); }
    });
  }

  loadConsultationState() {
    this.loading = true;

    // Fetch appointment to get PatientId
    this.doctorService.getAppointmentById(this.appointmentId).subscribe({
      next: (app) => {
        this.patientId = app.patientId;
        this.patientName = app.patient?.patientName || 'Patient';
        this.patientGender = app.patient?.gender || '';
        if (app.patient?.dob) {
          const birthDate = new Date(app.patient.dob);
          const today = new Date();
          let age = today.getFullYear() - birthDate.getFullYear();
          const m = today.getMonth() - birthDate.getMonth();
          if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
            age--;
          }
          this.patientAge = age > 0 ? `${age} yrs` : 'Newborn';
        }
      },
      error: (err) => console.error('Failed to load appointment details', err)
    });

    // Attempt to load existing consultation for this appointment
    this.doctorService.getConsultationByAppointment(this.appointmentId).subscribe({
      next: (data) => {
        // A consultation already exists!
        this.consultationData = data;
        this.consultationId = data.consultationId || null;

        // Populate form data
        this.noteData = {
          symptoms: data.symptoms || '',
          diagnosis: data.diagnosis || '',
          notes: data.doctorNotes || ''
        };
        this.loading = false;
      },
      error: () => {
        // No existing consultation found (404), allow new entry
        this.consultationData = {
          appointmentId: this.appointmentId,
          patientMedicines: [],
          patientLabTests: []
        };
        this.loading = false;
      }
    });
  }

  loadPatientHistory() {
    if (!this.patientId || !this.doctorId) {
      console.warn('PatientId or DoctorId missing', { patientId: this.patientId, doctorId: this.doctorId });
      return;
    }
    this.loadingHistory = true;
    this.doctorService.getPatientHistory(this.patientId, this.doctorId).subscribe({
      next: (res) => {
        // Filter out current consultation
        this.patientHistory = res.filter(c => c.consultationId !== this.consultationId);
        this.loadingHistory = false;
      },
      error: (err) => {
        console.error('Failed to load history', err);
        this.loadingHistory = false;
      }
    });
  }

  onTabChange(tab: 'notes' | 'medicines' | 'labtests' | 'history') {
    this.activeTab = tab;
    if (tab === 'history') {
      this.loadPatientHistory();
    }
  }

  saveConsultation(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    // Space/Whitespace only validation
    if (!this.noteData.symptoms.trim() || !this.noteData.diagnosis.trim()) {
      this.showAlert('Invalid Input', 'Symptoms and Diagnosis cannot be empty or contain only spaces.', 'error');
      return;
    }

    this.saving = true;

    const payload: Consultation = {
      ...this.consultationData,
      consultationId: (this.consultationId ?? 0) as any,
      appointmentId: this.appointmentId,
      symptoms: this.noteData.symptoms,
      diagnosis: this.noteData.diagnosis,
      doctorNotes: this.noteData.notes,
      createdAt: new Date().toISOString()
    };

    this.doctorService.addConsultation(payload).subscribe({
      next: (res) => {
        this.consultationData = res;
        this.consultationId = res.consultationId || 101; // store the updated ID
        this.saving = false;
        this.showAlert('Success', 'Consultation saved successfully!', 'success');
      },
      error: (err) => {
        console.error('Failed to save consultation notes', err);
        this.saving = false;
        this.showAlert('Error', 'Failed to save notes. Ensure backend API is running.', 'error');
      }
    });
  }

  addMedicine(form: NgForm) {
    if (this.medicineData.medicineId == null || form.invalid) {
      form.control.markAllAsTouched();
      return;
    }
    if (!this.consultationId) {
      this.showAlert('Warning', 'Please save the consultation notes first before adding medicines.', 'error');
      return;
    }

    // The Select dropdown is bound via [ngValue] to the integers: 3, 2, 1
    const frequencyValue = typeof this.medicineData.dosage === 'number'
      ? this.medicineData.dosage
      : parseInt(this.medicineData.dosage, 10) || 0;

    // Map the form data to match the C# PatientMedicine model expected property names
    const payload: PatientMedicine = {
      medicineId: this.medicineData.medicineId,
      frequency: frequencyValue, // Map for backend Quantity calc
      durationDays: (this.medicineData.duration as any) || 0, // Map for backend Quantity calc
      notes: this.medicineData.notes
    };

    // Duplicate Check
    // Duplicate Check
    const medId = payload.medicineId;
    const isDuplicate = this.consultationData?.patientMedicines?.some((m: any) =>
      (m.medicineId ?? m.medicineid ?? m.id) == medId
    );
    if (isDuplicate) {
      this.showAlert('Warning', 'This medicine is already added to the prescription.', 'error');
      return;
    }

    this.doctorService.addMedicine(this.consultationId, payload).subscribe({
      next: (res: any) => {
        // Enrich the response with the medicine name for immediate display
        const resMedId = res.medicineId ?? res.medicineid ?? res.id;
        const selectedMed = this.medicines.find(m => (m.medicineId ?? m.id) == resMedId);
        if (selectedMed) {
          res.medicine = selectedMed;
        }

        this.consultationData?.patientMedicines?.push(res);
        this.resetMedicineForm();
        this.saving = false;
        this.showAlert('Success', 'Medicine added successfully!', 'success');
      },
      error: (err) => {
        console.error('Failed to add medicine', err);
        this.showAlert('Error', 'Failed to save medicine. Check console/network logs or ensure consultation is saved first.', 'error');
        this.saving = false;
      }
    });
  }

  resetMedicineForm() {
    this.medicineData.medicineId = null as any;
    this.medicineData.dosage = '' as any;
    this.medicineData.duration = null as any;
    this.medicineData.notes = '';
    this.medSearchTerm = '';
    this.showMedDropdown = false;
    if (this.medicineForm) {
      this.medicineForm.resetForm(this.medicineData);
    }
  }

  deleteMedicine(medId: number | undefined) {
    if (!medId) return;
    
    this.showConfirm(
      'Remove Prescription',
      'Are you sure you want to remove this medicine from the patient\'s prescription?',
      'warning',
      () => {
        this.doctorService.deletePrescription(medId).subscribe({
          next: () => {
            if (this.consultationData?.patientMedicines) {
              this.consultationData.patientMedicines = this.consultationData.patientMedicines.filter(m => m.patientMedicineId !== medId);
            }
            this.showAlert('Deleted', 'Medicine removed from prescription.', 'success');
          },
          error: (err) => {
            console.error('Failed to delete medicine', err);
            this.showAlert('Error', 'Failed to remove medicine. It might have been already issued.', 'error');
          }
        });
      }
    );
  }

  addLabTest(form: NgForm) {
    if (this.labTestData.labTestId == null || form.invalid) {
      form.control.markAllAsTouched();
      return;
    }
    if (!this.consultationId) {
      this.showAlert('Warning', 'Please save consultation notes first.', 'error');
      return;
    }

    this.saving = true;
    const payload = this.labTestData as unknown as PatientLabTest;

    // Duplicate Check
    // Duplicate Check
    const testId = this.labTestData.labTestId;
    const isDuplicate = this.consultationData?.patientLabTests?.some((t: any) =>
      (t.labTestId ?? t.labtestId ?? t.id) == testId
    );
    if (isDuplicate) {
      this.showAlert('Warning', 'This lab test is already ordered for this patient.', 'error');
      this.saving = false;
      return;
    }

    this.doctorService.addLabTest(this.consultationId, payload).subscribe({
      next: (res: any) => {
        // Enrich the response with the lab test name for immediate display
        const resTestId = res.labTestId ?? res.labtestId ?? res.id;
        const selectedTest = this.labTests.find(t => (t.labTestId ?? t.labtestId ?? t.id) == resTestId);
        if (selectedTest) {
          res.labtest = selectedTest;
        }

        this.consultationData?.patientLabTests?.push(res);
        this.resetLabTestForm();
        this.saving = false;
        this.showAlert('Success', 'Lab test ordered successfully!', 'success');
      },
      error: (err) => {
        console.error('Failed to add lab test', err);
        this.showAlert('Error', 'Failed to save lab test. Check network logs.', 'error');
        this.saving = false;
      }
    });
  }

  resetLabTestForm() {
    this.labTestData.labTestId = null as any;
    this.labTestData.testNotes = '';
    this.labSearchTerm = '';
    this.showLabDropdown = false;
    if (this.labTestForm) {
      this.labTestForm.resetForm(this.labTestData);
    }
  }

  deleteLabTest(testId: number | undefined) {
    if (!testId) return;
    
    this.showConfirm(
      'Cancel Lab Order',
      'Are you sure you want to remove this lab test order?',
      'warning',
      () => {
        this.doctorService.deleteLabTest(testId).subscribe({
          next: () => {
            if (this.consultationData?.patientLabTests) {
              this.consultationData.patientLabTests = this.consultationData.patientLabTests.filter(t => t.patientLabtestId !== testId);
            }
            this.showAlert('Deleted', 'Lab test removed.', 'success');
          },
          error: (err) => {
            console.error('Failed to delete lab test', err);
            this.showAlert('Error', 'Failed to remove lab test. Results might already exist.', 'error');
          }
        });
      }
    );
  }

  finishConsultation() {
    this.router.navigate(['/doctor/pending']);
  }

  showAlert(title: string, message: string, type: 'success' | 'error' | 'warning' | '' = '') {
    this.modalTitle = title;
    this.modalMessage = message;
    this.modalType = type;
    this.modalIcon = type === 'success' ? 'fa-check-circle' : (type === 'error' ? 'fa-times-circle' : 'fa-info-circle');
    this.isConfirmMode = false;
    this.confirmCallback = null;
    this.showModal = true;
  }

  showConfirm(title: string, message: string, type: 'warning' | 'error' = 'warning', onConfirm: () => void) {
    this.modalTitle = title;
    this.modalMessage = message;
    this.modalType = type;
    this.modalIcon = 'fa-exclamation-triangle';
    this.isConfirmMode = true;
    this.confirmCallback = onConfirm;
    this.showModal = true;
  }

  onModalConfirm() {
    if (this.confirmCallback) {
      this.confirmCallback();
    }
    this.closeModal();
  }

  closeModal() {
    this.showModal = false;
    this.confirmCallback = null;
  }

  // Helper to parse multi-parameter results for better display
  parseResult(result: string): { key: string, value: string }[] {
    if (!result || !result.includes('|')) return [];
    return result.split('|').map(part => {
      const parts = part.split(':');
      if (parts.length < 2) return { key: 'Result', value: part.trim() };
      return {
        key: parts[0].trim(),
        value: parts.slice(1).join(':').trim()
      };
    });
  }
}
