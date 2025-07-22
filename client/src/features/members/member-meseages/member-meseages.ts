import {
  Component,
  effect,
  ElementRef,
  inject,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { Message } from '../../../types/message';
import { MemberService } from '../../../core/services/member-service';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-meseages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-meseages.html',
  styleUrl: './member-meseages.css',
})
export class MemberMeseages implements OnInit {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;
  private messageService = inject(MessageService);
  private memberService = inject(MemberService);
  protected messages = signal<Message[]>([]);
  protected messageContent = '';

  constructor() {
    effect(() => {
      const currentMessage = this.messages();
      if (currentMessage.length > 0) {
        this.scrollToBottom();
      }
    });
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const memberId = this.memberService.member()?.id;
    if (memberId) {
      this.messageService.getMessageThread(memberId).subscribe({
        next: (messages) => {
          this.messages.set(
            messages.map((message) => ({
              ...message,
              currentUserSender: message.senderId !== memberId,
            }))
          );
        },
      });
    }
  }

  sendMessage() {
    const recipientId = this.memberService.member()?.id;
    if (!recipientId) return;
    this.messageService
      .sendMessage(recipientId, this.messageContent)
      .subscribe({
        next: (message) => {
          this.messages.update((messages) => {
            message.currentUserSender = true;
            return [...messages, message];
          });
          this.messageContent = '';
        },
      });
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    });
  }
}
