import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { BusyService } from '../services/busy-service';
import { inject } from '@angular/core';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string, HttpEvent<unknown>>();
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramString = params
      .keys()
      .map((key) => `${key}=${params.get(key)}`)
      .join('&');
    return paramString ? `${url}?${paramString}` : url;
  };

  const invalidateCache = (urlPattern: string) => {
    for (const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key);
      }
    }
  };

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method === 'GET') {
    const cacheResponse = cache.get(cacheKey);
    if (cacheResponse) {
      return of(cacheResponse);
    }
  }

  if (req.method === 'POST' && req.url.includes('/likes')) {
    invalidateCache('/likes');
  }

  if (req.method === 'POST' && req.url.includes('/messages')) {
    invalidateCache('/messages');
  }

  busyService.busy();

  return next(req).pipe(
    delay(500),
    tap((response) => {
      cache.set(cacheKey, response);
    }),
    finalize(() => busyService.idle())
  );
};
