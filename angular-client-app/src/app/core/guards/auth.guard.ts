import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const AuthGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const user = auth.getCurrentUser();

  if (!user?.token) {
    router.navigate(['/login']);
    return false;
  }

  const requiredRoles = route.data?.['roles'] as string[] | undefined;
  const userRole = user.role?.trim().toLowerCase();

  if (requiredRoles && !requiredRoles.map(r => r.toLowerCase()).includes(userRole ?? '')) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
