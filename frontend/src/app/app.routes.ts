import { Routes } from '@angular/router';

// Shopkeeper
import { InventoryComponent } from './components/Shopkepper/inventory/inventory.component';

// Admin
import { DashboardComponent } from './components/Admin/dashboard/dashboard.component';
import { SubscriptionApprovalComponent } from './components/Admin/subscription-approval/subscription-approval.component';

export const routes: Routes = [

  //  Default → Login page
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  //  Auth pages
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register').then(m => m.Register)
  },

  //  Shopkeeper Routes
  { path: 'inventory', component: InventoryComponent },

  //  Admin Routes
  { path: 'admin/dashboard', component: DashboardComponent },
  { path: 'admin/subscriptions', component: SubscriptionApprovalComponent },

  // Wildcard
  { path: '**', redirectTo: 'login' }
];