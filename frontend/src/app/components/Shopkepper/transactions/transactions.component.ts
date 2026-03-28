import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.css'
})
export class TransactionsComponent implements OnInit {

  all: any[] = [];
  filter: 'all' | 'Sale' | 'Purchase' = 'all';
  search = '';
  dateFrom = '';
  dateTo = '';

  constructor(private api: ApiService, private auth: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    const userId = this.auth.getUserId();
    this.api.getShopsByUser(userId).subscribe({
      next: shops => {
        if (!shops.length) return;
        this.api.getTransactionsByShop(shops[0].shopId).subscribe({
          next: txs => { this.all = [...txs].sort((a, b) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime()); this.cdr.detectChanges(); }
        });
      }
    });
  }

  get filtered() {
    return this.all.filter(t => {
      const matchType   = this.filter === 'all' || t.transactionType === this.filter;
      const matchSearch = !this.search || t.itemName?.toLowerCase().includes(this.search.toLowerCase());
      const txDate      = new Date(t.transactionDate);
      const matchFrom   = !this.dateFrom || txDate >= new Date(this.dateFrom);
      const matchTo     = !this.dateTo   || txDate <= new Date(this.dateTo + 'T23:59:59');
      return matchType && matchSearch && matchFrom && matchTo;
    });
  }

  get totalSales()     { return this.filtered.filter(t => t.transactionType === 'Sale').reduce((s, t) => s + t.totalAmount, 0); }
  get totalPurchases() { return this.filtered.filter(t => t.transactionType === 'Purchase').reduce((s, t) => s + t.totalAmount, 0); }
  get netProfit()      { return this.totalSales - this.totalPurchases; }

  clearFilters() { this.filter = 'all'; this.search = ''; this.dateFrom = ''; this.dateTo = ''; }
}
