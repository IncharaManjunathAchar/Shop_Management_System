import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  profile: any = null;
  shops: any[] = [];
  editMode = false;
  saving = false;
  successMsg = '';
  errorMsg = '';
  showCancelConfirm = false;
  showAddShop = false;
  addingShop = false;

  form = { phoneNumber: '', shopName: '', shopAddress: '', contactNumber: '' };
  newShop = { shopName: '', shopAddress: '', contactNumber: '' };
  logoFile: File | null = null;
  logoPreview: string | null = null;

  private readonly BASE = 'http://localhost:5244';

  constructor(private api: ApiService, private cdr: ChangeDetectorRef, public router: Router) {}

  ngOnInit() { this.load(); }

  load() {
    this.api.getProfile().subscribe({
      next: res => { this.profile = res; this.resetForm(); this.cdr.detectChanges(); },
      error: () => { this.errorMsg = 'Failed to load profile.'; }
    });
    this.api.getShopsByUser('').subscribe({
      next: res => { this.shops = [...res]; this.cdr.detectChanges(); },
      error: () => {}
    });
  }

  resetForm() {
    this.form = {
      phoneNumber:   this.profile.phoneNumber   ?? '',
      shopName:      this.profile.shop?.shopName      ?? '',
      shopAddress:   this.profile.shop?.shopAddress   ?? '',
      contactNumber: this.profile.shop?.contactNumber ?? ''
    };
    this.logoFile = null;
    this.logoPreview = null;
  }

  get logoUrl(): string | null {
    if (this.logoPreview) return this.logoPreview;
    if (this.profile?.shop?.logoUrl) return this.BASE + this.profile.shop.logoUrl;
    return null;
  }

  onLogoSelect(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    const allowed = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowed.includes(file.type)) { this.errorMsg = 'Only JPG, PNG or WebP images allowed.'; return; }
    if (file.size > 2 * 1024 * 1024) { this.errorMsg = 'Logo must be under 2MB.'; return; }
    this.logoFile = file;
    this.errorMsg = '';
    const reader = new FileReader();
    reader.onload = e => { this.logoPreview = e.target?.result as string; this.cdr.detectChanges(); };
    reader.readAsDataURL(file);
  }

  save() {
    this.saving = true; this.successMsg = ''; this.errorMsg = '';
    const fd = new FormData();
    fd.append('phoneNumber',   this.form.phoneNumber);
    fd.append('shopName',      this.form.shopName);
    fd.append('shopAddress',   this.form.shopAddress);
    fd.append('contactNumber', this.form.contactNumber);
    if (this.logoFile) fd.append('logo', this.logoFile);

    this.api.updateProfile(fd).subscribe({
      next: res => {
        this.saving = false; this.editMode = false;
        this.successMsg = 'Profile updated successfully!';
        if (res.logoUrl) {
          this.profile.shop.logoUrl = res.logoUrl;
          localStorage.setItem('shopLogoUrl', 'http://localhost:5244' + res.logoUrl);
        }
        this.profile.phoneNumber   = this.form.phoneNumber;
        this.profile.shop.shopName = this.form.shopName;
        this.profile.shop.shopAddress   = this.form.shopAddress;
        this.profile.shop.contactNumber = this.form.contactNumber;
        this.logoPreview = null;
        this.cdr.detectChanges();
      },
      error: err => { this.saving = false; this.errorMsg = err?.error || 'Update failed.'; this.cdr.detectChanges(); }
    });
  }

  cancel() { this.editMode = false; this.resetForm(); this.successMsg = ''; this.errorMsg = ''; }

  addShop() {
    if (!this.newShop.shopName || !this.newShop.shopAddress || !this.newShop.contactNumber) {
      this.errorMsg = 'All shop fields are required.'; return;
    }
    this.addingShop = true; this.errorMsg = '';
    const fd = new FormData();
    fd.append('shopName',      this.newShop.shopName);
    fd.append('shopAddress',   this.newShop.shopAddress);
    fd.append('contactNumber', this.newShop.contactNumber);
    this.api.createShop(fd).subscribe({
      next: shop => {
        this.shops = [...this.shops, shop];
        this.addingShop = false;
        this.showAddShop = false;
        this.newShop = { shopName: '', shopAddress: '', contactNumber: '' };
        this.successMsg = 'Shop added successfully!';
        this.cdr.detectChanges();
      },
      error: err => {
        this.addingShop = false;
        this.errorMsg = err?.error || 'Failed to add shop.';
        this.cdr.detectChanges();
      }
    });
  }

  cancelSubscription() {
    this.api.cancelSubscription().subscribe({
      next: () => {
        this.showCancelConfirm = false;
        this.router.navigate(['/shopkeeper/subscription']);
      },
      error: err => { this.errorMsg = err?.error || 'Failed to cancel subscription.'; this.cdr.detectChanges(); }
    });
  }

  get subBadgeClass(): string {
    const s = this.profile?.subscription?.status?.toLowerCase();
    if (s === 'approved') return 'badge-active';
    if (s === 'pending')  return 'badge-pending';
    return 'badge-expired';
  }
}
