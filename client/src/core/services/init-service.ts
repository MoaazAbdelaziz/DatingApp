import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { tap } from 'rxjs';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private accoutService = inject(AccountService);
  private likesService = inject(LikesService);

  init() {
    return this.accoutService.refreshToken().pipe(
      tap((user) => {
        if (user) {
          this.accoutService.setCurrentUser(user);
          this.accoutService.startTokenRefreshInterval();
        }
      })
    );
  }
}
