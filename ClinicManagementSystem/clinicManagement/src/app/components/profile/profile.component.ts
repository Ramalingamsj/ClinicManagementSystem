import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { DoctorService } from '../../services/doctor.service';
import { Doctor } from '../../models/doctor.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  doctorData: Doctor | null = null;
  loading: boolean = true;
  authUsername: string = '';

  constructor(
    private authService: AuthService,
    private doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.authUsername = user.username;
      
      this.doctorService.getDoctorProfile(user.userId).subscribe({
        next: (data) => {
          this.doctorData = data;
          this.loading = false;
        },
        error: () => {
          // Fallback mock data if server falls over
          this.doctorData = {
            doctorId: user.userId,
            fullName: user.fullName || 'Test Doctor',
            specializationId: 2,
            experience: 12,
            consultationFee: 500
          };
          this.loading = false;
        }
      });
    }
  }
}
