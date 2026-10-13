import { AfterViewInit, Component, Injectable, OnDestroy } from "@angular/core";
import { mountToasts, toast, ToastOptions } from "./toast";

@Injectable({ providedIn: "root" })
export class ToastService {
  success(message: string, options?: ToastOptions) {
    return toast.success(message, options);
  }
  error(message: string, options?: ToastOptions) {
    return toast.error(message, options);
  }
  warning(message: string, options?: ToastOptions) {
    return toast.warning(message, options);
  }
  info(message: string, options?: ToastOptions) {
    return toast.info(message, options);
  }
  dismiss(id: number) {
    toast.dismiss(id);
  }
  clear() {
    toast.clear();
  }
}

@Component({ selector: "app-toast-container", standalone: true, template: "" })
export class ToastContainerComponent implements AfterViewInit, OnDestroy {
  private cleanup?: () => void;
  ngAfterViewInit() {
    this.cleanup = mountToasts();
  }
  ngOnDestroy() {
    this.cleanup?.();
  }
}
