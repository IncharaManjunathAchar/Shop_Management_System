import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ApiService } from '../services/api.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn()) return true;
  router.navigate(['/login']);
  return false;
};

export const roleGuard = (role: string): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn() && auth.getRole() === role) return true;
  router.navigate(['/login']);
  return false;
};

export const subscriptionGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const api = inject(ApiService);
  const router = inject(Router);

  if (!auth.isLoggedIn() || auth.getRole() !== 'Shopkeeper') {
    router.navigate(['/login']);
    return false;
  }

  const userId = auth.getUserId();
  return api.getActiveSubscription(userId).pipe(
    map((res: any) => {
      if (res?.isActive === true) return true;
      router.navigate(['/shopkeeper/subscription']);
      return false;
    }),
    catchError(() => {
      router.navigate(['/shopkeeper/subscription']);
      return of(false);
    })
  );
};
