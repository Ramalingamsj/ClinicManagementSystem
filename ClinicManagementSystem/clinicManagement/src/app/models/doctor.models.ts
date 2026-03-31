export interface Patient {
  patientId: number;
  patientName: string;
  contact?: string;
  email?: string;
  dob?: string;
  gender?: string;
  address?: string;
}

export interface Slot {
  slotId: number;
  tokenNo: number;
  slotTime: string;
}

export interface Doctor {
  doctorId: number;
  fullName?: string;
  specializationId?: number;
  experience?: number;
  consultationFee?: number;
}

export interface Appointment {
  appointmentId: number;
  patientId: number;
  doctorId: number;
  appointmentDate?: string;
  statusId?: number;
  patient?: Patient;
  slot?: Slot;
}

export interface PatientMedicine {
  patientMedicineId?: number;
  medicineId: number;
  consultationId?: number;
  dosage?: string;
  duration?: string;
  durationDays?: number;
  frequency?: number;
  quantity?: number;
  notes?: string;
  statusId?: number;
  medicine?: {
    medicineName?: string;
  };
}

export interface PatientLabTest {
  patientLabtestId?: number;
  labTestId: number;
  consultationId?: number;
  testNotes?: string;
  result?: string;
  labtest?: {
    testName?: string;
  };
  statusId?: number;
}

export interface Consultation {
  consultationId?: number;
  appointmentId: number;
  createdAt?: string;
  consultationDate?: string;
  symptoms?: string;
  diagnosis?: string;
  doctorNotes?: string;
  patientMedicines?: PatientMedicine[];
  patientLabTests?: PatientLabTest[];
}

export interface Medicine {
  medicineId?: number;
  id?: number;
  medicineName?: string;
  name?: string;
}

export interface LabTest {
  labtestId?: number;
  labTestId?: number;
  id?: number;
  testName?: string;
  name?: string;
}

export interface Bill {
  billId?: number;
  consultationId?: number;
  billDate?: string;
  totalAmount?: number;
  paymentStatus?: string;
  billType?: string;
  createdAt?: string;
  status?: string;
}

export interface PendingPharmacistQueue {
  consultationId: number;
  appointmentId: number;
  patientName?: string;
  consultationDate?: string;
  medicineCount?: number;
  prescriptions?: PatientMedicine[];
}

export interface PendingLabTechQueue {
  consultationId: number;
  appointmentId: number;
  patientName?: string;
  consultationDate?: string;
  testCount?: number;
  tests?: PatientLabTest[];
}
