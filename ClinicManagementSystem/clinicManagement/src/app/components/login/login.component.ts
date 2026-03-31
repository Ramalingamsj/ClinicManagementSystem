import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginData = {
    username: '',
    password: ''
  };

  error: string = '';
  loading: boolean = false;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    const user = this.authService.currentUserValue;
    if (user) {
      const role = user.roleName?.toLowerCase() || '';
      if (role === 'pharmacist' || role === 'pharmacy' || role.includes('pharm')) {
        this.router.navigate(['/pharmacist/pending']);
      } else if (role === 'labtechnician' || role === 'lab technician' || role.includes('lab')) {
        this.router.navigate(['/labtech/pending']);
      } else if (role === 'receptionist' || role === 'reception' || role.includes('recept')) {
        this.router.navigate(['/receptionist/workstation']);
      } else {
        this.router.navigate(['/doctor/pending']);
      }
    }
  }

  onSubmit(valid: boolean | null) {
    if (!valid) {
      return;
    }

    this.loading = true;
    this.error = '';
    this.authService.login(this.loginData.username, this.loginData.password)
      .subscribe({
        next: (data) => {
          const role = data.roleName?.toLowerCase() || '';

          if (role === 'pharmacist' || role === 'pharmacy' || role.includes('pharm')) {
            this.router.navigate(['/pharmacist/pending']);
          } else if (role === 'labtechnician' || role === 'lab technician' || role.includes('lab')) {
            this.router.navigate(['/labtech/pending']);
          } else if (role === 'receptionist' || role === 'reception' || role.includes('recept')) {
            this.router.navigate(['/receptionist/workstation']);
          } else {
            this.router.navigate(['/doctor/pending']);
          }
        },
        error: (err) => {
          this.error = 'Invalid Credentials or server offline.';
          this.loading = false;
        }
      });
  }
}
