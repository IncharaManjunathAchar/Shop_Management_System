import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-plans',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './plans.component.html',
  styleUrl: './plans.component.css'
})
export class PlansComponent implements OnInit {

  plans: any[] = [];
  showForm = false;
  editing = false;
  form: any = this.emptyForm();
  deleteTarget: any = null;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.load(); }

  load() {
    this.api.getPlans().subscribe({
      next: res => { this.plans = [...res]; this.cdr.detectChanges(); },
      error: err => console.error(err)
    });
  }

  emptyForm() {
    return { planId: 0, planName: '', price: 0, durationDays: 30, trialDays: 0, description: '', maxShops: 1 };
  }

  openAdd() { this.form = this.emptyForm(); this.editing = false; this.showForm = true; }

  openEdit(plan: any) {
    this.form = { ...plan };
    this.editing = true;
    this.showForm = true;
  }

  save() {
    const call = this.editing
      ? this.api.updatePlan(this.form.planId, this.form)
      : this.api.createPlan(this.form);
    call.subscribe({ next: () => { this.showForm = false; this.load(); }, error: err => console.error(err) });
  }

  confirmDelete(plan: any) { this.deleteTarget = plan; }

  doDelete() {
    this.api.deletePlan(this.deleteTarget.planId).subscribe({
      next: () => { this.deleteTarget = null; this.load(); },
      error: err => console.error(err)
    });
  }
}
