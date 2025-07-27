import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const LoginRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const user = auth.getCurrentUser();
  const role = user?.role?.trim().toLowerCase();

  if (user?.token) {
    if (role === 'admin') {
      router.navigate(['/admin']);
    } else if (role === 'user') {
      router.navigate(['/home']);
    } else {
      router.navigate(['/unauthorized']);
    }
    return false; // Prevent access to login page
  }

  return true; // Allow access if not logged in
};
