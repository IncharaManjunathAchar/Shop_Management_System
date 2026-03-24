import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { CardModule } from 'primeng/card';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    TableModule, ButtonModule, InputNumberModule, SelectModule,
    TagModule, ToastModule, DialogModule, InputTextModule,
    DatePickerModule, CardModule
  ],
  providers: [MessageService],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css'
})
export class InventoryComponent implements OnInit {
  shopId = 0;
  shops: any[] = [];
  selectedShop: any = null;
  items: any[] = [];
  filteredItems: any[] = [];
  filter = 'all';
  activeForm: 'purchase' | 'sale' = 'purchase';
  showAddDialog = false;

  purchaseForm = { itemId: null, quantity: null, costPrice: null };
  saleForm = { itemId: null, quantity: null, sellingPrice: null };
  newItem = { itemName: '', quantity: null, costPrice: null, sellingPrice: null, expiryDate: null };

  constructor(
    private api: ApiService,
    private auth: AuthService,
    private route: ActivatedRoute,
    private msg: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(p => {
      if (p['action'] === 'purchase') this.activeForm = 'purchase';
    });
    this.loadShops();
  }

  loadShops() {
    const userId = this.auth.getUserId();
    this.api.getShopsByUser(userId).subscribe({
      next: shops => {
        this.shops = shops;
        if (shops.length > 0) {
          const savedId = this.auth.getShopId();
          this.selectedShop = shops.find(s => s.shopId === savedId) || shops[0];
          this.shopId = this.selectedShop.shopId;
          this.auth.setShopId(this.shopId);
          this.cdr.detectChanges();
          this.loadItems();
        }
      },
      error: () => this.msg.add({ severity: 'error', summary: 'Error', detail: 'Failed to load shops.' })
    });
  }

  onShopChange() {
    this.shopId = this.selectedShop.shopId;
    this.auth.setShopId(this.shopId);
    this.loadItems();
  }

  loadItems() {
    this.api.getItems(this.shopId).subscribe({
      next: data => { this.items = data; this.applyFilter(); },
      error: () => this.msg.add({ severity: 'error', summary: 'Error', detail: 'Failed to load items.' })
    });
  }

  applyFilter() {
    const today = new Date();
    const soon = new Date(); soon.setDate(today.getDate() + 30);
    if (this.filter === 'low') {
      this.filteredItems = this.items.filter(i => i.quantity <= 5);
    } else if (this.filter === 'expiring') {
      this.filteredItems = this.items.filter(i => new Date(i.expiryDate) <= soon && new Date(i.expiryDate) >= today);
    } else {
      this.filteredItems = [...this.items];
    }
  }

  setFilter(f: string) { this.filter = f; this.applyFilter(); }

  onPurchase() {
    const tx = { shopId: this.shopId, itemId: this.purchaseForm.itemId, transactionType: 'Purchase', quantity: this.purchaseForm.quantity, unitPrice: this.purchaseForm.costPrice };
    this.api.createTransaction(tx).subscribe({
      next: () => {
        this.msg.add({ severity: 'success', summary: 'Success', detail: 'Stock added successfully.' });
        this.purchaseForm = { itemId: null, quantity: null, costPrice: null };
        this.loadItems();
      },
      error: err => this.msg.add({ severity: 'error', summary: 'Error', detail: err.error || 'Purchase failed.' })
    });
  }

  onSale() {
    const tx = { shopId: this.shopId, itemId: this.saleForm.itemId, transactionType: 'Sale', quantity: this.saleForm.quantity, unitPrice: this.saleForm.sellingPrice };
    this.api.createTransaction(tx).subscribe({
      next: () => {
        this.msg.add({ severity: 'success', summary: 'Success', detail: 'Sale recorded successfully.' });
        this.saleForm = { itemId: null, quantity: null, sellingPrice: null };
        this.loadItems();
      },
      error: err => this.msg.add({ severity: 'error', summary: 'Error', detail: err.error || 'Sale failed.' })
    });
  }

  addItem() {
    const item = { ...this.newItem, shopId: this.shopId };
    this.api.createItem(item).subscribe({
      next: () => {
        this.msg.add({ severity: 'success', summary: 'Success', detail: 'Item added.' });
        this.showAddDialog = false;
        this.newItem = { itemName: '', quantity: null, costPrice: null, sellingPrice: null, expiryDate: null };
        this.loadItems();
      },
      error: () => this.msg.add({ severity: 'error', summary: 'Error', detail: 'Failed to add item.' })
    });
  }

  deleteItem(id: number) {
    this.api.deleteItem(id).subscribe({
      next: () => { this.msg.add({ severity: 'success', summary: 'Deleted', detail: 'Item removed.' }); this.loadItems(); },
      error: () => this.msg.add({ severity: 'error', summary: 'Error', detail: 'Delete failed.' })
    });
  }

  lowStockCount() { return this.items.filter(i => i.quantity <= 5).length; }
  expiringSoonCount() {
    const soon = new Date(); soon.setDate(soon.getDate() + 30);
    return this.items.filter(i => new Date(i.expiryDate) <= soon && new Date(i.expiryDate) >= new Date()).length;
  }

  totalValue(item: any) { return item.quantity * item.sellingPrice; }
  isLowStock(item: any) { return item.quantity <= 5; }
  isExpiringSoon(item: any) {
    const soon = new Date(); soon.setDate(soon.getDate() + 30);
    return new Date(item.expiryDate) <= soon && new Date(item.expiryDate) >= new Date();
  }

  stockSeverity(item: any): 'success' | 'warn' | 'danger' {
    if (item.quantity === 0) return 'danger';
    if (item.quantity <= 5) return 'warn';
    return 'success';
  }
}
