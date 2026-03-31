import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

// We will lazy-load or directly map these once components are created. 
// For now, mapping directly assuming they will exist shortly.
export const routes: Routes = [
  { 
    path: '', 
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent)
  },
  { 
    path: 'login', 
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'doctor',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', redirectTo: 'pending', pathMatch: 'full' },
      { 
        path: 'pending', 
        loadComponent: () => import('./components/pending-patients/pending-patients.component').then(m => m.PendingPatientsComponent)
      },
      { 
        path: 'consultation/:id', 
        loadComponent: () => import('./components/consultation-workspace/consultation-workspace.component').then(m => m.ConsultationWorkspaceComponent)
      },
      { 
        path: 'profile', 
        loadComponent: () => import('./components/profile/profile.component').then(m => m.ProfileComponent)
      },
      { 
        path: 'history', 
        loadComponent: () => import('./components/doctor-history/doctor-history.component').then(m => m.DoctorHistoryComponent)
      }
    ]
  },

  {
    path: 'pharmacist',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', redirectTo: 'pending', pathMatch: 'full' },
      { 
        path: 'pending', 
        loadComponent: () => import('./components/pharmacist-pending/pharmacist-pending.component').then(m => m.PharmacistPendingComponent)
      },
      { 
        path: 'issue/:id', 
        loadComponent: () => import('./components/issue-workspace/issue-workspace.component').then(m => m.IssueWorkspaceComponent)
      },
      { 
        path: 'history', 
        loadComponent: () => import('./components/pharmacist-history/pharmacist-history.component').then(m => m.PharmacistHistoryComponent)
      },
      { 
        path: 'inventory', 
        loadComponent: () => import('./components/pharmacist-inventory/pharmacist-inventory.component').then(m => m.PharmacistInventoryComponent)
      }
    ]
  },
  {
    path: 'labtech',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', redirectTo: 'pending', pathMatch: 'full' },
      { 
        path: 'pending', 
        loadComponent: () => import('./components/labtech-pending/labtech-pending.component').then(m => m.LabtechPendingComponent)
      },
      { 
        path: 'test/:id', 
        loadComponent: () => import('./components/test-workspace/test-workspace.component').then(m => m.TestWorkspaceComponent)
      },
      { 
        path: 'history', 
        loadComponent: () => import('./components/lab-history/lab-history.component').then(m => m.LabHistoryComponent)
      },
      { 
        path: 'inventory', 
        loadComponent: () => import('./components/lab-inventory/lab-inventory.component').then(m => m.LabInventoryComponent)
      }
    ]
  },
  {
    path: 'receptionist',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', redirectTo: 'workstation', pathMatch: 'full' },
      { 
        path: 'workstation', 
        loadComponent: () => import('./components/receptionist-workstation/receptionist-workstation.component').then(m => m.ReceptionistWorkstationComponent)
      },
      { 
        path: 'history', 
        loadComponent: () => import('./components/receptionist-history/receptionist-history.component').then(m => m.ReceptionistHistoryComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
