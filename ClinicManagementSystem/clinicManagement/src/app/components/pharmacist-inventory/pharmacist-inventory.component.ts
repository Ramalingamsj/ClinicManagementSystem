import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PharmacistService } from '../../services/pharmacist.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-pharmacist-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pharmacist-inventory.component.html',
  styleUrl: './pharmacist-inventory.component.css'
})
export class PharmacistInventoryComponent implements OnInit {
  medicines: any[] = [];
  loading: boolean = true;
  searchTerm: string = '';

  // Modal & Toast state
  showAddModal: boolean = false;
  isSubmitting: boolean = false;
  toasts: any[] = [];

  newMed = {
    medicineName: '',
    medicineType: 'Tablet',
    description: '',
    stockQuantity: 0,
    price: 0
  };

  medicineTypes = ['Tablet', 'Capsule', 'Syrup', 'Injection', 'Ointment', 'Other'];

  constructor(private pharmacistService: PharmacistService) { }

  ngOnInit(): void {
    this.fetchMedicines();
  }

  showToast(message: string, type: 'success' | 'warning' | 'error' = 'success'): void {
    const id = Date.now();
    this.toasts.push({ id, message, type });
    setTimeout(() => {
      this.toasts = this.toasts.filter(t => t.id !== id);
    }, 4000);
  }

  removeToast(id: number): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  fetchMedicines(): void {
    this.loading = true;
    this.pharmacistService.getMedicines().subscribe({
      next: (data) => {
        this.medicines = (data || []).sort((a: any, b: any) => a.medicineName.localeCompare(b.medicineName));
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load inventory:', err);
        this.showToast('Failed to sync inventory data. Backend unreachable.', 'error');
        this.loading = false;
      }
    });
  }

  get filteredMedicines(): any[] {
    if (!this.searchTerm.trim()) return this.medicines;
    const term = this.searchTerm.toLowerCase();
    return this.medicines.filter(m =>
      m.medicineName.toLowerCase().includes(term) ||
      m.medicineType?.toLowerCase().includes(term)
    );
  }

  updateStock(med: any, newQty: string): void {
    const qty = parseInt(newQty);
    if (isNaN(qty) || qty < 0) {
      this.showToast('Please enter a valid positive quantity', 'warning');
      return;
    }

    this.pharmacistService.updateStock(med.medicineId, qty).subscribe({
      next: () => {
        med.stockQuantity += qty;
        this.showToast(`Pharma-Restock: Added ${qty} units to ${med.medicineName}.`, 'success');
      },
      error: (err) => {
        console.error('Stock update failed:', err);
        this.showToast('Security verification failed. Stock update rejected.', 'error');
      }
    });
  }

  openAddModal(): void {
    this.showAddModal = true;
    this.newMed = {
      medicineName: '',
      medicineType: 'Tablet',
      description: '',
      stockQuantity: 0,
      price: 0
    };
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  onSubmitMedicine(): void {
    if (!this.newMed.medicineName || this.newMed.price <= 0) {
      this.showToast('Invalid Registration: Name or Price requirements not met.', 'warning');
      return;
    }

    this.isSubmitting = true;
    this.pharmacistService.addMedicine(this.newMed).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.closeAddModal();
        this.showToast(`Success: ${this.newMed.medicineName} issued into inventory.`, 'success');
        this.fetchMedicines();
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Failed to add medicine:', err);
        this.showToast(err.error || 'Conflict detected. Medicine registration failed.', 'error');
      }
    });
  }

  getStockStatus(qty: number): string {
    if (qty <= 0) return 'Out of Stock';
    if (qty < 20) return 'Low Stock';
    return 'Available';
  }

  getStatusClass(qty: number): string {
    if (qty <= 0) return 'status-danger';
    if (qty < 20) return 'status-warning';
    return 'status-success';
  }
}
