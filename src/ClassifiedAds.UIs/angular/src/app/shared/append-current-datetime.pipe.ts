import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "appendCurrentDateTime"
})
export class AppendCurrentDateTimePipe implements PipeTransform {
  transform(value: string, prefix: string = " "): string {
    return value + prefix + new Date();
  }
}
