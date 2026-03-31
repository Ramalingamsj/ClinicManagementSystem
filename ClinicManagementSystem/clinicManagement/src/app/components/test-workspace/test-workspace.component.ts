import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LabTechnicianService } from '../../services/lab-technician.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-test-workspace',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test-workspace.component.html',
  styleUrl: './test-workspace.component.css'
})
export class TestWorkspaceComponent implements OnInit {
  consultationId: number = 0;
  patientName: string = '';
  labTests: any[] = [];
  labUserId: number = 0;
  loading: boolean = true;
  error: string | null = null;
  allCompleted: boolean = false;
  generatedBill: any = null;
  generatingBill: boolean = false;
  today: Date = new Date();
  patientPhone: string = '';
  patientId: number = 0;
  sendingSmsMap: { [key: number]: boolean } = {};

  sendingSummarySms: boolean = false;

  // Real-time medical test field schemas (Expanded for Global Clinical Standards)
  TEST_SCHEMAS: { [key: string]: string[] } = {
    'Complete Blood Count': ['Hemoglobin (g/dL)', 'WBC Count (cells/mcL)', 'RBC Count (million/mcL)', 'Platelets (mcL)', 'Hematocrit (%)', 'MCV (fL)', 'MCH (pg)'],
    'Lipid Profile': ['Total Cholesterol (mg/dL)', 'HDL Cholesterol (mg/dL)', 'LDL Cholesterol (mg/dL)', 'Triglycerides (mg/dL)', 'VLDL (mg/dL)'],
    'Urinalysis': ['Color', 'Appearance', 'Specific Gravity', 'pH Level', 'Protein (Albumin)', 'Glucose (Sugar)', 'Ketones', 'Bilirubin', 'WBC / Pus Cells'],
    'Blood Sugar': ['Fasting Sugar (mg/dL)', 'Post-Prandial (mg/dL)', 'HbA1c (%)', 'Avg Glucose (mg/dL)'],
    'Thyroid Profile': ['TSH (mIU/L)', 'Free T3 (pg/mL)', 'Free T4 (ng/dL)'],
    'Renal Function Test': ['Urea (mg/dL)', 'Creatinine (mg/dL)', 'Uric Acid (mg/dL)', 'BUN (mg/dL)'],
    'Electrolytes Panel': ['Sodium (mmol/L)', 'Potassium (mmol/L)', 'Chloride (mmol/L)', 'Calcium (mg/dL)'],
    'Liver Function Test': ['SGOT (AST)', 'SGPT (ALT)', 'Bilirubin Total', 'Bilirubin Direct', 'Alkaline Phosphatase', 'Albumin (g/dL)'],
    'COVID-19 PCR': ['Result (Positive/Negative)', 'Ct Value (ORF1ab)', 'Ct Value (N gene)', 'Interpretation'],
    'Vitamin Panel': ['Vitamin B12 (pg/mL)', 'Vitamin D (ng/mL)', 'Reference Range'],
    'ECG / Cardiology': ['Rhythm', 'Heart Rate (bpm)', 'PR Interval (ms)', 'QRS Duration (ms)', 'QT/QTc (ms)', 'Impression'],
    'Radiology / X-Ray': ['Lung Fields', 'Heart Size', 'Bony Thorax / Diaphragm', 'Costophrenic Angles', 'Impression']
  };

  GENERIC_SCHEMA = ['Finding 1', 'Finding 2', 'Finding 3'];

  // Maps PatientLabtestId -> { FieldName -> Value }
  testFieldsMap: { [key: number]: { [fieldName: string]: string } } = {};

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private labService: LabTechnicianService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.labUserId = user.userId;
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

    // Check if bill already exists for this consultation
    this.labService.billExists(this.consultationId).subscribe(exists => {
      if (exists) {
        this.labService.getBillAmount(this.consultationId).subscribe(amount => {
          this.generatedBill = { totalAmount: amount, consultationId: this.consultationId };
          this.allCompleted = true;
        });
      }
    });

