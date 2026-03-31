import { Component } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterModule, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {
  showScrollButton: boolean = false;
  userName: string = '';
  roleName: string = 'Staff';
  isDoctor: boolean = false;
  isPharmacist: boolean = false;
  isLabTech: boolean = false;
  navItems: { path: string, icon: string, label: string }[] = [];

  constructor(private authService: AuthService, private router: Router) {
    const user = this.authService.currentUserValue;
    if (user) {
      this.userName = user.fullName || user.username || 'User';
      this.roleName = user.roleName || 'Staff';

      const rId = this.roleName.toLowerCase();

      if (rId.includes('doctor')) {
        this.isDoctor = true;
        this.navItems = [
          { path: '/doctor/pending', label: 'Pending Patients', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>' },
          { path: '/doctor/history', label: 'Patient History', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>' },
        ];
      }
      else if (rId.includes('pharmac') || rId.includes('pharmacy')) {
        this.isPharmacist = true;
        this.navItems = [
          { path: '/pharmacist/pending', label: 'Pending Prescriptions', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10 20.5V20M14 20.5V20M6 20.5V20M4 4h16c1.1 0 2 .9 2 2v2c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2zM4 10h16v10c0 1.1-.9 2-2 2H6c-1.1 0-2-.9-2-2V10z"></path></svg>' },
          { path: '/pharmacist/inventory', label: 'Medicine Inventory', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m10.5 20.5 10-10a4.95 4.95 0 1 0-7-7l-10 10a4.95 4.95 0 1 0 7 7Z"></path><path d="m8.5 8.5 7 7"></path></svg>' },
          { path: '/pharmacist/history', label: 'Billing & History', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>' }
        ];
      } else if (rId.includes('lab')) {
        this.isLabTech = true;
        this.navItems = [
          { path: '/labtech/pending', label: 'Pending Tests', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 2 22 22 22 12 2"></polygon></svg>' },
          { path: '/labtech/inventory', label: 'Lab Inventory', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 10a8 8 0 0 1 8-8v8Z"/><path d="M22 10a8 8 0 0 0-8-8v8Z"/><path d="M22 14a8 8 0 0 1-8 8v-8Z"/><path d="M2 14a8 8 0 0 0 8 8v-8Z"/></svg>' },
          { path: '/labtech/history', label: 'Lab History', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>' }
        ];
      } else if (rId.includes('recept')) {
        this.navItems = [
          { path: '/receptionist/workstation', label: 'Reception Workstation', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>' },
          { path: '/receptionist/history', label: 'Audit History', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>' }
        ];
      }
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  onScroll(event: any) {
    const scrollTop = event.target.scrollTop;
    this.showScrollButton = scrollTop > 300;
  }

  scrollToTop(container: HTMLElement) {
    container.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }
}
