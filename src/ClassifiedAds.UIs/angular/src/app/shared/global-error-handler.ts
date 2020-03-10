import { ErrorHandler, Injectable } from "@angular/core";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor() {}

  handleError(error: any) {
    // TODO: log message to remote server
    console.error("GlobalErrorHandler " + new Date(), error);
  }
}
