import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastMessage {
    id: number;
    type: ToastType;
    title: string;
    message: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
    private readonly toastsSubject = new BehaviorSubject<ToastMessage[]>([]);
    readonly toasts$ = this.toastsSubject.asObservable();

    private nextId = 1;

    success(title: string, message: string, timeoutMs = 3200): void {
        this.show('success', title, message, timeoutMs);
    }

    error(title: string, message: string, timeoutMs = 4200): void {
        this.show('error', title, message, timeoutMs);
    }

    info(title: string, message: string, timeoutMs = 3000): void {
        this.show('info', title, message, timeoutMs);
    }

    dismiss(id: number): void {
        const next = this.toastsSubject.value.filter((t) => t.id !== id);
        this.toastsSubject.next(next);
    }

    private show(type: ToastType, title: string, message: string, timeoutMs: number): void {
        const toast: ToastMessage = {
            id: this.nextId++,
            type,
            title,
            message
        };

        this.toastsSubject.next([toast, ...this.toastsSubject.value]);

        window.setTimeout(() => this.dismiss(toast.id), timeoutMs);
    }
}
