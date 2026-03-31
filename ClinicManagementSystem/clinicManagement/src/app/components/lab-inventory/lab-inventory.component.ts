import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LabTechnicianService } from '../../services/lab-technician.service';

@Component({
  selector: 'app-lab-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lab-inventory.component.html',
  styleUrl: './lab-inventory.component.css'
})
export class LabInventoryComponent implements OnInit {
  labTests: any[] = [];
  loading: boolean = true;
  searchTerm: string = '';
  
  // Modal state
  showAddModal: boolean = false;
  isSubmitting: boolean = false;
  toasts: any[] = [];

  // KPI Stats (optional to keep or remove if perfectly matching, but let's keep logic for now)
  totalTests: number = 0;
  avgPrice: number = 0;
  premiumTests: number = 0;

  newTest = {
    testName: '',
    description: '',
    price: 0
  };

  constructor(private labService: LabTechnicianService) {}

  ngOnInit(): void {
    this.fetchLabTests();
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

  getPriceTier(price: number): string {
    if (price < 500) return 'Standard';
    if (price <= 1500) return 'Premium';
    return 'Elite';
  }

  getPriceClass(price: number): string {
    if (price < 500) return 'status-success';
    if (price <= 1500) return 'status-warning';
    return 'status-danger';
  }

  fetchLabTests(): void {
    this.loading = true;
    this.labService.getLabTests().subscribe({
      next: (data) => {
        this.labTests = data || [];
        this.calculateKPIs();
        this.loading = false;
      },
      error: (err) => {
        console.error('Fetch error:', err);
        this.showToast('Failed to sync diagnostic catalog.', 'error');
        this.loading = false;
      }
    });
  }

  calculateKPIs(): void {
    this.totalTests = this.labTests.length;
    if (this.totalTests > 0) {
      const sum = this.labTests.reduce((acc, curr) => acc + (curr.price || 0), 0);
      this.avgPrice = sum / this.totalTests;
      this.premiumTests = this.labTests.filter(t => t.price > 1000).length;
    }
  }

  get filteredTests(): any[] {
    if (!this.searchTerm.trim()) return this.labTests;
    const term = this.searchTerm.toLowerCase();
    return this.labTests.filter(t => 
      t.testName.toLowerCase().includes(term) || 
      t.description?.toLowerCase().includes(term)
    );
  }

  openAddModal(): void {
    this.showAddModal = true;
    this.newTest = { testName: '', description: '', price: 0 };
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  updatePrice(test: any, newPriceStr: string): void {
    const price = parseFloat(newPriceStr);
    if (isNaN(price) || price <= 0) {
      this.showToast('Invalid price point detected.', 'warning');
      return;
    }

    this.labService.updateLabPrice(test.labtestId, price).subscribe({
      next: () => {
        test.price = price;
        this.calculateKPIs();
        this.showToast(`Catalog updated: ${test.testName} price synced.`, 'success');
      },
      error: (err) => {
        console.error('Update error:', err);
        this.showToast('Price adjustment rejected by server.', 'error');
      }
    });
  }

  onSubmitTest(): void {
    if (!this.newTest.testName || this.newTest.price <= 0) {
      this.showToast('Diagnostic requirements incomplete.', 'warning');
      return;
    }

    this.isSubmitting = true;
    this.labService.addLabTest(this.newTest).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.closeAddModal();
        this.showToast(`New diagnostic issued: ${this.newTest.testName}.`, 'success');
        this.fetchLabTests();
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Add error:', err);
        const msg = err.status === 409 ? err.error : 'Conflict detected in diagnostic registry.';
        this.showToast(msg, 'error');
      }
    });
  }
}
