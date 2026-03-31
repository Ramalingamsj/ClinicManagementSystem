import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LabTechnicianService } from '../../services/lab-technician.service';

@Component({
  selector: 'app-labtech-pending',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './labtech-pending.component.html',
  styleUrl: './labtech-pending.component.css'
})
export class LabtechPendingComponent implements OnInit {
  todayTests: any[] = [];
  filteredTodayTests: any[] = [];
  searchTerm: string = '';
  loading: boolean = true;
  error: string | null = null;

  constructor(
    private labService: LabTechnicianService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchToday();
  }

  fetchToday(): void {
    this.loading = true;
    this.labService.getPendingTests().subscribe({
      next: (data) => {
        this.todayTests = data || [];
        this.filteredTodayTests = [...this.todayTests];
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load today\'s tests:', err);
        this.error = 'Unable to fetch today\'s lab tests from the server.';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredTodayTests = [...this.todayTests];
      return;
    }

    this.filteredTodayTests = this.todayTests.filter(test => 
      test.patientName?.toLowerCase().includes(term) ||
      test.consultationId.toString().includes(term)
    );
  }

  openTestWorkspace(consultationId: number): void {
    this.router.navigate(['/labtech/test', consultationId]);
  }

  getStatusLabel(tests: any[]): string {
    if (!tests || tests.length === 0) return 'No Tests';
    const completed = tests.filter(t => t.statusId === 2).length;
    if (completed === 0) return 'Pending Results';
    if (completed < tests.length) return 'Partial Upload';
    return 'Completed';
  }

  getStatusClass(tests: any[]): string {
    if (!tests || tests.length === 0) return 'status-pending';
    const completed = tests.filter(t => t.statusId === 2).length;
    if (completed === 0) return 'status-pending';
    if (completed < tests.length) return 'status-partial';
    return 'status-completed';
  }
}
