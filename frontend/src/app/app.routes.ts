import { Routes } from '@angular/router';
import { InventoryComponent } from './components/Shopkepper/inventory/inventory.component';

export const routes: Routes = [
  // ✅ Default → Login page
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // ✅ Auth pages
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

  // ✅ Protected page
  { path: 'inventory', component: InventoryComponent }
];