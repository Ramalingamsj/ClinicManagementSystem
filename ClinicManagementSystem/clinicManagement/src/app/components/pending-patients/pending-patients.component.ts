import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { DoctorService } from '../../services/doctor.service';
import { AuthService } from '../../services/auth.service';
import { Appointment } from '../../models/doctor.models';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pending-patients',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './pending-patients.component.html',
  styleUrl: './pending-patients.component.css'
})
export class PendingPatientsComponent implements OnInit {
  todayAppointments: Appointment[] = [];
  loading: boolean = true;
  error: string | null = null;
  doctorId: number = 0;
  searchTerm: string = '';
  activeFilter: 'all' | 'pending' | 'completed' = 'all';

  get filteredAppointments(): Appointment[] {
    let list = this.todayAppointments;

    // First filter by dashboard status selection
    if (this.activeFilter === 'pending') {
      list = list.filter((a: Appointment) => a.statusId !== 2);
    } else if (this.activeFilter === 'completed') {
      list = list.filter((a: Appointment) => a.statusId === 2);
    }

    // Then filter by search term (Name or ID)
    if (!this.searchTerm) return list;
    const term = this.searchTerm.toLowerCase();
    return list.filter((a: Appointment) =>
      (a.patient?.patientName || '').toLowerCase().includes(term) ||
      (a.patient?.patientId?.toString() || '').includes(term)
    );
  }

  setFilter(type: 'all' | 'pending' | 'completed') {
    this.activeFilter = type;
  }

  getPendingCount(): number {
    return this.todayAppointments.filter((a: Appointment) => a.statusId !== 2).length;
  }

  getCompletedCount(): number {
    return this.todayAppointments.filter((a: Appointment) => a.statusId === 2).length;
  }

  getCount(type: 'pending' | 'completed' | 'all'): number {
    if (type === 'all') return this.todayAppointments.length;
    if (type === 'pending') return this.getPendingCount();
    return this.getCompletedCount();
  }


  constructor(
    private doctorService: DoctorService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.doctorId = user.userId;
      this.fetchToday();
    }
  }

  fetchToday() {
    this.loading = true;
    this.doctorService.getTodayPendingPatients(this.doctorId).subscribe({
      next: (data) => {
        this.todayAppointments = data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load today\'s appointments. Please make sure the API is running.';
        this.loading = false;

        // Mock data for display purposes if backend fails during testing
        this.todayAppointments = [
          { appointmentId: 101, patientId: 1, doctorId: this.doctorId, appointmentDate: new Date().toISOString(), patient: { patientId: 1, patientName: 'Sarah Connor', contact: '555-0101', gender: 'Female', dob: '1988-05-15' } },
          { appointmentId: 102, patientId: 2, doctorId: this.doctorId, appointmentDate: new Date().toISOString(), patient: { patientId: 2, patientName: 'John Doe', contact: '555-0192', gender: 'Male', dob: '1995-12-02' } }
        ];
        this.error = 'Showing mock data because API request failed.';
      }
    });
  }

  startConsultation(appointmentId: number) {
    this.router.navigate(['/doctor/consultation', appointmentId]);
  }

  getPatientAge(dobString?: string): string | number {
    if (!dobString) return 'Unknown';
    const dob = new Date(dobString);
    if (isNaN(dob.getTime())) return 'Unknown';
    const ageDiffMs = Date.now() - dob.getTime();
    const ageDate = new Date(ageDiffMs);
    const calculatedAge = Math.abs(ageDate.getUTCFullYear() - 1970);
    return calculatedAge || 0;
  }

  getCurrentDate(): string {
    return new Date().toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  getInitials(name?: string): string {
    if (!name) return '??';
    return name.split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }
}
