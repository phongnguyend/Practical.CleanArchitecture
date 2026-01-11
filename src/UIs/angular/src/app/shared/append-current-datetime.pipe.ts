import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "appendCurrentDateTime",
  standalone: true,
})
export class AppendCurrentDateTimePipe implements PipeTransform {
  transform(value: string, prefix: string = " "): string {
    return value + prefix + new Date();
  }
}
