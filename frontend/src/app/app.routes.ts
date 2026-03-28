import { Routes } from '@angular/router';
import { ShopkeeperLayoutComponent } from './components/Shopkepper/shopkeeper-layout/shopkeeper-layout.component';
import { DashboardspkComponent } from './components/Shopkepper/dashboardspk/dashboardspk.component';
import { InventoryComponent } from './components/Shopkepper/inventory/inventory.component';
import { SubscriptionspkComponent } from './components/Shopkepper/subscriptionspk/subscriptionspk.component';
import { TransactionsComponent } from './components/Shopkepper/transactions/transactions.component';
import { ProfileComponent } from './components/Shopkepper/profile/profile.component';
import { AdminLayoutComponent } from './components/Admin/admin-layout/admin-layout.component';
import { DashboardComponent } from './components/Admin/dashboard/dashboard.component';
import { UsersComponent } from './components/Admin/users/users.component';
import { SubscriptionsComponent } from './components/Admin/subscriptions/subscriptions.component';
import { PlansComponent } from './components/Admin/plans/plans.component';
import { roleGuard, subscriptionGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // Public
  { path: 'login',    loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.Register) },

  // Shopkeeper
  {
    path: 'shopkeeper',
    component: ShopkeeperLayoutComponent,
    canActivate: [roleGuard('Shopkeeper')],
    children: [
      { path: '',             redirectTo: 'subscription', pathMatch: 'full' },
      { path: 'subscription', component: SubscriptionspkComponent },
      { path: 'dashboard',    component: DashboardspkComponent,    canActivate: [subscriptionGuard] },
      { path: 'inventory',    component: InventoryComponent,        canActivate: [subscriptionGuard] },
      { path: 'transactions', component: TransactionsComponent,     canActivate: [subscriptionGuard] },
      { path: 'profile',       component: ProfileComponent,          canActivate: [subscriptionGuard] },
    ]
  },

  // Legacy redirects
  { path: 'subscriptionspk', redirectTo: 'shopkeeper/subscription', pathMatch: 'full' },
  { path: 'inventory',       redirectTo: 'shopkeeper/inventory',    pathMatch: 'full' },

  // Admin
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [roleGuard('Admin')],
    children: [
      { path: '',             redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',     component: DashboardComponent },
      { path: 'users',         component: UsersComponent },
      { path: 'subscriptions', component: SubscriptionsComponent },
      { path: 'plans',         component: PlansComponent },
    ]
  },

  { path: '**', redirectTo: 'login' }
];
