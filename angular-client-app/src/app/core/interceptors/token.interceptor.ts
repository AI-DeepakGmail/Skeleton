import { HttpInterceptorFn } from '@angular/common/http';
import { inject, runInInjectionContext } from '@angular/core';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { globalInjector } from '../injection/global-injector';

export const TokenInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('access_token');

  // Attach token if available
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq);
};

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401 && globalInjector) {
        return runInInjectionContext(globalInjector, () => {
          const authService = inject(AuthService);
          const router = inject(Router);

          // Skip refresh logic for login or refresh endpoints
          const isAuthRequest = ['/auth/login', '/auth/refresh'].some(url =>
            req.url.includes(url)
          );

          if (isAuthRequest) {
            router.navigate(['/unauthorized']);
            return throwError(() => err);
          }

          const refreshToken = localStorage.getItem('refresh_token');
          if (!refreshToken) {
            authService.logout();
            router.navigate(['/unauthorized']);
            return throwError(() => err);
          }

          return authService.refreshToken().pipe(
            switchMap(() => {
              const newToken = authService.getAccessToken();
              const retryReq = req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              });
              return next(retryReq); // ✅ Retry original request
            }),
            catchError(() => {
              authService.logout();
              router.navigate(['/unauthorized']);
              return throwError(() => err);
            })
          );
        });
      }

      return throwError(() => err);
    })
  );
};
