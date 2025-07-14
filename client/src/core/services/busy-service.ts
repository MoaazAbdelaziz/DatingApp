import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = signal(0);

  busy(): void {
    this.busyRequestCount.update((current) => current + 1);
  }

  idle(): void {
    this.busyRequestCount.update((current) => Math.max(0, current - 1));
  }
}
