import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../../services/doctor.service';
import { AuthService } from '../../services/auth.service';
import { Consultation } from '../../models/doctor.models';

interface HistoryRecord extends Consultation {
  patientName?: string;
}

@Component({
  selector: 'app-doctor-history',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './doctor-history.component.html',
  styleUrl: './doctor-history.component.css'
})
export class DoctorHistoryComponent implements OnInit {
  history: HistoryRecord[] = [];
  loading: boolean = true;
  error: string | null = null;
  doctorId: number = 0;
  searchTerm: string = '';

  constructor(
    private doctorService: DoctorService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.doctorId = user.userId;
      this.fetchHistory();
    }
  }

  fetchHistory() {
    this.loading = true;
    this.doctorService.getDoctorHistory(this.doctorId).subscribe({
      next: (data) => {
        this.history = data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to synchronize with clinical database. Please check your network connection.';
        this.loading = false;
        this.history = [];
      }
    });
  }


  get filteredHistory(): HistoryRecord[] {
    if (!this.searchTerm) return this.history;
    const term = this.searchTerm.toLowerCase();
    return this.history.filter(c =>
      (c.patientName || '').toLowerCase().includes(term) ||
      (c.diagnosis || '').toLowerCase().includes(term) ||
      (c.consultationId?.toString() || '').includes(term)
    );
  }

  viewDetails(appointmentId: number) {
    this.router.navigate(['/doctor/consultation', appointmentId]);
  }
}