    this.labService.getPendingTests().subscribe({
      next: (data) => {
        const currentApt = data.find((d: any) => d.consultationId === this.consultationId);
        if (currentApt && currentApt.labTests) {
          // Being resilient to both 'contact' and 'Contact' property casing from API
          this.patientId = currentApt.patientId || currentApt.PatientId || 0;
          this.patientName = currentApt.patientName || currentApt.PatientName || 'Unknown';
          this.patientPhone = currentApt.contact || currentApt.Contact || 'Not Provided';

          this.labTests = currentApt.labTests.map((t: any) => {
            const mappedTest = {
              ...t,
              inputResult: t.result || '', // To hold the bound form data
              _saving: false
            };

            // Initialize fields from existing result if available
            const schema = this.getTestSchema(t.testName);
            this.testFieldsMap[t.patientLabtestId] = {}; // Initialize even if no schema

            if (schema) {
              schema.forEach(field => {
                // Try to extract value from result string if it matches "Field: Value"
                // e.g. "Hb: 14.5 | WBC: 7000"
                const escapedField = field.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
                const regex = new RegExp(`${escapedField}: ([^|]*)`, 'i');
                const match = mappedTest.inputResult.match(regex);
                this.testFieldsMap[t.patientLabtestId][field] = match ? match[1].trim() : '';
              });
            }

            return mappedTest;
          });

          this.checkIfAllCompleted();
        } else {
          this.error = "No pending tests found for this consultation.";
        }
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load lab tests.';
        this.loading = false;
      }
    });
  }

  getTestSchema(testName: string): string[] | null {
    if (!testName) return null;
    const search = testName.trim().toLowerCase();

    // 1. Precise Keyword Mapping (High Priority)
    if (search.includes('hemoglobin') || search === 'hgb') return ['Hemoglobin (g/dL)'];
    if (search.includes('vitamin d')) return ['Vitamin D (ng/mL)', 'Reference Range'];
    if (search.includes('vitamin b12')) return ['Vitamin B12 (pg/mL)', 'Reference Range'];
    if (search.includes('fasting sugar') || (search.includes('sugar') && !search.includes('post'))) return ['Fasting Sugar (mg/dL)'];
    if (search.includes('hba1c')) return ['HbA1c (%)', 'Estimated Avg Glucose'];
    if (search === 'ecg' || search === 'ekg' || search.includes('electrocardiogram')) return this.TEST_SCHEMAS['ECG / Cardiology'];
    if (search.includes('x-ray') || search.includes('xray') || search.includes('imaging')) return this.TEST_SCHEMAS['Radiology / X-Ray'];

    // 2. Robust Group Matching (Standard Panels)
    const match = Object.keys(this.TEST_SCHEMAS).find(key => {
      const normalizedKey = key.toLowerCase().trim();
      return search.includes(normalizedKey) ||
        normalizedKey.includes(search) ||
        (normalizedKey === 'complete blood count' && (search === 'cbc' || search.includes('blood'))) ||
        (normalizedKey === 'covid-19 pcr' && search.includes('pcr')) ||
        (normalizedKey === 'renal function test' && (search.includes('rft') || search.includes('renal') || search.includes('kidney'))) ||
        (normalizedKey === 'liver function test' && (search.includes('lft') || search.includes('liver'))) ||
        (normalizedKey === 'electrolytes panel' && (search.includes('electrolyte') || search.includes('sodium') || search.includes('potassium'))) ||
        (normalizedKey === 'urinalysis' && (search.includes('urine') || search.includes('urinary')));
    });

    return match ? this.TEST_SCHEMAS[match] : this.GENERIC_SCHEMA;
  }

  onFieldChange(testId: number): void {
    const test = this.labTests.find(t => t.patientLabtestId === testId);
    if (!test) return;

    const schema = this.getTestSchema(test.testName) || [];
    const fields = this.testFieldsMap[testId];
    if (!fields) return;

    // Filter out empty fields and consolidate into a professional analytical string
    test.inputResult = schema
      .filter(f => fields[f] && fields[f].trim() !== '')
      .map(field => `${field}: ${fields[field]}`)
      .join(' | ');

    // Fallback if NO fields are entered
    if (!test.inputResult) {
      test.inputResult = '';
    }
  }

  isNumericField(field: string): boolean {
    if (!field) return false;
    const lower = field.toLowerCase();
    // Exclude explicitly text-based clinical fields
    if (lower.includes('result') || lower.includes('interpretation') || lower.includes('remark') || lower.includes('ketone')) return false;

    // Include measurements, units, and clinical markers
    const numericMarkers = ['(', ')', 'count', 'value', 'sugar', 'hba1c', 'tsh', 't3', 't4', 'ct', 'level', '%', 'bp', 'g/dL', 'ng/mL', 'pg/mL', 'mmol/L', 'mg/dL'];
    return numericMarkers.some(marker => lower.includes(marker.toLowerCase()));
  }

  isTestFormInvalid(test: any): boolean {
    const schema = this.getTestSchema(test.testName) || [];
    const fields = this.testFieldsMap[test.patientLabtestId];
    if (!fields) return true;

    // ALL analytical fields must be filled AND pass clinical validation patterns
    return schema.some(field => {
      const val = (fields[field] || '').toString();
      return val.trim() === '' || this.isFieldInvalid(test.patientLabtestId, field);
    });
  }

  isFieldInvalid(testId: number, field: string): boolean {
    const fields = this.testFieldsMap[testId];
    if (!fields || !fields[field]) return false;

    const val = fields[field].toString();
    if (val.trim() === '') return false;

    const isNumeric = this.isNumericField(field);

    // Precise clinical validation hierarchy
    // 1. Numeric Fields: Allow leading numbers (e.g. 14.5) but no leading space
    // 2. Text Fields: No leading space, NO leading number (Standard clinical remark)
    const pattern = isNumeric
      ? /^(?!\s)[a-zA-Z0-9\s.,?!()\-]*$/
      : /^(?!\s)(?![0-9])[a-zA-Z0-9\s.,?!()\-]*$/;

    return !pattern.test(val);
  }

  hasSpecialCharacters(testId: number): boolean {
    const fields = this.testFieldsMap[testId];
    if (!fields) return false;

    return Object.keys(fields).some(field => {
      const val = fields[field];
      if (!val) return false;

      const isNumeric = this.isNumericField(field);
      const pattern = isNumeric
        ? /^(?!\s)[a-zA-Z0-9\s.,?!()\-]*$/
        : /^(?!\s)(?![0-9])[a-zA-Z0-9\s.,?!()\-]*$/;

      return !pattern.test(val.toString());
    });
  }

  checkIfAllCompleted(): void {
    if (this.labTests.length > 0) {
      this.allCompleted = this.labTests.every(t => t.statusId === 2);
    }
  }

  saveResult(testId: number): void {
    const testIndex = this.labTests.findIndex(t => t.patientLabtestId === testId);
    if (testIndex === -1) return;

    const test = this.labTests[testIndex];
    if (!test.inputResult || test.inputResult.trim() === '' || test.inputResult.includes(': N/A')) {
      this.showAlert('Validation Error', 'Please complete all parameters before saving.');
      return;
    }

    test._saving = true;

    this.labService.updateResult(testId, this.labUserId, { result: test.inputResult }).subscribe({
      next: (updatedTest) => {
        this.labTests[testIndex].statusId = 2; // Mark as Completed locally
        this.labTests[testIndex].statusName = 'Completed';
        this.labTests[testIndex]._saving = false;

        this.checkIfAllCompleted();

        if (this.allCompleted) {
          this.showAlert('Success', 'Test result saved! All tests are now complete and the bill has been updated.');
          // Reload bill if all completed
          this.labService.getBillAmount(this.consultationId).subscribe(amount => {
            this.generatedBill = { totalAmount: amount, consultationId: this.consultationId };
          });
        }
      },
      error: (err) => {
        console.error('Failed to save result:', err);
        this.showAlert('Error', 'Failed to save test result due to a server error.');
        this.labTests[testIndex]._saving = false;
      }
    });
  }

  sendSmsForTest(test: any): void {
    // 1. Phone validation with immediate non-blocking fallback if needed
    if (!this.patientPhone || this.patientPhone === 'Not Provided') {
      this.patientPhone = '555-0199 (Test Mode)';
    }

    const formattedResult = this.formatResultForSms(test.inputResult);
    const message = `Hello ${this.patientName}, your Laboratory Result for "${test.testName}" is now available. Details: [ ${formattedResult} ]. ClinicSys Diagnostic.`;

    // 2. Immediate execution - Removed ALL confirm() popups to prevent browser blocking
    const testId = test.patientLabtestId;
    this.sendingSmsMap[testId] = true;

    this.labService.sendSMS(this.consultationId, this.patientPhone, message).subscribe({
      next: (res: any) => {
        this.sendingSmsMap[testId] = false;
        if (res.success) {
          this.showAlert('SMS Delivered', `Successfully sent to ${this.patientPhone}.\n\nTwilio Status: ${res.message || 'Queued'}`);
        } else {
          this.showAlert('Delivery Failed', `Twilio Error: ${res.message}\n\nPlease check the number: ${this.patientPhone}`);
        }
      },
      error: (err) => {
        this.sendingSmsMap[testId] = false;
        this.showAlert('System Error', `Failed to connect to SMS service.\n${err.message || 'Unknown Error'}`);
      }
    });
  }

  sendSummarySMS(): void {
    // 1. Phone validation with immediate non-blocking fallback if needed
    if (!this.patientPhone || this.patientPhone === 'Not Provided') {
      this.patientPhone = '555-0199 (Test Mode)';
    }

    const summaryParts = this.labTests
      .filter(t => t.statusId === 2)
      .map(t => `${t.testName}: ${this.formatResultForSms(t.inputResult) || 'Verified'}`);

    const resultsText = summaryParts.length > 0 ? summaryParts.join(' | ') : 'All tests complete';
    const message = `Laboratory Update for ${this.patientName}: Your complete diagnostic results are now ready. Summary: [ ${resultsText} ]. ClinicSys Diagnostics.`;

    // 2. Immediate execution - Removed ALL confirm() popups to prevent browser blocking
    this.sendingSummarySms = true;

    this.labService.sendSMS(this.consultationId, this.patientPhone, message).subscribe({
      next: (res: any) => {
        this.sendingSummarySms = false;
        if (res.success) {
          this.showAlert('Summary Sent', `All results transmitted to ${this.patientPhone}.\n\nTwilio Status: ${res.message || 'Queued'}`);
        } else {
          this.showAlert('Delivery Failed', `Twilio Error: ${res.message}\n\nPlease check the number: ${this.patientPhone}`);
        }
      },
      error: (err) => {
        this.sendingSummarySms = false;
        this.showAlert('System Error', `Failed to send summary.\n${err.message || 'Unknown Error'}`);
      }
    });
  }

  formatResultForSms(result: string): string {
    if (!result) return '';
    // Replace the internal pipe delimiter with a cleaner dash or comma-space for SMS readability
    return result.split('|').map(r => r.trim()).join(', ');
  }

  generateBill(): void {
    if (!this.allCompleted || this.generatingBill) return;

    this.generatingBill = true;
    this.labService.generateLabBill(this.consultationId).subscribe({
      next: (bill) => {
        this.generatedBill = bill;
        this.generatingBill = false;
        this.showAlert('Success', `Lab Bill generated successfully! Total Amount: $${bill.totalAmount}`);
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
    const isError = title.toLowerCase().includes('error') || title.toLowerCase().includes('failed');

    const overlay = document.createElement('div');
    overlay.className = 'custom-modal-overlay animate-fade-in';

    const modal = document.createElement('div');
    modal.className = `custom-modal ${isError ? 'modal-error' : ''}`;

    modal.innerHTML = `
      <div class="modal-header">
        <div style="display: flex; align-items: center; gap: 12px;">
          <i class="fas ${isError ? 'fa-exclamation-circle' : 'fa-check-circle'}" 
             style="font-size: 1.5rem; color: ${isError ? '#ef4444' : '#4f46e5'}"></i>
          <h3 style="color: ${isError ? '#ef4444' : '#4f46e5'}; font-size: 1.25rem; font-weight: 700; margin: 0;">${title}</h3>
        </div>
      </div>
      <div class="modal-body">
        <p style="font-size: 1rem; color: #475569; line-height: 1.6; margin: 0;">${message.replace(/\n/g, '<br>')}</p>
      </div>
      <div class="modal-footer" style="padding: 1rem 1.5rem; background: #f8fafc; text-align: right; border-top: 1px solid #e2e8f0;">
        <button class="premium-btn" id="closeModalBtn" style="min-width: 80px;">OK</button>
      </div>
    `;

    overlay.appendChild(modal);
    document.body.appendChild(overlay);

    document.getElementById('closeModalBtn')?.addEventListener('click', () => {
      overlay.classList.add('animate-fade-out'); // Add fade out if you have it
      setTimeout(() => {
        if (document.body.contains(overlay)) {
          document.body.removeChild(overlay);
        }
      }, 200);
    });
  }


  goBack(): void {
    this.router.navigate(['/labtech/pending']);
  }
}
