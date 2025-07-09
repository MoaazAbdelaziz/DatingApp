import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastService } from '../services/toast-service';

export const authGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toastService = inject(ToastService);

  if (accountService.currentUser()) return true;
  else {
    toastService.error('You shall not pass');
    router.navigateByUrl('/');
    return false;
  }
};
