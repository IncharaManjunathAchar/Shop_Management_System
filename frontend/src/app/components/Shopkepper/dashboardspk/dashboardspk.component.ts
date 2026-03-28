import { Component, OnInit, AfterViewInit, ChangeDetectorRef, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboardspk',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboardspk.component.html',
  styleUrl: './dashboardspk.component.css'
})
export class DashboardspkComponent implements OnInit {

  @ViewChild('salesChart') salesChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('profitChart') profitChartRef!: ElementRef<HTMLCanvasElement>;

  shop: any = null;
  items: any[] = [];
  transactions: any[] = [];
  profitLoss: any = null;
  subscription: any = null;
  private salesChartInstance: Chart | null = null;
  private profitChartInstance: Chart | null = null;

  constructor(
    private api: ApiService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit() {
    const userId = this.auth.getUserId();

    this.api.getActiveSubscription(userId).subscribe({
      next: res => { this.subscription = res.subscription; this.cdr.detectChanges(); }
    });

    this.api.getShopsByUser(userId).subscribe({
      next: shops => {
        if (!shops.length) { this.cdr.detectChanges(); return; }
        this.shop = shops[0];
        const shopId = this.shop.shopId;
        this.auth.setShopId(shopId);

        this.api.getItems(shopId).subscribe({
          next: items => { this.items = [...items]; this.cdr.detectChanges(); }
        });

        this.api.getTransactionsByShop(shopId).subscribe({
          next: txs => {
            this.transactions = txs;
            this.cdr.detectChanges();
            setTimeout(() => this.renderCharts(), 0);
          }
        });

        this.api.getProfitLoss(shopId).subscribe({
          next: pl => { this.profitLoss = pl; this.cdr.detectChanges(); },
          error: () => { this.profitLoss = null; }
        });
      }
    });
  }

  // --- Stats ---
  get totalItems()     { return this.items.length; }
  get totalStock()     { return this.items.reduce((s, i) => s + (i.quantity ?? 0), 0); }
  get inventoryValue() { return this.items.reduce((s, i) => s + (i.quantity * i.sellingPrice), 0); }
  get todaySales() {
    const today = new Date().toDateString();
    return this.transactions
      .filter(t => t.transactionType === 'Sale' && new Date(t.transactionDate).toDateString() === today)
      .reduce((s, t) => s + t.totalAmount, 0);
  }
  get netProfit() { return this.profitLoss?.profit ?? 0; }

  // --- Alerts ---
  get lowStockItems() { return this.items.filter(i => i.quantity <= 10); }
  get expiringItems() {
    const soon = new Date(); soon.setDate(soon.getDate() + 7);
    return this.items.filter(i => i.expiryDate && new Date(i.expiryDate) <= soon && new Date(i.expiryDate) >= new Date());
  }
  get subscriptionDaysLeft(): number {
    if (!this.subscription?.expiryDate) return 0;
    const diff = new Date(this.subscription.expiryDate).getTime() - Date.now();
    return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }

  // --- Recent Activity ---
  get recentActivity() {
    return [...this.transactions]
      .sort((a, b) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime())
      .slice(0, 5);
  }

  // --- Chart Data: last 7 days ---
  private getLast7Days(): string[] {
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(); d.setDate(d.getDate() - (6 - i));
      return d.toDateString();
    });
  }

  private getDayLabel(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-IN', { weekday: 'short', day: 'numeric' });
  }

  renderCharts() {
    const days = this.getLast7Days();
    const labels = days.map(d => this.getDayLabel(d));

    const salesData = days.map(day =>
      this.transactions
        .filter(t => t.transactionType === 'Sale' && new Date(t.transactionDate).toDateString() === day)
        .reduce((s, t) => s + t.totalAmount, 0)
    );

    const purchaseData = days.map(day =>
      this.transactions
        .filter(t => t.transactionType === 'Purchase' && new Date(t.transactionDate).toDateString() === day)
        .reduce((s, t) => s + t.totalAmount, 0)
    );

    const profitData = days.map((_, i) => salesData[i] - purchaseData[i]);

    if (this.salesChartInstance) this.salesChartInstance.destroy();
    if (this.profitChartInstance) this.profitChartInstance.destroy();

    if (this.salesChartRef?.nativeElement) {
      this.salesChartInstance = new Chart(this.salesChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels,
          datasets: [{
            label: 'Sales (₹)',
            data: salesData,
            backgroundColor: 'rgba(13,148,136,0.7)',
            borderRadius: 6,
            borderSkipped: false
          }]
        },
        options: {
          responsive: true, maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { color: '#64748b', font: { size: 11 } } },
            x: { grid: { display: false }, ticks: { color: '#64748b', font: { size: 11 } } }
          }
        }
      });
    }

    if (this.profitChartRef?.nativeElement) {
      this.profitChartInstance = new Chart(this.profitChartRef.nativeElement, {
        type: 'line',
        data: {
          labels,
          datasets: [{
            label: 'Profit (₹)',
            data: profitData,
            borderColor: '#fb923c',
            backgroundColor: 'rgba(251,146,60,0.1)',
            borderWidth: 2,
            pointBackgroundColor: '#fb923c',
            pointRadius: 4,
            fill: true,
            tension: 0.4
          }]
        },
        options: {
          responsive: true, maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            y: { grid: { color: '#f1f5f9' }, ticks: { color: '#64748b', font: { size: 11 } } },
            x: { grid: { display: false }, ticks: { color: '#64748b', font: { size: 11 } } }
          }
        }
      });
    }
  }

  go(path: string) { this.router.navigate([path]); }
}
