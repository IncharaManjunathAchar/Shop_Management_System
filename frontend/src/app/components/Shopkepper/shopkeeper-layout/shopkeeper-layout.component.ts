import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-shopkeeper-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './shopkeeper-layout.component.html',
  styleUrl: './shopkeeper-layout.component.css'
})
export class ShopkeeperLayoutComponent implements OnInit {

  logoUrl: string | null = null;
  private readonly BASE = 'http://localhost:5244';

  constructor(private auth: AuthService, private api: ApiService, private cdr: ChangeDetectorRef, public router: Router) {}

  ngOnInit() {
    // Show cached logo instantly, then refresh from API
    const cached = localStorage.getItem('shopLogoUrl');
    if (cached) { this.logoUrl = cached; this.cdr.detectChanges(); }

    this.api.getShopsByUser('').subscribe({
      next: shops => {
        const logo = shops[0]?.logoUrl;
        this.logoUrl = logo ? this.BASE + logo : null;
        if (this.logoUrl) localStorage.setItem('shopLogoUrl', this.logoUrl);
        else localStorage.removeItem('shopLogoUrl');
        this.cdr.detectChanges();
      }
    });
  }

  get username() { return this.auth.getUsername(); }
  logout() { localStorage.removeItem('shopLogoUrl'); this.auth.logout(); }
}
