import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReceptionistService } from '../../services/receptionist.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-receptionist-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receptionist-history.component.html',
  styleUrls: ['./receptionist-history.component.css']
})
export class ReceptionistHistoryComponent implements OnInit {
  appointmentsHistory: any[] = [];
  loadingHistory: boolean = false;
  searchTerm: string = '';
  selectedLog: any = null;
  showLogModal: boolean = false;

  constructor(
    private receptionService: ReceptionistService,
    private authService: AuthService
  ) { }

  get currentUser(): any {
    return this.authService.currentUserValue;
  }

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    const id = this.currentUser?.userId;
    if (!id) return;

    this.loadingHistory = true;
    this.receptionService.getAppointmentHistory(id).subscribe({
      next: (data) => {
        this.appointmentsHistory = data;
        this.loadingHistory = false;
      },
      error: () => this.loadingHistory = false
    });
  }

  get filteredHistory(): any[] {
    if (!this.searchTerm.trim()) return this.appointmentsHistory;
    const term = this.searchTerm.toLowerCase();
    return this.appointmentsHistory.filter(h => 
      h.patientName.toLowerCase().includes(term) || 
      h.doctorName.toLowerCase().includes(term) ||
      h.appointmentId.toString().includes(term)
    );
  }

  viewLog(item: any): void {
    this.selectedLog = item;
    this.showLogModal = true;
  }

  closeLog(): void {
    this.showLogModal = false;
    this.selectedLog = null;
  }
}
