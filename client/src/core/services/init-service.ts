import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { of } from 'rxjs';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private accoutService = inject(AccountService);
  private likesService = inject(LikesService);

  init() {
    const userString = localStorage.getItem('user');
    if (!userString) return of(null);
    const user = JSON.parse(userString);
    this.accoutService.setCurrentUser(user);
    this.likesService.getLikeIds();

    return of(null);
  }
}
