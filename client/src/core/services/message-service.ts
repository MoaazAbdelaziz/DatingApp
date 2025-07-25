import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PaginationResult } from '../../types/pagination';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('container', container);
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return this.http.get<PaginationResult<Message>>(this.baseUrl + 'messages', {
      params,
    });
  }

  getMessageThread(memberId: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + memberId
    );
  }

  sendMessage(recipientId: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', {
      recipientId,
      content,
    });
  }

  deleteMessage(id: string) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
