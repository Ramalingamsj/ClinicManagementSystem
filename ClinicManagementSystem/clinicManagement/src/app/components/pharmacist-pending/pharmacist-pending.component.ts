import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PharmacistService } from '../../services/pharmacist.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-pharmacist-pending',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './pharmacist-pending.component.html',
  styleUrl: './pharmacist-pending.component.css'
})
export class PharmacistPendingComponent implements OnInit {
  todayConsultations: any[] = [];
  loading: boolean = true;
  error: string | null = null;
  pharmacyUserId: number = 0;
  searchTerm: string = '';

  constructor(
    private pharmacistService: PharmacistService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.pharmacyUserId = user.userId;
      this.fetchToday();
    }
  }

  fetchToday(): void {
    this.loading = true;
    this.pharmacistService.getTodaysConsultations().subscribe({
      next: (data) => {
        this.todayConsultations = data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load today\'s consultations:', err);
        this.error = 'Unable to fetch today\'s prescriptions.';
        this.loading = false;
      }
    });
  }

  get filteredConsultations(): any[] {
    if (!this.searchTerm.trim()) return this.todayConsultations;
    const term = this.searchTerm.toLowerCase();
    return this.todayConsultations.filter(c =>
      c.patientName?.toLowerCase().includes(term) ||
      c.consultationId.toString().includes(term)
    );
  }

  openIssueWorkspace(consultationId: number): void {
    this.router.navigate(['/pharmacist/issue', consultationId]);
  }

  isFulfilled(apt: any): boolean {
    if (!apt.medicines || apt.medicines.length === 0) return false;
    return apt.medicines.every((m: any) => m.statusId === 7);
  }
}
